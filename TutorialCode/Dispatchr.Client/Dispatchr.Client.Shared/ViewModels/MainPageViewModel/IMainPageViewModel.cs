using System;
using System.Collections.ObjectModel;
using Dispatchr.Client.Models;
using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.ViewModels
{
    public interface IMainPageViewModel
    {
        bool Loading { get; }
        ObservableCollection<ViewModels.IAppointmentItemViewModel> Appointments { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand PickFileCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand RefreshCommand { get; }
        ViewModels.IHeaderViewModel HeaderViewModel { get; }
        Models.User User { get; set; }
        DelegateCommand AboutCommand { get;  }
        DelegateCommand PrivacyCommand { get;  }

#if WINDOWS_PHONE_APP
        DelegateCommand OpenSettingsCommand { get;  }
#endif
    }
}
