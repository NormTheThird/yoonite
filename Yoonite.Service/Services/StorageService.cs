using System;
using System.Linq;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface IStorageService
    {
        SaveStorageResponse SaveStorage(SaveStorageRequest request);
    }

    public class StorageService : BaseService, IStorageService
    {

        public SaveStorageResponse SaveStorage(SaveStorageRequest request)
        {
            try
            {
                var response = new SaveStorageResponse();
                using (var context = new YooniteEntities())
                {
                    var storage = context.Storages.FirstOrDefault(_ => _.Id.Equals(request.Storage.Id));
                    if (storage == null)
                    {
                        storage = new Storage
                        {
                            Id = Guid.NewGuid(),
                            AzureStorageId = request.Storage.AzureStorageId,
                            IsActive = true,
                            IsDeleted = false,
                            DateCreated = DateTimeOffset.Now
                        };
                        context.Storages.Add(storage);
                    }

                    storage.FileName = request.Storage.FileName;
                    storage.MimeType = request.Storage.MimeType;
                    storage.FileSizeInBytes = request.Storage.FileSizeInBytes;
                    context.SaveChanges();

                    response.StorageId = storage.Id;
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SaveStorageResponse { ErrorMessage = "Unable to save storage." };
            }
        }
    }
}