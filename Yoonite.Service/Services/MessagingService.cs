using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Yoonite.Common.Models;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface IMessagingService : IBaseService
    {
        GetUserUnreadMessageCountResponse GetUserUnreadMessageCount(GetUserUnreadMessageCountRequest request);
        GetUserMessagesResponse GetUserMessages(GetUserMessagesRequest request);
        GetMessageResponse GetMessage(GetMessageRequest request);
        SaveMessageResponse SaveMessage(SaveMessageRequest request);
        DeleteMessageResponse DeleteMessage(DeleteMessageRequest request);
        MarkMessageAsReadResponse MarkMessageAsRead(MarkMessageAsReadRequest request);

        SendEmailResponse SendEmail(SendEmailRequest request);
        ContactUsResponse ContactUs(ContactUsRequest request);
    }

    public class MessagingService : BaseService, IMessagingService
    {
        public GetUserUnreadMessageCountResponse GetUserUnreadMessageCount(GetUserUnreadMessageCountRequest request)
        {
            try
            {
                var response = new GetUserUnreadMessageCountResponse();
                using (var context = new YooniteEntities())
                {
                    response.Count = context.Messages.AsNoTracking()
                                                     .Count(_ => (_.ToAccountId.Equals(request.UserId)) && !_.IsDeleted && _.IsActive && _.UnRead);
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetUserUnreadMessageCountResponse { ErrorMessage = "Unable to get user unread message count." };

            }
        }

        public GetUserMessagesResponse GetUserMessages(GetUserMessagesRequest request)
        {
            try
            {
                var response = new GetUserMessagesResponse();
                using (var context = new YooniteEntities())
                {
                    var messages = context.Messages.AsNoTracking()
                                                   .Where(_ => (_.ToAccountId.Equals(request.UserId) || _.FromAccountId.Equals(request.UserId))
                                                            && _.Id.Equals(_.ParentId) && !_.IsDeleted && _.IsActive)
                                                   .OrderByDescending(_ => _.UnRead)
                                                   .ThenByDescending(_ => _.DateCreated)
                                                   .Select(_ => new MessageWithChildrenModel
                                                   {
                                                       Id = _.Id,
                                                       ParentId = _.ParentId,
                                                       FromAccountId = _.FromAccountId,
                                                       FromAccount = new AccountListModel
                                                       {
                                                           Id = _.Account.Id,
                                                           FirstName = _.Account.FirstName,
                                                           LastName = _.Account.LastName,
                                                           Email = _.Account.Email,
                                                           PhoneNumber = _.Account.PhoneNumber,
                                                           IsActive = _.Account.IsActive,
                                                           DateCreated = _.Account.DateCreated
                                                       },
                                                       ToAccountId = _.ToAccountId,
                                                       ToAccount = new AccountListModel
                                                       {
                                                           Id = _.Account1.Id,
                                                           FirstName = _.Account1.FirstName,
                                                           LastName = _.Account1.LastName,
                                                           Email = _.Account1.Email,
                                                           PhoneNumber = _.Account1.PhoneNumber,
                                                           IsActive = _.Account1.IsActive,
                                                           DateCreated = _.Account1.DateCreated
                                                       },
                                                       Subject = _.Subject,
                                                       Message = _.Message1,
                                                       UnRead = _.Message11.Any(__ => __.UnRead && __.ToAccountId.Equals(request.UserId)),
                                                       IsActive = _.IsActive,
                                                       IsDeleted = _.IsDeleted,
                                                       DateCreated = _.DateCreated,
                                                       ChildMessages = _.Message11.OrderBy(__ => __.DateCreated)
                                                                                  .Select(__ => new MessageModel
                                                                                  {
                                                                                      Id = __.Id,
                                                                                      ParentId = __.ParentId,
                                                                                      FromAccountId = __.FromAccountId,
                                                                                      FromAccount = new AccountListModel
                                                                                      {
                                                                                          Id = __.Account.Id,
                                                                                          FirstName = __.Account.FirstName,
                                                                                          LastName = __.Account.LastName,
                                                                                          Email = __.Account.Email,
                                                                                          PhoneNumber = __.Account.PhoneNumber,
                                                                                          IsActive = __.Account.IsActive,
                                                                                          DateCreated = __.Account.DateCreated
                                                                                      },
                                                                                      ToAccountId = __.ToAccountId,
                                                                                      Subject = __.Subject,
                                                                                      Message = __.Message1,
                                                                                      IsActive = __.IsActive,
                                                                                      UnRead = __.UnRead,
                                                                                      IsDeleted = __.IsDeleted,
                                                                                      DateCreated = __.DateCreated
                                                                                  }).ToList()
                                                   }).ToList();

                    response.Messages = messages;
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetUserMessagesResponse { ErrorMessage = "Unable to get user messages." };

            }
        }

        public GetMessageResponse GetMessage(GetMessageRequest request)
        {
            try
            {
                var response = new GetMessageResponse();
                using (var context = new YooniteEntities())
                {
                    var message = context.Messages.AsNoTracking().FirstOrDefault(_ => _.Id.Equals(request.MessageId));
                    if (message == null) throw new ApplicationException($"Message does not exist for id {request.MessageId}");
                    response.Message = new MessageModel
                    {
                        Id = message.Id,
                        ParentId = message.ParentId,
                        FromAccountId = message.FromAccountId,
                        FromAccount = new AccountListModel
                        {
                            Id = message.Account.Id,
                            FirstName = message.Account.FirstName,
                            LastName = message.Account.LastName,
                            Email = message.Account.Email,
                            PhoneNumber = message.Account.PhoneNumber,
                            IsActive = message.Account.IsActive,
                            DateCreated = message.Account.DateCreated
                        },
                        ToAccountId = message.ToAccountId,
                        ToAccount = new AccountListModel
                        {
                            Id = message.Account1.Id,
                            FirstName = message.Account1.FirstName,
                            LastName = message.Account1.LastName,
                            Email = message.Account1.Email,
                            PhoneNumber = message.Account1.PhoneNumber,
                            IsActive = message.Account1.IsActive,
                            DateCreated = message.Account1.DateCreated
                        },
                        Subject = message.Subject,
                        Message = message.Message1,
                        UnRead = message.UnRead,
                        IsActive = message.IsActive,
                        IsDeleted = message.IsDeleted,
                        DateCreated = message.DateCreated
                    };
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetMessageResponse { ErrorMessage = "Unable to get message." };

            }
        }

        public SaveMessageResponse SaveMessage(SaveMessageRequest request)
        {
            try
            {
                var response = new SaveMessageResponse();
                using (var context = new YooniteEntities())
                {
                    var message = context.Messages.FirstOrDefault(_ => _.Id.Equals(request.Message.Id));
                    if (message == null)
                    {
                        var newGuid = Guid.NewGuid();
                        message = new Message
                        {
                            Id = newGuid,
                            ParentId = request.Message.ParentId.Equals(Guid.Empty) ? newGuid : request.Message.ParentId,
                            FromAccountId = request.Message.FromAccountId,
                            ToAccountId = request.Message.ToAccountId,
                            UnRead = true,
                            IsActive = true,
                            IsDeleted = false,
                            DateCreated = DateTimeOffset.Now,
                        };
                        context.Messages.Add(message);
                    }

                    message.Subject = request.Message.Subject;
                    message.Message1 = request.Message.Message;
                    context.SaveChanges();

                    response.MessageId = message.Id;
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SaveMessageResponse { ErrorMessage = "Unable to save message." };

            }
        }

        public DeleteMessageResponse DeleteMessage(DeleteMessageRequest request)
        {
            try
            {
                var response = new DeleteMessageResponse();
                using (var context = new YooniteEntities())
                {
                    var messages = context.Messages.Where(_ => _.ParentId.Equals(request.MessageId)).ToList();
                    foreach (var message in messages)
                    {
                        message.IsDeleted = true;
                        message.IsActive = false;
                    }
                    context.SaveChanges();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new DeleteMessageResponse { ErrorMessage = "Unable to delete message." };

            }
        }

        public MarkMessageAsReadResponse MarkMessageAsRead(MarkMessageAsReadRequest request)
        {
            try
            {
                var response = new MarkMessageAsReadResponse();
                using (var context = new YooniteEntities())
                {
                    var messages = context.Messages.Where(_ => _.ParentId.Equals(request.ParentMessageId) && _.ToAccountId.Equals(request.UserId));
                    foreach (var message in messages)
                        message.UnRead = false;
                    context.SaveChanges();
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new MarkMessageAsReadResponse { ErrorMessage = "Unable to mark message as read." };

            }
        }


        public SendEmailResponse SendEmail(SendEmailRequest request)
        {
            try
            {
                var response = new SendEmailResponse();
                var client = new SendGridClient(ConfigurationManager.AppSettings["SendGridAPIKey"]);
                var sendGridMessage = new SendGridMessage();
                sendGridMessage.From = new EmailAddress(request.FromEmail, request.FromName);
                sendGridMessage.AddTos(request.Recipients.Select(_ => new EmailAddress { Email = _ }).ToList());

                var emailContent = GetEmailContent(request.EmailType);
                sendGridMessage.Subject = emailContent.Subject;
                sendGridMessage.PlainTextContent = this.ReplaceParameters(emailContent.PlainTextContent, request.Parameters);
                sendGridMessage.HtmlContent = this.ReplaceParameters(emailContent.HtmlContent, request.Parameters);

                var sendEmailResponse = client.SendEmailAsync(sendGridMessage).Result;
                if (sendEmailResponse.StatusCode != HttpStatusCode.Accepted)
                    throw new ApplicationException($"Unable to send email, status code {sendEmailResponse.StatusCode}");
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SendEmailResponse { ErrorMessage = ex.Message };

            }
        }

        public ContactUsResponse ContactUs(ContactUsRequest request)
        {
            try
            {
                var response = new ContactUsResponse();
                var client = new SendGridClient(ConfigurationManager.AppSettings["SendGridAPIKey"]);
                var sendGridMessage = new SendGridMessage();
                sendGridMessage.From = new EmailAddress(request.FromEmail, request.FromName);
                sendGridMessage.AddTos(this.GetSupportEmails());
                sendGridMessage.Subject = request.Subject;
                sendGridMessage.PlainTextContent = request.Message;
                sendGridMessage.HtmlContent = request.Message;

                var sendEmailResponse = client.SendEmailAsync(sendGridMessage).Result;
                if (sendEmailResponse.StatusCode != HttpStatusCode.Accepted)
                    throw new ApplicationException($"Unable to send email, status code {sendEmailResponse.StatusCode}");
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new ContactUsResponse { ErrorMessage = ex.Message };
            }
        }


        private (string Subject, string PlainTextContent, string HtmlContent) GetEmailContent(EmailType emailType)
        {
            switch (emailType)
            {
                case EmailType.ResetPassword:
                    return GetResetEmailContent();
                default:
                    throw new ApplicationException($"Email type {emailType} was not found.");
            }
        }

        private (string Subject, string PlainTextContent, string HtmlContent) GetResetEmailContent()
        {
            var subject = "A password reset has been requested.";

            var htmlFilePath = "C:\\Users\\Development\\Desktop\\Yoonite\\Yoonite.Common\\HtmlTemplates\\ResetEmailHtmlContent.html";
            // TODO: TREY 8/11/2019 Figure out the correct way to do this.
            //"{AppDomain.CurrentDomain.BaseDirectory}/../Yoonite.Common/HtmlTemplates/ResetEmailHtmlContent.html";

            //LoggingService.LogError(new LogErrorRequest { ex = new ApplicationException(AppDomain.CurrentDomain.BaseDirectory) });

            var plainTextContent = $"Your Yoonite password has been reset. Please click on the following link to reset your password. [[url]]";
            var htmlContent = $"Your Yoonite password has been reset. Please click on the following link to reset your password. [[url]]";
            return (subject, plainTextContent, htmlContent);
            //using (var reader = File.OpenText(htmlFilePath))
            //    return (subject, plainTextContent, reader.ReadToEnd());
        }

        private string ReplaceParameters(string content, Dictionary<string, string> parameters)
        {
            var retval = content;
            foreach (var parameter in parameters)
                retval = content.Replace($"[[{parameter.Key}]]", parameter.Value);
            return retval;
        }

        private List<EmailAddress> GetSupportEmails()
        {
            return new List<EmailAddress> { new EmailAddress { Email = "support@unieksoftware.com" } };
        }
    }
}