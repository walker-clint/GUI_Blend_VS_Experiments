using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Dispatchr.Client.ServiceProxies;
using Dispatchr.Client.Services;
using Dispatchr.Client.Services.SolarizrSqlLiteService;
using Dispatchr.Client.ViewModels;
using LocalSQLite.Repositories;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Dispatchr.Client
{
    public sealed partial class App : MvvmAppBase
    {
        public static readonly IUnityContainer Container = new UnityContainer();
        private IAdalService _adalService;

        void DebugThis(string text = null, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine("{0} {1}", caller, text);
        }

        public App()
        {
            DebugThis("Shared");

            InitializeComponent();
            UnhandledException += App_UnhandledException;

            // some setup is necessary per-platform
            PartialConstructor();
        }

        public static CoreDispatcher Disptacher { get; private set; }
        private static ILaunchActivatedEventArgs OriginalLaunchActivatedEventArgs { get; set; }
        partial void PartialConstructor();

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            DebugThis(args.Kind.ToString());

            // dispatcher ref
            // this is used, in part, by model base which does not have an injection option
            // as a result it has to reach out to App.Dispatcher to raise property changed on the UI thread
            Disptacher = Window.Current.Dispatcher;

            // services
            var settings = new Services.Settings();
            Container.RegisterInstance<ISettings>(settings);
            var launch = new LaunchTimeService(settings);
            Container.RegisterInstance<ILaunchTimeService>(launch);
            Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterInstance<IDispatcherService>(new DispatcherService(Window.Current.Dispatcher));
            Container.RegisterInstance(NavigationService);

            Container.RegisterType<ISolarizrSqlLiteService, SolarizrSqlLiteService>(new ContainerControlledLifetimeManager());

            Container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<INotificationHubService, NotificationHubService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IToastService, ToastService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IManifestService, ManifestService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMapService, MapService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IWebApiService, WebApiService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPrimaryTileService, PrimaryTileService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IKeyboardService, KeyboardService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ILocationService, LocationService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<INetworkAvailableService, NetworkAvailableService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IHeaderViewModel, HeaderViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IBlobService, BlobService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISecondaryTileService, SecondaryTileService>(new ContainerControlledLifetimeManager());

            _adalService = new AdalService(settings.AdTenant, AdalClientId);
            _adalService.AfterLogin += Adal_AfterLogin;
            _adalService.AfterLogout += Adal_AfterLogout;
            Container.RegisterInstance<IAdalService>(_adalService);

#if WINDOWS_APP

            // windows
            Container.RegisterType<ISettingsContractService, SettingsContractService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPrintContractService, PrintContractService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISearchContractService, SearchContractService>(new ContainerControlledLifetimeManager());
#endif

            // repositories
            Container.RegisterType<IAppointmentServiceProxy, AppointmentServiceProxy>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUserServiceProxy, UserServiceProxy>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAppointmentSQLiteRepository, AppointmentSQLiteRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IStatusSQLiteRepository, StatusSQLiteRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPhotoSQLiteRepository, PhotoSQLiteRepository>(new ContainerControlledLifetimeManager());

            // register view models
            Container.RegisterType<IAppointmentItemViewModel, AppointmentItemViewModel>(new PerResolveLifetimeManager());

            // handle launch time & clear cache if prev activation was too far
            if (launch.ShouldClearSessionState)
                SessionStateService.SessionState.Clear();

            // the end
            return Task.FromResult<object>(null);
        }

        protected override object Resolve(Type type)
        {
            DebugThis(type.ToString());

            // this is important; it indicates how to create objects
            try
            {
                return Container.Resolve(type);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        private async Task Setup(ILaunchActivatedEventArgs args)
        {
            DebugThis(args.Kind.ToString());

            // services used in this method
            var adalService = Container.Resolve<IAdalService>();
            var settingsService = Container.Resolve<ISettings>();

            // persist launch args (static)
            OriginalLaunchActivatedEventArgs = args;

            // must auth: attempt silent login
            // silent login in allows us to use a cached auth token
            bool silentLogin = await adalService.SilentLogin(settingsService.ServiceResourceName);

            //
            if (!silentLogin)
            {
                // if not logged in, then login
                // at this point the extended splash screen will be destroyed
                var navigationService = Container.Resolve<INavigationService>();
                navigationService.Navigate(Experiences.Login);
            }
        }

        private async void Adal_AfterLogin(object sender, AfterLoginEventArgs args)
        {
            DebugThis();

            if (!args.LogonSuccessful)
            {
                // we only proceed if the login is successful
                return;
            }

            var adalService = Container.Resolve<IAdalService>();
            var dialogService = Container.Resolve<IDialogService>();
            var resourceLoader = Container.Resolve<IResourceLoader>();
            var navigationService = Container.Resolve<INavigationService>();
            var notificationHubService = Container.Resolve<INotificationHubService>();

            // get rid of the login from the history
            if (args.LogonType == LoginTypes.Interactive)
                navigationService.ClearHistory();

            // register/re-register notification hub
            try
            {
                string tag = notificationHubService.GenerateUserTag(adalService.UserInfo.DisplayableId);
                await notificationHubService.RegisterAsync(tag);
            }
            catch
            {
                string content = resourceLoader.GetString("NotificationHub-Exception-Content");
                string title = resourceLoader.GetString("NotificationHub-Exception-Title");
                dialogService.Show(content, title);
            }

            // then, exec long-running something
            // if the user had to login, then the extended splash screen is no longer visible
            {
                //
                // note: if the user had to login then the splash screen does not reveal
            }

            // normal launch
            switch (OriginalLaunchActivatedEventArgs.Kind)
            {
#if WINDOWS_PHONE_APP
                case ActivationKind.WebAuthenticationBrokerContinuation:
                // this will fall-through and process like launch, because it is post-login/continuation
#endif
                case ActivationKind.Launch:
                    {
                        ILaunchActivatedEventArgs e = OriginalLaunchActivatedEventArgs;
                        if (e.TileId.Equals("App") && e.Arguments == null)
                        {
                            // activated by primary tile
                            navigationService.Navigate(Experiences.Main, e.Arguments);
                        }
                        else if (e.TileId.Equals("App") && e.Arguments != null)
                        {
                            // activated by notification toast
                            navigationService.Navigate(Experiences.Main, e.Arguments);
                        }
                        else
                        {
                            // activated by secondary tile
                            navigationService.Navigate(Experiences.Appointment, e.Arguments);
                        }
                        break;
                    }

                // this is for demonstration-only
                case ActivationKind.Protocol:
                    {
                        navigationService.Navigate(Experiences.Main);
                        break;
                    }

                default:
                    {
                        Window.Current.Activate();
                        break;
                    }
            }
        }

        private void Adal_AfterLogout(object sender, EventArgs e)
        {
            DebugThis();

            var navigationService = Container.Resolve<INavigationService>();
            navigationService.ClearHistory();
            navigationService.Navigate(Experiences.Login, null);
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs arg)
        {
            DebugThis(arg.Exception.ToString());

            var resourceLoader = Container.Resolve<IResourceLoader>();
            var dialogService = Container.Resolve<IDialogService>();

            // do not let the application exit unexpectedly
            arg.Handled = true;

            if (arg.Exception is WebApiService.WebApiException)
            {
                var ex = arg.Exception as WebApiService.WebApiException;
                switch (ex.Status)
                {
                    case HttpStatusCode.Unauthorized:
                        {
                            // unauthorized exception
                            string content = resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-Unauthorized-Content");
                            string title = resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-Unauthorized-Title");
                            string loginCommand = resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-Unauthorized-LoginCommand");
                            string exitCommand = resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-Unauthorized-ExitCommand");
                            var login = new UICommand(loginCommand, e => { _adalService.Logout(); });
                            var exit = new UICommand(exitCommand, e => { Current.Exit(); });
                            dialogService.Show(content, title, login, exit);
                            break;
                        }
                    default:
                        {
                            // terminal exceptio
                            string content = resourceLoader.GetString("TerminalException-Content");
                            string title = resourceLoader.GetString("TerminalException-Title");
                            string exitCommand = resourceLoader.GetString("TerminalException-ExitCommand");
                            var exit = new UICommand(exitCommand, e => { Current.Exit(); });
                            dialogService.Show(content, title, exit);
                            break;
                        }
                }
            }
            else
            {
                // terminal exception
                string content = resourceLoader.GetString("TerminalException-Content");
                string title = resourceLoader.GetString("TerminalException-Title");
                string exitCommand = resourceLoader.GetString("TerminalException-ExitCommand");
                var exit = new UICommand(exitCommand, e => { Current.Exit(); });
                dialogService.Show(content, title, exit);
            }
        }
    }
}