using System;
using System.Linq;
using Yoonite.Common.Models;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface IAccountService
    {
        GetAccountResponse GetAccount(GetAccountRequest request);
        GetAccountWithSkillsResponse GetAccountWithSkills(GetAccountRequest request);
        GetAccountsResponse GetAccounts(GetAccountsRequest request);
        GetAccountsWithSkillsResponse GetAccountsWithSkills(GetAccountsRequest request);
        SaveAccountResponse SaveAccount(SaveAccountRequest request);
        SaveAccountProfileImageResponse SaveAccountProfileImage(SaveAccountProfileImageRequest request);
        ChangeAccountStatusResponse ChangeAccountStatus(ChangeAccountStatusRequest request);
    }

    public class AccountService : BaseService, IAccountService
    {
        public GetAccountResponse GetAccount(GetAccountRequest request)
        {
            try
            {
                var response = new GetAccountResponse();
                using (var context = new YooniteEntities())
                {
                    var account = context.Accounts.FirstOrDefault(_ => _.Id.Equals(request.AccountId));
                    if (account == null) return new GetAccountResponse { ErrorMessage = $"Account does not exist for id {request.AccountId}" };
                    response.Account = MapperService.Map<Account, AccountModel>(account);
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetAccountResponse { ErrorMessage = "Unable to get account." };
            }
        }

        public GetAccountWithSkillsResponse GetAccountWithSkills(GetAccountRequest request)
        {
            try
            {
                var response = new GetAccountWithSkillsResponse();
                using (var context = new YooniteEntities())
                {
                    var account = context.Accounts.FirstOrDefault(_ => _.Id.Equals(request.AccountId));
                    if (account == null) return new GetAccountWithSkillsResponse { ErrorMessage = $"Account does not exist for id {request.AccountId}" };
                    response.Account = MapperService.Map<Account, AccountModel>(account);

                    var accountSkills = context.AccountSkillCrossLinks.Where(_ => _.AccountId.Equals(request.AccountId));
                    if (accountSkills != null)
                        response.AccountSkills = accountSkills.Select(_ => _.SkillId).ToList();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetAccountWithSkillsResponse { ErrorMessage = "Unable to get account with skills." };
            }
        }

        public GetAccountsResponse GetAccounts(GetAccountsRequest request)
        {
            try
            {
                var response = new GetAccountsResponse();
                using (var context = new YooniteEntities())
                {
                    var accounts = context.Accounts.ToList();
                    response.Accounts = MapperService.MapToList<Account, AccountModel>(accounts);
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetAccountsResponse { ErrorMessage = "Unable to get accounts." };
            }
        }

        public GetAccountsWithSkillsResponse GetAccountsWithSkills(GetAccountsRequest request)
        {
            try
            {
                var response = new GetAccountsWithSkillsResponse();
                using (var context = new YooniteEntities())
                {
                    var accounts = context.Accounts.Where(_ => _.IsActive)
                                                   .Select(_ => new AccountWithSkillsModel
                                                   {
                                                       Id = _.Id,
                                                       ProfileImageStorageId = _.ProfileImageStorageId,
                                                       ProfileImageStorage = _.ProfileImageStorageId == null ? null : new StorageModel
                                                       {
                                                           Id = _.Storage.Id,
                                                           AzureStorageId = _.Storage.AzureStorageId,
                                                           FileName = _.Storage.FileName,
                                                           MimeType = _.Storage.MimeType,
                                                           FileSizeInBytes = _.Storage.FileSizeInBytes,
                                                           IsActive = _.Storage.IsActive,
                                                           IsDeleted = _.Storage.IsDeleted,
                                                           DateCreated = _.Storage.DateCreated,
                                                       },
                                                       AddressId = _.AddressId,
                                                       Address = new AddressModel
                                                       {
                                                           Id = _.Address.Id,
                                                           Address1 = _.Address.Address1,
                                                           Address2 = _.Address.Address2,
                                                           City = _.Address.City,
                                                           State = _.Address.State,
                                                           ZipCode = _.Address.ZipCode,
                                                           Latitude = _.Address.Latitude,
                                                           Longitude = _.Address.Longitude,
                                                           DateCreated = _.Address.DateCreated
                                                       },
                                                       FirstName = _.FirstName,
                                                       LastName = _.LastName,
                                                       Email = _.Email,
                                                       PhoneNumber = _.PhoneNumber,
                                                       AltPhoneNumber = _.AltPhoneNumber,
                                                       FacebookUrl = _.FacebookUrl,
                                                       TwitterUrl = _.TwitterUrl,
                                                       LinkedinUrl = _.LinkedInUrl,
                                                       InstagramUrl = _.InstagramUrl,
                                                       WebsiteUrl = _.WebsiteUrl,
                                                       Bio = _.Bio,
                                                       Skills = _.AccountSkillCrossLinks.Where(xl => xl.Skill.IsActive)
                                                                                        .Select(xl => new SkillModel
                                                                                        {
                                                                                            Id = xl.Skill.Id,
                                                                                            Name = xl.Skill.Name,
                                                                                            Description = xl.Skill.Description,
                                                                                            IsActive = xl.Skill.IsActive,
                                                                                            DateCreated = xl.Skill.DateCreated
                                                                                        }).ToList(),
                                                       DateCreated = _.DateCreated,
                                                   });
                    response.Accounts = accounts.ToList();
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetAccountsWithSkillsResponse { ErrorMessage = "Unable to get accounts with skills." };
            }
        }

        public SaveAccountResponse SaveAccount(SaveAccountRequest request)
        {
            try
            {
                var response = new SaveAccountResponse();
                using (var context = new YooniteEntities())
                {
                    var account = context.Accounts.FirstOrDefault(_ => _.Id.Equals(request.Account.Id));
                    if (account == null) return new SaveAccountResponse { ErrorMessage = $"Account does not exist for id {request.Account.Id}" };
                    MapperService.Map(request.Account, account);

                    foreach (var skill in request.AccountSkills)
                    {
                        var accountSkill = context.AccountSkillCrossLinks.FirstOrDefault(_ => _.AccountId.Equals(account.Id) && _.SkillId.Equals(skill));
                        if (accountSkill == null)
                        {
                            accountSkill = new AccountSkillCrossLink { Id = Guid.NewGuid(), AccountId = account.Id, SkillId = skill, DateCreated = DateTimeOffset.Now };
                            context.AccountSkillCrossLinks.Add(accountSkill);
                        }
                    }

                    var removeAccountSkills = context.AccountSkillCrossLinks.Where(_ => _.AccountId.Equals(account.Id) && !request.AccountSkills.Contains(_.SkillId));
                    context.AccountSkillCrossLinks.RemoveRange(removeAccountSkills);
                    context.SaveChanges();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SaveAccountResponse { ErrorMessage = "Unable to save account." };
            }
        }

        public SaveAccountProfileImageResponse SaveAccountProfileImage(SaveAccountProfileImageRequest request)
        {
            try
            {
                var response = new SaveAccountProfileImageResponse();
                using (var context = new YooniteEntities())
                {
                    var account = context.Accounts.FirstOrDefault(_ => _.Id.Equals(request.AccountId));
                    if (account == null) return new SaveAccountProfileImageResponse { ErrorMessage = $"Account does not exist for id {request.AccountId}" };
                    account.ProfileImageStorageId = request.ProfileImageStorageId;
                    context.SaveChanges();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SaveAccountProfileImageResponse { ErrorMessage = "Unable to save account profile image." };
            }
        }

        public ChangeAccountStatusResponse ChangeAccountStatus(ChangeAccountStatusRequest request)
        {
            try
            {
                var response = new ChangeAccountStatusResponse();
                using (var context = new YooniteEntities())
                {
                    var account = context.Accounts.FirstOrDefault(a => a.Id.Equals(request.AccountId));
                    if (account == null) throw new ApplicationException($"Account does not exist for id {request.AccountId}");
                    account.IsActive = !account.IsActive;
                    context.SaveChanges();
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new ChangeAccountStatusResponse { ErrorMessage = "Unable to get account." };
            }
        }
    }
}