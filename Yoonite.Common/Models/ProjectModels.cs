using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class ProjectModel : ActiveModel
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public string ProjectName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string CompanyName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Description { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Url { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string City { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string State { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string ZipCode { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public bool CanBeRemote { get; set; } = false;
        [DataMember(IsRequired = true)] public bool IsDeleted { get; set; } = false;
        [DataMember(IsRequired = true)] public List<SkillModel> ProjectSkills { get; set; } = new List<SkillModel>();
    }
}