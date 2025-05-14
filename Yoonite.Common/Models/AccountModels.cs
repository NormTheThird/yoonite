using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class AccountListModel : ActiveModel
    {
        [DataMember(IsRequired = true)] public Guid? ProfileImageStorageId { get; set; } = null;
        [DataMember(IsRequired = true)] public StorageModel ProfileImageStorage { get; set; } = new StorageModel();
        [DataMember(IsRequired = true)] public string FirstName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string LastName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Email { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string PhoneNumber { get; set; } = string.Empty;
    }

    [DataContract]
    public class AccountModel : AccountListModel
    {
        [DataMember(IsRequired = true)] public Guid AddressId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public AddressModel Address { get; set; } = new AddressModel();
        [DataMember(IsRequired = true)] public string AltPhoneNumber { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string FacebookUrl { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string TwitterUrl { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string LinkedinUrl { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string InstagramUrl { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string WebsiteUrl { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Bio { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public int RefreshTokenMinutes { get; set; } = 0;
        [DataMember(IsRequired = true)] public DateTime? DateOfBirth { get; set; } = null;
    }

    [DataContract]
    public class AccountWithSkillsModel : AccountModel
    {
        [DataMember(IsRequired = true)] public List<SkillModel> Skills { get; set; } = new List<SkillModel>();
    }
}