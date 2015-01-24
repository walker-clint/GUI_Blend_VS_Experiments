using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispatchr.Common;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace System
{
    public static partial class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> newItems )
        {
            foreach (var newItem in newItems)
            {
                collection.Add(newItem);
            }
        }

        public static void Refresh<T>(this ObservableCollection<T> collection)
        {
            var original = collection.ToArray();
            collection.Clear();
            foreach (var item in original)
                collection.Add(item);
        }

        public static BitmapImage ToBitmapImage(this Uri value)
        {
            return new BitmapImage(value);
        }

        public static ImageSource ToImageSource(this Uri value)
        {
            return value.ToBitmapImage() as ImageSource;
        }

        public async static Task Save(this ImageSource image,
            StorageFile file,
            BitmapPixelFormat format = BitmapPixelFormat.Bgra8,
            BitmapAlphaMode alpha = BitmapAlphaMode.Ignore,
            float? dpi = null)
        {
            var bitmap = image as WriteableBitmap;
            // write bitmap to file
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var pixels = bitmap.PixelBuffer;
                // using System.Runtime.InteropServices.WindowsRuntime;
                var bytes = pixels.ToArray();
                var encoder = await BitmapEncoder
                    .CreateAsync(BitmapEncoder.PngEncoderId, stream);
                var width = (uint)bitmap.PixelWidth;
                var height = (uint)bitmap.PixelHeight;
                dpi = dpi ?? DisplayInformation.GetForCurrentView().LogicalDpi;
                encoder.SetPixelData(format, alpha, width, height, dpi.Value, dpi.Value, bytes);
                await encoder.FlushAsync();
                stream.Seek(0);
            }

            // finalize
            await FileIO.WriteBytesAsync(file, null);
        }

        public async static Task Render(this UIElement element,
            StorageFile file,
            BitmapPixelFormat format = BitmapPixelFormat.Bgra8,
            BitmapAlphaMode alpha = BitmapAlphaMode.Ignore,
            float? dpi = null)
        {
            // create bitmap
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(element);

            // write bitmap to file
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var pixels = await bitmap.GetPixelsAsync();
                // using System.Runtime.InteropServices.WindowsRuntime;
                var bytes = pixels.ToArray();
                var encoder = await BitmapEncoder
                    .CreateAsync(BitmapEncoder.PngEncoderId, stream);
                var width = (uint)bitmap.PixelWidth;
                var height = (uint)bitmap.PixelHeight;
                dpi = dpi ?? DisplayInformation.GetForCurrentView().LogicalDpi;
                encoder.SetPixelData(format, alpha, width, height, dpi.Value, dpi.Value, bytes);
                await encoder.FlushAsync();
                stream.Seek(0);
            }

            // finalize
            await FileIO.WriteBytesAsync(file, null);
        }
    }
}

