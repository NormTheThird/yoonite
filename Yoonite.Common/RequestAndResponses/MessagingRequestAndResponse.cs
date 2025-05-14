using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yoonite.Common.Models;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class GetUserUnreadMessageCountRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid UserId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetUserUnreadMessageCountResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public int Count { get; set; } = 0;
    }

    [DataContract]
    public class GetUserMessagesRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid UserId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetUserMessagesResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public List<MessageWithChildrenModel> Messages { get; set; } = new List<MessageWithChildrenModel>();
    }

    [DataContract]
    public class GetMessageRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid MessageId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetMessageResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public MessageModel Message { get; set; } = new MessageModel();
    }

    [DataContract]
    public class GetMessageWithChildrenRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid MessageId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetMessageWithChildrenResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public MessageWithChildrenModel Message { get; set; } = new MessageWithChildrenModel();
    }

    [DataContract]
    public class SaveMessageRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public MessageModel Message { get; set; } = new MessageModel();
    }

    [DataContract]
    public class SaveMessageResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public Guid MessageId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class DeleteMessageRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid MessageId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class DeleteMessageResponse : BaseResponse { }

    [DataContract]
    public class MarkMessageAsReadRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid UserId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public Guid ParentMessageId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class MarkMessageAsReadResponse : BaseResponse { }

    [DataContract]
    public class SendEmailRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string FromName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string FromEmail { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public List<string> Recipients { get; set; } = new List<string>();
        [DataMember(IsRequired = true)] public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        [DataMember(IsRequired = true)] public EmailType EmailType { get; set; } = EmailType.Unknown;
    }

    [DataContract]
    public class SendEmailResponse : BaseResponse { }

    [DataContract]
    public class ContactUsRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string FromName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string FromEmail { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Subject { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Message { get; set; } = string.Empty;
    }

    [DataContract]
    public class ContactUsResponse : BaseResponse { }

    public enum EmailType
    {
        Unknown,
        ResetPassword,

    }
}