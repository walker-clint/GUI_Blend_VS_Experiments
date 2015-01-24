using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public enum Experiences { Main, Login, Appointment, Transfer, Settings, About, Privacy }

    public interface INavigationService
    {
        bool Navigate(Experiences experience, object param = null);
        void GoBack();
        bool CanGoBack { get; }
        void ClearHistory();
    }

    public class NavigationService : INavigationService
    {
        public NavigationService(Microsoft.Practices.Prism.Mvvm.Interfaces.INavigationService navigationService)
        {
            this._navigationService = navigationService;
        }

        public bool Navigate(Experiences experience, object param = null)
        {
            return _navigationService.Navigate(experience.ToString(), param);
        }

        public void ClearHistory()
        {
            _navigationService.ClearHistory();
        }

        public bool CanGoBack { get { return _navigationService.CanGoBack(); } }

        public void GoBack()
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();
        }

        public Microsoft.Practices.Prism.Mvvm.Interfaces.INavigationService _navigationService { get; private set; }
    }
}
