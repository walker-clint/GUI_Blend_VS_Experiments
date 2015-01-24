using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dispatchr.Client.Services
{
    public class BlobService : IBlobService
    {

//#if WINDOWS_PHONE_APP

//        public Task UploadAsync(StorageFile source, string name = null)
//        {
//            throw new NotImplementedException();
//        }

//        public Uri GetReadPath(string name)
//        {
//            throw new NotImplementedException();
//        }

//#elif WINDOWS_APP

        BlobHelper _helper;
        string _containerName;
        public BlobService(ISettings settings)
        {
            _containerName = settings.BlobContainerName;
            _helper = new BlobHelper(settings.BlobAccountName, settings.BlobAccessKey, settings.TransferGroupName);
        }

        public Uri GetReadPath(string name)
        {
            return _helper.Blob.GetSas(Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPermissions.Read, TimeSpan.FromDays(365 * 2), _containerName, name);
        }
      
        public async Task UploadAsync(StorageFile source, string name = null)
        {
            name = name ?? source.Name;
            // this upload method uses background task
            // await _helper.Blob.UploadBackgroundAsync(source, _containerName, name);
            await _helper.Blob.UploadAsync(source, _containerName, name);
        }

//#endif
    }
}
