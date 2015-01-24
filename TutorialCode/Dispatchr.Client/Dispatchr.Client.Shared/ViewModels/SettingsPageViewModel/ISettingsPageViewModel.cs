using Dispatchr.Client.Common;
using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.ViewModels
{
    public interface ISettingsPageViewModel
    {
        bool LocalOnly { get; set; }
        bool IsLoggedIn { get; set; }
        DelegateCommand LogoutCommand { get; }
        DelegateCommand ClearLocalDbCommand { get; }
    }
}