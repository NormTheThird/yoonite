using System;
using System.Runtime.Serialization;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class LogErrorRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Exception ex { get; set; } = null;
    }

    [DataContract]
    public class LogErrorResponse : BaseResponse { }
}