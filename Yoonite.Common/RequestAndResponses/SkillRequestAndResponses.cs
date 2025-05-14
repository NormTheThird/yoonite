using System.Collections.Generic;
using System.Runtime.Serialization;
using Yoonite.Common.Models;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class GetSkillsRequest : BaseActiveRequest { }

    [DataContract]
    public class GetSkillsResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public List<SkillModel> Skills { get; set; } = new List<SkillModel>();
    }

    [DataContract]
    public class SaveSkillRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public SkillModel Skill { get; set; } = new SkillModel();
    }

    [DataContract]
    public class SaveSkillResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public SkillModel Skill { get; set; } = new SkillModel();
    }
}