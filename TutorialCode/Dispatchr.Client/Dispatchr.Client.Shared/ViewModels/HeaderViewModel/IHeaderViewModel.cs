using System;
namespace Dispatchr.Client.ViewModels
{
    public interface IHeaderViewModel
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand GoBackCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand SettingsCommand { get; }
        bool CanGoBack { get; }
    }
}
