using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yoonite.Common.Models;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class GetUserProjectsRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetUserProjectsResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public List<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
    }

    [DataContract]
    public class GetProjectsRequest : BaseActiveRequest { }

    [DataContract]
    public class GetProjectsResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public List<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
    }

    [DataContract]
    public class SaveProjectRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public ProjectModel Project { get; set; } = new ProjectModel();
        [DataMember(IsRequired = true)] public List<Guid> ProjectSkills { get; set; } = new List<Guid>();
    }

    [DataContract]
    public class SaveProjectResponse : BaseResponse { }

    [DataContract]
    public class DeleteProjectRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid ProjectId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class DeleteProjectResponse : BaseResponse { }
}