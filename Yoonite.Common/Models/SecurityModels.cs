using System;
using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class SecurityModel
    {
        [DataMember(IsRequired = true)] public Guid Id { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public string FirstName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string LastName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Email { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public bool IsSystemAdmin { get; set; } = false;
    }
}