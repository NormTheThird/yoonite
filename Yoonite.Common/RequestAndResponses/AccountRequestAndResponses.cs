using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yoonite.Common.Models;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class GetAccountRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetAccountResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public AccountModel Account { get; set; } = new AccountModel();
    }

    [DataContract]
    public class GetAccountWithSkillsResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public AccountModel Account { get; set; } = new AccountModel();
        [DataMember(IsRequired = true)] public List<Guid> AccountSkills { get; set; } = new List<Guid>();
    }


    [DataContract]
    public class GetAccountsRequest : BaseActiveRequest { }

    [DataContract]
    public class GetAccountsResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public List<AccountModel> Accounts { get; set; } = new List<AccountModel>();
    }

    [DataContract]
    public class GetAccountsWithSkillsResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public List<AccountWithSkillsModel> Accounts { get; set; } = new List<AccountWithSkillsModel>();
    }

    [DataContract]
    public class SaveAccountRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public AccountModel Account { get; set; } = new AccountModel();
        [DataMember(IsRequired = true)] public List<Guid> AccountSkills { get; set; } = new List<Guid>();
    }

    [DataContract]
    public class SaveAccountResponse : BaseResponse { }

    [DataContract]
    public class SaveAccountProfileImageRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public Guid ProfileImageStorageId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class SaveAccountProfileImageResponse : BaseResponse { }

    [DataContract]
    public class ChangeAccountStatusRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class ChangeAccountStatusResponse : BaseResponse { }
}