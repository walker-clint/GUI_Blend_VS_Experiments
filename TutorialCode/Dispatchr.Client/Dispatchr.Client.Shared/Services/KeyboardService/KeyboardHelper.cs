﻿using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Dispatchr.Client.Services
{
    public class KeyboardHelper
    {
        public KeyboardHelper()
        {
            Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher
                .AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
            Windows.UI.Xaml.Window.Current.CoreWindow
                .PointerPressed += this.CoreWindow_PointerPressed;
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
           AcceleratorKeyEventArgs e)
        {
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown))
            {
                var coreWindow = Windows.UI.Xaml.Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                var virtualKey = e.VirtualKey;
                bool altKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;

                // raise keydown actions
                var keyDown = new KeyDownEventArgs
                {
                    AltKey = altKey,
                    Character = ToChar(virtualKey, shiftKey),
                    ControlKey = controlKey,
                    EventArgs = e,
                    ShiftKey = shiftKey,
                    VirtualKey = virtualKey
                };
                foreach (var item in this.KeyDown.ToArray())
                {
                    try { item.Value(keyDown); }
                    catch { this.KeyDown.Remove(item.Key); }
                }

                // Only investigate further when Left, Right, or the dedicated Previous or Next keys
                // are pressed
                if (virtualKey == VirtualKey.Left
                    || virtualKey == VirtualKey.Right
                    || (int)virtualKey == 166
                    || (int)virtualKey == 167
                    || (int)virtualKey == 69)
                {
                    bool noModifiers = !altKey && !controlKey && !shiftKey;
                    bool onlyAlt = altKey && !controlKey && !shiftKey;

                    if (((int)virtualKey == 166 && noModifiers)
                        || (virtualKey == VirtualKey.Left && onlyAlt))
                    {
                        // When the previous key or Alt+Left are pressed navigate back
                        e.Handled = true;
                        RaiseGoBackGestured();
                    }
                    else if (((int)virtualKey == 167 && noModifiers)
                        || (virtualKey == VirtualKey.Right && onlyAlt))
                    {
                        // When the next key or Alt+Right are pressed navigate forward
                        e.Handled = true;
                        RaiseGoForwardGestured();
                    }
                    else if (((int)virtualKey == 69 && controlKey))
                    {
                        // when control-E
                        e.Handled = true;
                        RaiseControlEGestured();
                    }
                }
            }
        }

        public class KeyDownEventArgs : EventArgs
        {
            public bool AltKey { get; set; }
            public bool ControlKey { get; set; }
            public bool ShiftKey { get; set; }
            public VirtualKey VirtualKey { get; set; }
            public AcceleratorKeyEventArgs EventArgs { get; set; }
            public char? Character { get; set; }
        }
        private Dictionary<Type, Action<KeyDownEventArgs>> _KeyDown = new Dictionary<Type, Action<KeyDownEventArgs>>();
        public Dictionary<Type, Action<KeyDownEventArgs>> KeyDown { get { return _KeyDown; } }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        /// page is active and occupies the entire window.  Used to detect browser-style next and
        /// previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) RaiseGoBackGestured();
                if (forwardPressed) RaiseGoForwardGestured();
            }
        }

        public Action GoForwardGestured { get; set; }
        protected void RaiseGoForwardGestured()
        {
            if (GoForwardGestured != null)
                GoForwardGestured();
        }

        public Action GoBackGestured { get; set; }
        protected void RaiseGoBackGestured()
        {
            if (GoBackGestured != null)
                GoBackGestured();
        }

        public Action ControlEGestured { get; set; }
        protected void RaiseControlEGestured()
        {
            if (ControlEGestured != null)
                ControlEGestured();
        }

        private static char? ToChar(VirtualKey key, bool shift)
        {
            // convert virtual key to char
            if (32 == (int)key)
                return ' ';

            VirtualKey search;

            // look for simple letter
            foreach (var letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            {
                if (Enum.TryParse<VirtualKey>(letter.ToString(), out search) && search.Equals(key))
                    return (shift) ? letter : letter.ToString().ToLower()[0];
            }

            // look for simple number
            foreach (var number in "1234567890")
            {
                if (Enum.TryParse<VirtualKey>("Number" + number.ToString(), out search) && search.Equals(key))
                    return number;
            }

            // not found
            return null;
        }
    }

    enum VKeyClass
    {
        Control, // 0-31, 33-47, 91-95, 144-165
        Character, // 32, 48-90
        NumPad, // 96-111
        Function // 112 - 135
    }

    public enum VKeyCharacterClass
    {
        Space,
        Numeric,
        Alphabetic
    }

}
