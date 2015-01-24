using Windows.UI.Core;

namespace Dispatchr.Client.Services
{
    using System;
    using Windows.ApplicationModel.Activation;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.Threading.Tasks;

    public enum LoginTypes { Interactive, Silent, Unknown }

    public class AfterLoginEventArgs : EventArgs
    {
        public LoginTypes LogonType { get; set; }
        public UserInfo UserInfo { get; set; }
        public bool LogonSuccessful { get; set; }
    }

    public interface IAdalService
    {

#if WINDOWS_PHONE_APP

        void Login(string resource = "https://graph.windows.net");
        void Continuation(IWebAuthenticationBrokerContinuationEventArgs args);

#elif WINDOWS_APP

        // we don't use the Async suffix to keep Phone/Windows interface the same
        Task<AuthenticationResult> Login(string resource = "https://graph.windows.net");

#endif

        void Logout();
        Task<bool> SilentLogin(string resource = "https://graph.windows.net");

        string Tenant { get; }
        bool IsLoggedIn { get; }
        string ClientId { get; }
        Uri RedirectUri { get; }
        string Authority { get; }
        UserInfo UserInfo { get; }
        LoginTypes LoginType { get; }
        AuthenticationContext Context { get; }

        string AuthHeader { get; }
        string AccessToken { get; }

        event EventHandler<AfterLoginEventArgs> AfterLogin;
        event EventHandler AfterLogout;
        event EventHandler IsLoggedInChanged;

        void RaiseAfterLogin();
        void RaiseAfterLogout();
        void RaiseIsLoggedInChanged();
    }

    public class AdalService : IAdalService
    {
        private string _authHeader;
        private string _accessToken;
        // phone: https://github.com/AzureADSamples/NativeClient-WindowsPhone8.1
        // windows: https://github.com/AzureADSamples/NativeClient-WindowsStore

        /// <param name="tenant">For example "{your_domain}.onmicrosoft.com"</param>
        /// <param name="clientId">This is aquired from the portal, when you register an app the portal generates this ID</param>
        public AdalService(string tenant, string clientId)
        {
            this.Tenant = tenant;
            this.ClientId = clientId;

            Init();
        }

        public string Tenant { get; protected set; }
        public string ClientId { get; protected set; }
        public AuthenticationContext Context { get; protected set; }
        public bool IsLoggedIn { get { return this.UserInfo != null; } }
        public Microsoft.IdentityModel.Clients.ActiveDirectory.UserInfo UserInfo { get; protected set; }
        public string Authority { get { return String.Format(System.Globalization.CultureInfo.InvariantCulture, "https://login.windows.net/{0}", this.Tenant); } }
        public Uri RedirectUri { get { return Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri(); } }

        public LoginTypes LoginType { get; set; }

        public string AuthHeader
        {
            get { return _authHeader; }
        }

        public string AccessToken
        {
            get { return _accessToken; }
        }

        public event EventHandler<AfterLoginEventArgs> AfterLogin;
        public void RaiseAfterLogin()
        {
            if (AfterLogin != null)
                AfterLogin(this, new AfterLoginEventArgs
                {
                    LogonType = this.LoginType,
                    UserInfo = this.UserInfo,
                    LogonSuccessful = IsLoggedIn
                });
        }

        public event EventHandler AfterLogout;
        public void RaiseAfterLogout()
        {
            if (AfterLogout != null)
                AfterLogout(this, EventArgs.Empty);
        }

        public event EventHandler IsLoggedInChanged;
        public void RaiseIsLoggedInChanged()
        {
            if (IsLoggedInChanged != null)
                IsLoggedInChanged(this, EventArgs.Empty);
        }

        // 1. this is called after Login(resource)
        // 2. this is also called after Continuation(args)
        void Login(AuthenticationResult result)
        {
            this.UserInfo = result.UserInfo;
            if (result.Status == AuthenticationStatus.Success)
            {
                RaiseIsLoggedInChanged();

                // create an authentication header for use in web api calls
                _authHeader = result.CreateAuthorizationHeader();
                _accessToken = result.AccessToken;
            }

            RaiseAfterLogin();
        }

        public void Logout()
        {
            this.Context.TokenCache.Clear();
            this.UserInfo = null;
            RaiseAfterLogout();
            RaiseIsLoggedInChanged();
        }

        public async Task<bool> SilentLogin(string resource = "https://graph.windows.net")
        {
            if (Context == null)
                throw new NullReferenceException("Context is null");
            var result = await Context.AcquireTokenSilentAsync(resource.ToString(), ClientId);
            if (result.Status == AuthenticationStatus.Success)
            {
                Login(result);
            }
            return IsLoggedIn;
        }

#if WINDOWS_PHONE_APP

        void Init()
        {
            // setup (be sure and keep it synchronous)
            Context = AuthenticationContext.CreateAsync(this.Authority).GetResults();
        }

        public async void Login(string resource = "https://graph.windows.net")
        {
            if (Context == null)
                throw new NullReferenceException("Context is null");
            var result = await this.Context.AcquireTokenSilentAsync(resource.ToString(), this.ClientId);
            if (result.Status == AuthenticationStatus.Success)
                Login(result);
            else
                Context.AcquireTokenAndContinue(resource, this.ClientId, this.RedirectUri, this.Login);
        }

        public async void Continuation(IWebAuthenticationBrokerContinuationEventArgs args)
        {
            if (Context == null)
                throw new NullReferenceException("Context is null");
            await Context.ContinueAcquireTokenAsync(args);
        }

#elif WINDOWS_APP

        void Init()
        {
            // setup
            Context = new AuthenticationContext(this.Authority);
        }

        public async Task<AuthenticationResult> Login(string resource = "https://graph.windows.net")
        {
            if (Context == null)
                throw new NullReferenceException("Context is null");
            var result = await Context.AcquireTokenSilentAsync(resource.ToString(), ClientId);
            if (result.Status != AuthenticationStatus.Success)
                result = await Context.AcquireTokenAsync(resource, ClientId, this.RedirectUri);
            Login(result);
            return result;
        }

#endif

    }

}