using System.Runtime.Serialization;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class BaseRequest
    {

    }

    [DataContract]
    public class BaseActiveRequest
    {
        [DataMember(IsRequired = true)] public bool GetActiveAndInactive { get; set; } = false;
    }

    [DataContract]
    public class BaseResponse
    {
        [DataMember(IsRequired = true)] public bool IsSuccess { get; set; } = false;
        [DataMember(IsRequired = true)] public string ErrorMessage { get; set; } = string.Empty;
    }
}