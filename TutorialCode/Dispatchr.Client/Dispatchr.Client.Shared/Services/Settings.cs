using Dispatchr.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public interface ISettings
    {
        string TransferGroupName { get; set; }
        string WebApiBase { get; set; }
        string BingMapKey { get; set; }
        int DaysForState { get; set; }
        bool UseSampleOnly { get; set; }
        int AppointmentCount { get; }
        string ServiceResourceName { get; }
        string BlobAccountName { get; }
        string BlobContainerName { get; }
        string BlobAccessKey { get; }
        string NotificationHubName { get; }
        string NotificationHubEndpoint { get; }
        string AdTenant { get; }
    }

    public class Settings : ISettings
    {
        public Settings()
        {
            /*
             * Because the data has been clensed you will not be able to:
             * 1. generate static map imagary (get your own key at bingmapsportal.com)
             * 2. save images to blob (get your own key at azure.com)
             * 3. register for push notifications (get your own key at auzre.com)
             */

            WebApiBase = "https://solarizrservice.azurewebsites.net/api/";
            BingMapKey = "1236zV2N1Keldfvh_skVQ3p36hqY4eedgEs5mIAFr3r9uD77luOsTW0AklquxyDF";
            DaysForState = 2;
            AppointmentCount = 5;
            ServiceResourceName = "https://solarizr.onmicrosoft.com/WebApp-solarizr.azurewebsites.net";
            BlobAccountName = "solarizr";
            BlobContainerName = "default";
            BlobAccessKey = "123cZYT16a+e5q0b2W7s0kuqpawrKydiAAELfeBXlkVayhsqkAtUtqqlUQmDgyKcd56MnWA5zHypEW/1Yswo/A==";
            NotificationHubName = "solarizrnotificationhub";
            NotificationHubEndpoint = "Endpoint=sb://solarizrnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=123YCqa2bKj2tnJw8a5wxKs06Qmp6WYebVyZgpmOhZc=";
            TransferGroupName = AdTenant = "solarizr.onmicrosoft.com";
        }

        public string AdTenant { get; set; }
        public string WebApiBase { get; set; }
        public string BingMapKey { get; set; }
        public int DaysForState { get; set; }
        public bool UseSampleOnly
        {
            // put/pull directly from settings
            get { return StorageHelper.GetSetting<bool>("Setting.SampleOnly", true); }
            set { StorageHelper.SetSetting("Setting.SampleOnly", value); }
        }
        public int AppointmentCount { get; set; }
        public string ServiceResourceName { get; set; }
        public string BlobAccountName { get; set; }
        public string BlobContainerName { get; set; }
        public string BlobAccessKey { get; set; }
        public string NotificationHubName { get; set; }
        public string NotificationHubEndpoint { get; set; }
        public string TransferGroupName { get; set; }
    }
}
