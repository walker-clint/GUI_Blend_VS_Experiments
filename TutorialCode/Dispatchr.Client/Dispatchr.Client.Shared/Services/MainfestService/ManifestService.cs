using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;
using System.Diagnostics;

namespace Dispatchr.Client.Services
{
    public class ManifestService : IManifestService
    {
        public ManifestService()
        {
            this.Helper = new ManifestHelper();
        }

        public string SplashImage
        {
            get { return this.Helper.App.SplashImage; }
        }

        public Color SplashBackgroundColor
        {
            get { return this.Helper.App.SplashBackgroundColor; }
        }

        public ManifestHelper Helper { get; set; }
    }
}
