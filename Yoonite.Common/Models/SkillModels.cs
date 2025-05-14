using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class SkillModel : ActiveModel
    {
        [DataMember(IsRequired = true)] public string Name { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Description { get; set; } = string.Empty;
    }
}