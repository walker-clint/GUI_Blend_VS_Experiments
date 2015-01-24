using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Dispatchr.Client.Common;
using Dispatchr.Client.Models;

namespace Dispatchr.Client.Controls
{
    public class HeroTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeroTemplate { get; set; }
        public DataTemplate NormalTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch ((item as Common.IVariableSizedItem).VariableItemSize)
            {
                case VariableItemSizes.Hero:
                    return this.HeroTemplate;
                case VariableItemSizes.Normal:
                    return this.NormalTemplate;
            }
            throw new NotImplementedException();
        }
    }
}
