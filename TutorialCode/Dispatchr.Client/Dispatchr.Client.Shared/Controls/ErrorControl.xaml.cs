using Dispatchr.Client.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Dispatchr.Client.Models;

namespace Dispatchr.Client.Controls
{
    public sealed partial class ErrorControl : UserControl
    {
        public ErrorControl()
        {
            this.InitializeComponent();
#if WINDOWS_APP
            this.PointerEntered += (s, e) => { Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1); };
            this.PointerExited += (s, e) =>{Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);};
#endif
            this.Loaded += (s, e) => { SetupPlacement(); };
        }

        public IModel Model
        {
            get { return (IModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(IModel), typeof(ErrorControl), new PropertyMetadata(null, ModelChanged));
        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = e.NewValue as IModel;
            var dictionary = model.Properties;
            var control = d as ErrorControl;
            var property = dictionary[control.PropertyName];
            control.LeftButton.DataContext = property;
            control.RightButton.DataContext = property;
        }

        public string PropertyName { get; set; }

        public UIElement InnerContent
        {
            get { return (UIElement)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(UIElement),
            typeof(ErrorControl), new PropertyMetadata(null, InnerContentChanged));
        private static void InnerContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ErrorControl).MyPresenter.Content = e.NewValue as UIElement;
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool),
            typeof(ErrorControl), new PropertyMetadata(false, IsReadOnlyChanged));
        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ErrorControl;
            var u = c.MyPresenter.Content as Control;
            u.IsEnabled = (bool)e.NewValue;
        }

        public enum Placements { Left, Right }

        public Placements Placement
        {
            get { return (Placements)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(Placements),
            typeof(ErrorControl), new PropertyMetadata(Placements.Right, PlacementChanged));
        private static void PlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ErrorControl;
            c.SetupPlacement();
        }
        private void SetupPlacement()
        {
            switch (this.Placement)
            {
                case Placements.Left:
                    VisualStateManager.GoToState(this, "LeftVisualState", false);
                    break;
                case Placements.Right:
                    VisualStateManager.GoToState(this, "RightVisualState", false);
                    break;
            }
        }
    }
}
