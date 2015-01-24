using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Dispatchr.Client.Services;

namespace Dispatchr.Client.ServiceProxies
{
    class UserServiceProxy : IUserServiceProxy
    {
        Services.ISettings _settings;
        Services.IWebApiService _webApiService;

        public UserServiceProxy(Services.ISettings settings, Services.IWebApiService webApiService)
        {
            _settings = settings;
            _webApiService = webApiService;
        }

        public async System.Threading.Tasks.Task<Models.User> GetAsync()
        {
            if (_settings.UseSampleOnly)
            {
                // sample data

                var user = SampleData.Factory.GenUser();
                return user;
            }
            else
            {
                // load
                var url = System.IO.Path.Combine(_settings.WebApiBase, "User");
                var uri = new Uri(url);
                var poco = await _webApiService.GetAsync<Dispatchr.Models.Poco.User>(uri);
                return Models.User.FromPoco(poco);
            }
        }
    }
}
