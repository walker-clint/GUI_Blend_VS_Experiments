using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using LocalSQLite;

namespace Dispatchr.Client.Models
{
    public partial class Photo
    {
        [Ignore]
        public BitmapImage ImageSource { get; set; }

        public async Task CopyAndAssignImageAsync(StorageFile file)
        {
            StorageFile copiedFile = await file.CopyAsync(
                ApplicationData.Current.LocalFolder,
                Id.ToString(),
                NameCollisionOption.ReplaceExisting);

            Path = copiedFile.Path;
            using (IRandomAccessStream fileStream = await copiedFile.OpenAsync(FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap 
                var imageSource = new BitmapImage();
                imageSource.SetSourceAsync(fileStream);
                ImageSource = imageSource;
            }
        }

        public async Task BuildImageSourceFromPath()
        {
            using (
                IRandomAccessStream fileStream =
                    await (await StorageFile.GetFileFromPathAsync(Path)).OpenAsync(FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap 
                var imageSource = new BitmapImage();
                imageSource.SetSourceAsync(fileStream);
                ImageSource = imageSource;
            }
        }


        public async Task DeleteCopyAsync()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(Path);
                await file.DeleteAsync();
            }
            catch
            {
                // we didn't find it... we don't care
            }
        }
    }
}