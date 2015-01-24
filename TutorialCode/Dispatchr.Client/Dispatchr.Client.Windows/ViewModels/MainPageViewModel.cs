using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Dispatchr.Client.ViewModels
{
    public partial class MainPageViewModel : ViewModel, IMainPageViewModel
    {
        private const double HeroSize = 500;

        partial void OnNavigatedToPartial(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState) { }
        partial void OnNavigatedFromPartial(Dictionary<string, object> viewModelState, bool suspending) { }

        async partial void PickFileCommandExecute()
        {
            // http://msdn.microsoft.com/en-us/library/windows/apps/windows.storage.pickers.fileopenpicker.aspx
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" },
            };
            var file = await picker.PickSingleFileAsync();
            if (file == null)
                return;
            // TODO: handle file
        }
    }

    public class FilePickerHelper
    {
        async Task<StorageFile> PickAsync() {

            // http://msdn.microsoft.com/en-us/library/windows/apps/windows.storage.pickers.fileopenpicker.aspx
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" },
            };
            return await picker.PickSingleFileAsync();
        }
    }
}
