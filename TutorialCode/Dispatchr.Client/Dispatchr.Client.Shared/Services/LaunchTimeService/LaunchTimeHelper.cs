using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    class LaunchTimeHelper
    {
        private readonly string _visitKey = "App-Launch-DateTime-Setting";

        public LaunchTimeHelper(DateTime? now = null)
        {
            var lastVisitString = Common.StorageHelper.GetSetting(_visitKey, string.Empty);
            var lastVisit = DateTime.MinValue;
            if (!DateTime.TryParse(lastVisitString, out lastVisit))
                lastVisit = DateTime.MinValue;
            this.LastVisit = lastVisit;

            if (now.HasValue)
                Common.StorageHelper.SetSetting(_visitKey, (this.ThisVisit = now.Value).ToString());
            else
                this.ThisVisit = DateTime.Now;
            this.TimeSinceLastLoad = this.ThisVisit.Subtract(this.LastVisit);
        }

        public TimeSpan TimeSinceLastLoad { get; private set; }
        public DateTime ThisVisit { get; private set; }
        public DateTime LastVisit { get; private set; }
    }
}
