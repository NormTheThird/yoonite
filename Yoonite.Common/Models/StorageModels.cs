using System;
using System.Runtime.Serialization;

namespace Yoonite.Common.Models
{
    [DataContract]
    public class StorageModel : ActiveModel
    {
        [DataMember(IsRequired = true)] public Guid AzureStorageId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public string FileName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string MimeType { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public int FileSizeInBytes { get; set; } = 0;
        [DataMember(IsRequired = true)] public bool IsDeleted { get; set; } = false;
    }
}