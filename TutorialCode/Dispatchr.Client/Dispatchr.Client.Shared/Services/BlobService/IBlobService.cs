using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
namespace Dispatchr.Client.Services
{
    public interface IBlobService
    {
        Task UploadAsync(StorageFile source, string name = null);
        Uri GetReadPath(string name);
    }
}
