using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;

namespace Dispatchr.Client.Services
{
    public interface IManifestService 
    {
        string SplashImage { get; }
        Color SplashBackgroundColor { get; }
    }
}