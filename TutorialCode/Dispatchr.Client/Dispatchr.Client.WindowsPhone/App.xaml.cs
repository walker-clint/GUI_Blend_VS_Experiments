using Dispatchr.Client.Services;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Dispatchr.Client
{
    public sealed partial class App : Microsoft.Practices.Prism.Mvvm.MvvmAppBase
    {
        readonly string AdalClientId = "2a3f14f2-4cc1-4e15-a182-76c29c45d9d8";

        async partial void PartialConstructor()
        {
            DebugThis();

            // setup cortana
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommandDefinition.xml"));
            await VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(file);
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            DebugThis();

            // because onapplaunch is not called in this case
            // we manually complete the standard pipeline
            await base.InitializeFrameAsync(args);
            var events = Container.Resolve<IEventAggregator>();

            var myargs = args as IActivatedEventArgs as ILaunchActivatedEventArgs;

            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                // continuation will trigger afterlogin event
                var adalService = App.Container.Resolve<IAdalService>();
                adalService.Continuation(args as IActivatedEventArgs as IWebAuthenticationBrokerContinuationEventArgs);
            }
            else if (args.Kind == ActivationKind.PickFileContinuation)
            {
                var e = args as IActivatedEventArgs as FileOpenPickerContinuationEventArgs;
                var data = e.ContinuationData["continuationType"].ToString();
                if (data.Equals(typeof(ViewModels.AppointmentPageViewModel).ToString()))
                    events.GetEvent<Messages.AddPhotoContinuation>().Publish(e);
                else
                    throw new NotImplementedException("Unexpected continuation. Data: " + data);
            }
            else if (myargs != null)
            {
                // this will most-likely be custom protocol
                await Setup(args as IActivatedEventArgs as ILaunchActivatedEventArgs);
            }

        }

        // The OnLaunchApp method gets called if the app is NOT restoring from termination. 
        // It is called when the app is launched from the start screen and NOT from termination.
        protected async override System.Threading.Tasks.Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            DebugThis(args.Kind.ToString());

            await Setup(args);

            var events = Container.Resolve<IEventAggregator>();
            switch (args.Kind)
            {
                case ActivationKind.VoiceCommand:
                    {
                        // cortana
                        var e = args as IActivatedEventArgs as VoiceCommandActivatedEventArgs;
                        break;
                    }
            }
        }

        protected override void OnHardwareButtonsBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            DebugThis();

            // don't do this if we call the base - goes back twice!
            //if (NavigationService.CanGoBack())
            //{
            //    NavigationService.GoBack();
            //    e.Handled = true;
            //}
            base.OnHardwareButtonsBackPressed(sender, e);
        }
    }
}
