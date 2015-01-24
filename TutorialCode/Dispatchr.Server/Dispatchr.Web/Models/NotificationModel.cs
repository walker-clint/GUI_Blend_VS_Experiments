using System.ComponentModel.DataAnnotations;

namespace Dispatchr.Web.Models
{
    public class NotificationModel
    {
        [Required]
        public string NotificationString { get; set; }

        public string UserName { get; set; }
        public string Payload { get; set; }
    }
}