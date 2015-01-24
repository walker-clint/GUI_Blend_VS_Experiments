using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dispatchr.Models.Poco;

namespace Dispatchr.WebService.Controllers
{
    public class SettingsController : ApiController
    {
        private SolarizrEntities db = new SolarizrEntities();

        // GET: api/Settings
        public IEnumerable<Dispatchr.Models.Poco.Status> Get()
        {
            var records = db.Set<Dispatchr.Models.Poco.Status>();
            return records;
        }

        // GET: api/Settings/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Settings
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Settings/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Settings/5
        public void Delete(int id)
        {
        }
    }
}
