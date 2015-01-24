using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using Dispatchr.Client.Common;
using Dispatchr.Client.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace Dispatchr.Client.ViewModels
{
    public class LoginPageViewModel : ViewModel, ILoginPageViewModel
    {
        private readonly IAdalService _adalService;
        private readonly ISettings _settings;
        private DelegateCommand _loginCommand;
        private bool _displayLoginButton = true;

        public LoginPageViewModel(IAdalService adalService, ISettings settings)
        {
            _adalService = adalService;
            _adalService.AfterLogin += (sender, args) => UpdateDisplay();
            _settings = settings;
        }

        public DelegateCommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new DelegateCommand(
                        // after login is successful, it is handled by app.xaml.cs
                        // because login is a result of this page for Windows
                        // and login is a result of Adal.Continuation for Phone
                        () =>
                        {
                            _adalService.Login(_settings.ServiceResourceName);
                            DisplayLoginButton = false;
                        },
                        () => !_adalService.IsLoggedIn);
                }
                return _loginCommand;
            }
        }

        public bool DisplayLoginButton
        {
            get { return _displayLoginButton; }
            set { SetProperty(ref _displayLoginButton, value); }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode,
            Dictionary<string, object> viewModelState)
        {
            UpdateDisplay();
            if (_adalService.IsLoggedIn)
            {
                _adalService.RaiseAfterLogin();
            }
        }

        private void UpdateDisplay()
        {
            DisplayLoginButton = !_adalService.IsLoggedIn;
            LoginCommand.RaiseCanExecuteChanged();
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
        }
    }
}