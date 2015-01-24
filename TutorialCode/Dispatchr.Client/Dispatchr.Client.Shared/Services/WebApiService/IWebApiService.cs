using System;
using System.Threading.Tasks;
namespace Dispatchr.Client.Services
{
    public interface IWebApiService
    {
        Task<T> GetAsync<T>(Uri uri);
        Task PutAsync<T>(Uri uri, T payload);
    }
}
