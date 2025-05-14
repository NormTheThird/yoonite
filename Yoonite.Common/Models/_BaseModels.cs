using System;
using System.Runtime.Serialization;
using Yoonite.Common.Enumerations;
using Yoonite.Common.Helpers;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class BaseModel
    {
        [DataMember(IsRequired = true)] public Guid Id { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public DateTimeOffset DateCreated { get; set; } = DateTimeConvert.GetTimeZoneDateTime(TimeZoneInfoId.CentralStandardTime);
    }

    [DataContract]
    public class ActiveModel : BaseModel
    {
        [DataMember(IsRequired = true)] public bool IsActive { get; set; } = false;
    }
}