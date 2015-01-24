using System.Collections.Generic;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;

namespace Dispatchr.Client
{
    public sealed partial class App : Microsoft.Practices.Prism.Mvvm.MvvmAppBase
    {
        readonly string AdalClientId = "3b2cc3e8-2fbb-4a72-8eef-1bc1cace8da3";

        partial void PartialConstructor()
        {
            DebugThis("Windows");

            // extended splash is invoked in InitializeFrameAsync, so must setup now
            this.ExtendedSplashScreenFactory = s => new Views.Splash(s, new Services.ManifestService());
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            DebugThis("Windows");

            // because onapplaunch is not called in this case
            // we manually complete the standard pipeline
            await base.InitializeFrameAsync(args);

            // this will most-likely be custom protocol
            await Setup(args as ILaunchActivatedEventArgs);
        }

        // The OnLaunchApp method gets called if the app is NOT restoring from termination. 
        // It is called when the app is launched from the start screen and NOT from termination.
        protected async override System.Threading.Tasks.Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            DebugThis(args.Kind.ToString());

            await Setup(args);
        }

        protected override System.Collections.Generic.IList<Windows.UI.ApplicationSettings.SettingsCommand> GetSettingsCommands()
        {
            var settings = new SettingsCommand(Guid.NewGuid(), "Settings", (e) => new Views.SettingsPage().Show());
            var about = new SettingsCommand(Guid.NewGuid(), "About", (e) => new Views.AboutPage().Show());
            var privacy = new SettingsCommand(Guid.NewGuid(), "Privacy", (e) => new Views.PrivacyPage().Show());
            return (new List<SettingsCommand> { settings, about, privacy });
        }
    }
}
