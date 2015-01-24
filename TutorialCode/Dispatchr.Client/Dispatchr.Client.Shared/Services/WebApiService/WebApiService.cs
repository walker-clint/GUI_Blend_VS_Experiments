using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;

namespace Dispatchr.Client.Services
{
    public class WebApiService : IWebApiService
    {
        private readonly IAdalService _adalService;

        public WebApiService(IAdalService adalService)
        {
            _adalService = adalService;
        }

        public async Task<T> GetAsync<T>(Uri uri)
        {
            using (HttpClient http = WrappedHttpClient())
            {
                HttpResponseMessage response = await http.GetAsync(uri);
                if (response.StatusCode != HttpStatusCode.Ok)
                    throw new WebApiException {Status = response.StatusCode};
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public async Task PutAsync<T>(Uri uri, T payload)
        {
            using (HttpClient http = WrappedHttpClient())
            {
                try
                {
                    string data = JsonConvert.SerializeObject(payload);
                    http.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response =
                        await http.PutAsync(uri, new HttpStringContent(data, UnicodeEncoding.Utf8, "application/json"));
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private HttpClient WrappedHttpClient()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Accept", "application/json");
            http.DefaultRequestHeaders.Add("Authorization", _adalService.AuthHeader);

            return http;
        }

        public class WebApiException : Exception
        {
            public HttpStatusCode Status { get; set; }
        }
    }
}