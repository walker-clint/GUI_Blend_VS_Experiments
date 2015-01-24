using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage.Pickers;

namespace Dispatchr.Client.ViewModels
{
    public partial class MainPageViewModel 
    {
        partial void OnNavigatedToPartial(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
        }

        partial void OnNavigatedFromPartial(Dictionary<string, object> viewModelState, bool suspending)
        {
        }

        private void PickFileContinuation(object e)
        {
            var args = e as FileOpenPickerContinuationEventArgs;
            if (!args.Files.Any())
                return;
            var file = args.Files.First();
            // TODO: handle file
        }

        partial void PickFileCommandExecute()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" },
            };
            picker.ContinuationData["continuationType"] = this.GetType().ToString();
            picker.PickSingleFileAndContinue();
        }

        DelegateCommand _OpenSettingsCommand = null;
        public DelegateCommand OpenSettingsCommand
        {
            get
            {
                if (_OpenSettingsCommand != null)
                    return _OpenSettingsCommand;
                _OpenSettingsCommand = new DelegateCommand
                (
                    () =>
                    {
                        _navigationService.Navigate(Services.Experiences.Settings);
                    },
                    () => true
                );
                this.PropertyChanged += (s, e) => _OpenSettingsCommand.RaiseCanExecuteChanged();
                return _OpenSettingsCommand;
            }
        }
    }
}
