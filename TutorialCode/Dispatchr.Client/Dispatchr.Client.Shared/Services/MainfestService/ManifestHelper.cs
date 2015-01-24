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
    public class ManifestHelper 
    {
        public ManifestHelper()
        {
            var manifest = Manifest;
        }

        private XDocument _Manifest = default(XDocument);
        private XDocument Manifest
        {
            get
            {
                if (_Manifest == null)
                    _Manifest = XDocument.Load("AppxManifest.xml", LoadOptions.None);
                return _Manifest;
            }
        }

        private AppModel _App;
        public AppModel App
        {
            get
            {
                if (_App == null)
                {
                    // find app node
                    var root = XNamespace.Get("http://schemas.microsoft.com/appx/2010/manifest");
                    var m2 = XNamespace.Get("http://schemas.microsoft.com/appx/2013/manifest");
#if WINDOWS_PHONE_APP
                    var m3 = XNamespace.Get("http://schemas.microsoft.com/appx/2014/manifest");
#endif
                    var package = this.Manifest.Document;
                    var apps = package.Descendants(root + "Applications").First();
                    var app = apps.Descendants(root + "Application").First();
#if WINDOWS_APP
                    var visual = app.Descendants(m2 + "VisualElements").First();
#elif WINDOWS_PHONE_APP
                    var visual = app.Descendants(m3 + "VisualElements").First();
#endif

                    // app name
                    string name = default(string);
                    try { name = visual.Attribute("DisplayName").Value; }
                    catch { }

                    // app capabilities
                    Capabilities[] array = new Capabilities[] { };
                    try
                    {
                        var capabilities = package.Descendants(root + "Capabilities").First();
                        var capability = capabilities.Descendants(root + "Capability");
                        array = capability
                            .Select(x => x.Attribute("Name").Value.Replace(".", "_"))
                            .Select(x =>
                            {
                                Capabilities enumeration;
                                if (!Enum.TryParse<Capabilities>(x.ToString(), out enumeration))
                                {
                                    // this indicates an unsupported capability
                                    Debugger.Break();
                                    return Capabilities.Unsupported;
                                }
                                return enumeration;
                            }).ToArray();
                    }
                    catch { }

                    // app protocols
                    string[] protocols = new string[] { };
                    try
                    {
                        var parent = app.Descendants(root + "Extensions").First();
                        var extens = parent.Descendants(root + "Extension");
                        var filter = extens.Where(x => x.Attribute("Category").Value.Equals("windows.protocol"));
                        var protos = filter.SelectMany(x => x.Descendants(root + "Protocol"));
                        protocols = protos.Select(x => x.Attribute("Name").Value).ToArray();
                    }
                    catch { }

                    // find splash node
#if WINDOWS_APP
                    var splash = visual
                        .Descendants(m2 + "SplashScreen").First();
#elif WINDOWS_PHONE_APP
                    var splash = visual
                        .Descendants(m3 + "SplashScreen").First();
#endif

                    // splash path
                    string splashImage = default(string);
                    try { splashImage = splash.Attribute("Image").Value; }
                    catch { }

                    // splash color
                    Color splashBackgroundColor = default(Color);
                    try
                    {
                        var hex = splash.Attribute("BackgroundColor").Value;

                        var property = typeof(Colors).GetRuntimeProperties().FirstOrDefault(x => x.Name.Equals(hex, StringComparison.CurrentCultureIgnoreCase));
                        if (property != null)
                        {
                            // get from actual property
                            splashBackgroundColor = (Color)property.GetValue(null);
                        }
                        else
                        {
                            // parse hex
                            splashBackgroundColor = Windows.UI.Color.FromArgb(255,
                                Convert.ToByte(hex.Substring(1, 2), 16),
                                Convert.ToByte(hex.Substring(3, 2), 16),
                                Convert.ToByte(hex.Substring(5, 2), 16)
                            );
                        }
                    }
                    catch { }

                    // setup
                    _App = new AppModel
                    {
                        DisplayName = name,
                        Capabilities = array,
                        Protocols = protocols,
                        SplashImage = splashImage,
                        SplashBackgroundColor = splashBackgroundColor,
                    };
                }
                return _App;
            }
        }

        public enum Capabilities { musicLibrary, picturesLibrary, videosLibrary, removableStorage, internetClient, internetClientServer, privateNetworkClientServer, location, microphone, proximity, webcam, usb, humaninterfacedevice, bluetooth_genericAttributeProfile, bluetooth_rfcomm, pointOfService, enterpriseAuthentication, sharedUserCertificates, documentsLibrary, Unsupported}

        public class AppModel
        {
            public string DisplayName { get; set; }
            public string[] Protocols { get; set; }
            public Capabilities[] Capabilities { get; set; }
            public string SplashImage { get; set; }
            public Color SplashBackgroundColor { get; set; }
        }
    }
}
