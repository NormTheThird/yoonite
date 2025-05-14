using System;
using System.Runtime.Serialization;
using Yoonite.Common.Models;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class SaveStorageRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public StorageModel Storage { get; set; } = new StorageModel();
    }

    [DataContract]
    public class SaveStorageResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public Guid StorageId { get; set; } = Guid.Empty;
    }
}