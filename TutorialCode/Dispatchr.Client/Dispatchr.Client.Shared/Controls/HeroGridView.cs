using Dispatchr.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Dispatchr.Client.Controls
{
    public class HeroGridView : GridView
    {
        public Size HeroSize { get; set; }
        public Size NormalSize { get; set; }

        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            try
            {
                var size = new Size(1, 1);
                switch ((item as Common.IVariableSizedItem).VariableItemSize)
                {
                    case VariableItemSizes.Hero:
                        size = HeroSize;
                        break;
                    case VariableItemSizes.Normal:
                        size = NormalSize;
                        break;
                }
                element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, size.Width);
                element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.RowSpanProperty, size.Height);
            }
            finally
            {
                base.PrepareContainerForItemOverride(element, item);
            }
        }
    }
}
