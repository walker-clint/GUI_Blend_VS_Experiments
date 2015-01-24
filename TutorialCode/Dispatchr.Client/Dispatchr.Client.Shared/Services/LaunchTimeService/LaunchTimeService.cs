using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    class LaunchTimeService : ILaunchTimeService
    {
        private ISettings _settings;
        public LaunchTimeService(ISettings settings)
        {
            this._settings = settings;
            this.Helper = new LaunchTimeHelper();
        }

        public bool ShouldClearSessionState { get { return this.Helper.TimeSinceLastLoad.TotalDays > _settings.DaysForState; } }

        public LaunchTimeHelper Helper { get; set; }
    }
}
