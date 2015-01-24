using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dispatchr.Client.Converters
{
    public class VisibleWhenPhoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#if WINDOWS_PHONE_APP
            return Visibility.Visible;
#else
            return Visibility.Collapsed;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
