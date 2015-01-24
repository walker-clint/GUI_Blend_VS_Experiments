using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Dispatchr.Models.Poco;
using Microsoft.AspNet.Identity;

namespace Dispatchr.WebService.Controllers
{
    public class UserController : ApiController
    {
        public Models.Poco.User Get()
        {
            var db = new SolarizrEntities();
            var identity = User.Identity as ClaimsIdentity;
            var email = identity != null ? identity.FindFirstValue(ClaimsIdentity.DefaultNameClaimType) : "john@solarizr.onmicrosoft.com";
            var record = db.Set<Dispatchr.Models.Poco.User>().FirstOrDefault(user => user.Email == email);
            return record;
        }
    }
}
