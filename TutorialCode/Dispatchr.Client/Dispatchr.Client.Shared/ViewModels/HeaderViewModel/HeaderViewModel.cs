using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.ViewModels
{
    public class HeaderViewModel : ViewModel, Dispatchr.Client.ViewModels.IHeaderViewModel
    {
        Services.IBlobService _blobService;
        private Services.IAdalService _adalService;
        Services.INavigationService _navigationService;
        public HeaderViewModel(Services.IAdalService adalService, Services.IBlobService blobService, Services.INavigationService navigationService)
        {
            _adalService = adalService;
            _blobService = blobService;
            _navigationService = navigationService;

            this.FirstName = _adalService.UserInfo.GivenName;
            this.LastName = _adalService.UserInfo.FamilyName;
        }

        public override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            // 
        }

        string _FirstName = default(string);
        public string FirstName { get { return _FirstName; } set { SetProperty(ref _FirstName, value); } }

        string _LastName = default(string);
        public string LastName { get { return _LastName; } set { SetProperty(ref _LastName, value); } }

        DelegateCommand _goBackCommand = null;
        public DelegateCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new DelegateCommand
                        (
                        () => { _navigationService.GoBack(); },
                        () => { return _navigationService.CanGoBack; }
                        );
                }
                return _goBackCommand;
            }
        }

        public bool CanGoBack
        {
            get { return _navigationService.CanGoBack; }
        }

        DelegateCommand _settingsCommand = null;
        public DelegateCommand SettingsCommand
        {
            get
            {
                if (_settingsCommand != null)
                    return _settingsCommand;
                _settingsCommand = new DelegateCommand
                (
                    () =>
                    {
#if WINDOWS_APP
                        new Views.SettingsPage().ShowIndependent();
#elif WINDOWS_PHONE_APP
                        _navigationService.Navigate(Services.Experiences.Settings);
#endif
                    },
                    () => { return true; }
                );
                return _settingsCommand;
            }
        }
    }
}


