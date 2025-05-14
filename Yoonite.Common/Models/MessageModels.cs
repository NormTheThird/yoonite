using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class MessageModel : ActiveModel
    {
        [DataMember(IsRequired = true)] public Guid ParentId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public Guid FromAccountId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public AccountListModel FromAccount { get; set; } = new AccountListModel();
        [DataMember(IsRequired = true)] public Guid ToAccountId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public AccountListModel ToAccount { get; set; } = new AccountListModel();
        [DataMember(IsRequired = true)] public string Subject { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Message { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public bool UnRead { get; set; } = false;
        [DataMember(IsRequired = true)] public bool IsDeleted { get; set; } = false;
    }

    [DataContract]
    public class MessageWithChildrenModel : MessageModel
    {
        [DataMember(IsRequired = true)] public List<MessageModel> ChildMessages { get; set; } = new List<MessageModel>();
    }
}