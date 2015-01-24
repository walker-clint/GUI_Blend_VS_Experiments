using Microsoft.Xaml.Interactivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Dispatchr.Client.Behaviors
{
    [Microsoft.Xaml.Interactivity.TypeConstraint(typeof(Windows.UI.Xaml.Controls.Page))]
    public class KeyNavBehavior : DependencyObject, IBehavior
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Windows.UI.Xaml.DependencyObject AssociatedObject { get; set; }

        Windows.UI.Xaml.Controls.Page _page = default(Windows.UI.Xaml.Controls.Page);
        public void Attach(Windows.UI.Xaml.DependencyObject associatedObject)
        {
            this._page = (this.AssociatedObject = associatedObject) as Windows.UI.Xaml.Controls.Page;
            Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerPressed += this.CoreWindow_PointerPressed;
        }

        public void Detach()
        {
            Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerPressed += this.CoreWindow_PointerPressed;
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;
                if (backPressed && this.AllowGoBack && this._page.Frame.CanGoBack)
                    this._page.Frame.GoBack();
                if (forwardPressed && this.AllowGoForward && this._page.Frame.CanGoForward)
                    this._page.Frame.GoForward();
            }
        }

        private void CoreDispatcher_AcceleratorKeyActivated(Windows.UI.Core.CoreDispatcher sender, Windows.UI.Core.AcceleratorKeyEventArgs args)
        {
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                 args.EventType == CoreAcceleratorKeyEventType.KeyDown))
            {
                var virtualKey = args.VirtualKey;

                // Only investigate further when Left, Right, or the dedicated Previous or Next keys
                // are pressed
                if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                    args.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                    (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                    (int)virtualKey == 166 || (int)virtualKey == 167))
                {
                    var coreWindow = Window.Current.CoreWindow;
                    var downState = CoreVirtualKeyStates.Down;
                    bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                    bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                    bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                    bool noModifiers = !menuKey && !controlKey && !shiftKey;
                    bool onlyAlt = menuKey && !controlKey && !shiftKey;

                    if (((int)virtualKey == 166 && noModifiers) ||
                        (virtualKey == VirtualKey.Left && onlyAlt))
                    {
                        // When the previous key or Alt+Left are pressed navigate back
                        args.Handled = true;
                        if (this.AllowGoBack && this._page.Frame.CanGoBack)
                            this._page.Frame.GoBack();
                    }
                    else if (((int)virtualKey == 167 && noModifiers) ||
                        (virtualKey == VirtualKey.Right && onlyAlt))
                    {
                        // When the next key or Alt+Right are pressed navigate forward
                        args.Handled = true;
                        if (this.AllowGoForward && this._page.Frame.CanGoForward)
                            this._page.Frame.GoForward();
                    }
                }
            }
        }

        public bool AllowGoBack
        {
            get { return (bool)GetValue(AllowGoBackProperty); }
            set { SetValue(AllowGoBackProperty, value); }
        }
        public static readonly DependencyProperty AllowGoBackProperty =
            DependencyProperty.Register("AllowGoBack", typeof(bool), typeof(KeyNavBehavior), new PropertyMetadata(true));

        public bool AllowGoForward
        {
            get { return (bool)GetValue(AllowGoForwardProperty); }
            set { SetValue(AllowGoForwardProperty, value); }
        }
        public static readonly DependencyProperty AllowGoForwardProperty =
            DependencyProperty.Register("AllowGoForward", typeof(bool), typeof(KeyNavBehavior), new PropertyMetadata(true));
    }
}
