using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class AddressModel : BaseModel
    {
        [DataMember(IsRequired = true)] public string Address1 { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Address2 { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string City { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string State { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string ZipCode { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public decimal Latitude { get; set; } = 0.0m;
        [DataMember(IsRequired = true)] public decimal Longitude { get; set; } = 0.0m;
    }
}