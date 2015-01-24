using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;
using Dispatchr.Web.Models;
using Microsoft.ServiceBus.Notifications;

namespace Dispatchr.Web.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View(new NotificationModel());
        }


        // POST: Notification/Create
        [HttpPost]
        public async Task<ActionResult> Create(NotificationModel notificationModel)
        {
            try
            {
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(
                    "Endpoint=sb://solarizrnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=AaParVcTwT/fdce0QvY/xJU5JxTA7/P/WzkpBZkWM4Y=",
                    "solarizrnotificationhub");

                string payload = string.Empty;
                if (!string.IsNullOrWhiteSpace(notificationModel.Payload))
                {
                    payload = HttpUtility.HtmlEncode(notificationModel.Payload);
                }

                string toast = "<toast launch=\""+ payload + "\"><visual><binding template=\"ToastText01\"><text id=\"1\">" +
                                notificationModel.NotificationString + "</text></binding></visual></toast>";
                NotificationOutcome result;
                if (string.IsNullOrWhiteSpace(notificationModel.UserName))
                {
                    result = await hub.SendWindowsNativeNotificationAsync(toast);
                }
                else
                {
                    result = await hub.SendWindowsNativeNotificationAsync(toast,
                        string.Format("User:{0}", notificationModel.UserName));
                }
                //return RedirectToAction("Index");
                notificationModel.NotificationString = result.State.ToString();
                return View("Index", notificationModel);
            }
            catch (Exception ex)
            {
                string m = ex.Message;
                return View();
            }
        }
    }
}