using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FordToolbox
{
    public class GlobalHotKey : IDisposable
    {
        /// <summary>
        /// Registers a global hotkey
        /// </summary>
        /// <param name="aKeyGesture">e.g. Alt + Shift + Control + Win + S</param>
        /// <param name="aAction">Action to be called when hotkey is pressed</param>
        /// <returns>true, if registration succeeded, otherwise false</returns>
        public static bool RegisterHotKey(string aKeyGestureString, Action aAction)
        {
            var c = new KeyGestureConverter();
            KeyGesture aKeyGesture = (KeyGesture)c.ConvertFrom(aKeyGestureString);
            return RegisterHotKey(aKeyGesture.Modifiers, aKeyGesture.Key, aAction);
        }

        public static bool RegisterHotKey(ModifierKeys aModifier, Key aKey, Action aAction)
        {
            if (aModifier == ModifierKeys.None)
            {
                throw new ArgumentException("Modifier must not be ModifierKeys.None");
            }
            if (aAction is null)
            {
                throw new ArgumentNullException(nameof(aAction));
            }

            System.Windows.Forms.Keys aVirtualKeyCode = (System.Windows.Forms.Keys)KeyInterop.VirtualKeyFromKey(aKey);
            _currentId = _currentId + 1;
            bool aRegistered = RegisterHotKey(Window.Handle, _currentId,
                                        (uint)aModifier | _modNorepeat,
                                        (uint)aVirtualKeyCode);

            if (aRegistered)
            {
                _registeredHotKeys.Add(new HotKeyWithAction(aModifier, aKey, aAction));
            }
            return aRegistered;
        }

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(Window.Handle, i);
            }

            // dispose the inner native window.
            Window.Dispose();
        }

        static GlobalHotKey()
        {
            Window.KeyPressed += (s, e) =>
            {
                _registeredHotKeys.ForEach(x =>
                {
                    if (e.Modifier == x.Modifier && e.Key == x.Key)
                    {
                        x.Action();
                    }
                });
            };
        }

        private static readonly InvisibleWindowForMessages Window = new InvisibleWindowForMessages();
        private static int _currentId;
        private static uint _modNorepeat = 0x4000;
        private static List<HotKeyWithAction> _registeredHotKeys = new List<HotKeyWithAction>();

        private class HotKeyWithAction
        {

            public HotKeyWithAction(ModifierKeys modifier, Key key, Action action)
            {
                Modifier = modifier;
                Key = key;
                Action = action;
            }

            public ModifierKeys Modifier { get; }
            public Key Key { get; }
            public Action Action { get; }
        }

        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private class InvisibleWindowForMessages : System.Windows.Forms.NativeWindow, IDisposable
        {
            public InvisibleWindowForMessages()
            {
                CreateHandle(new System.Windows.Forms.CreateParams());
            }

            private static int _wmHotkey = 0x0312;
            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == _wmHotkey)
                {
                    var aWpfKey = KeyInterop.KeyFromVirtualKey(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
                    if (KeyPressed != null)
                    {
                        KeyPressed(this, new HotKeyPressedEventArgs(modifier, aWpfKey));
                    }
                }
            }

            public class HotKeyPressedEventArgs : EventArgs
            {
                private ModifierKeys _modifier;
                private Key _key;

                internal HotKeyPressedEventArgs(ModifierKeys modifier, Key key)
                {
                    _modifier = modifier;
                    _key = key;
                }

                public ModifierKeys Modifier
                {
                    get { return _modifier; }
                }

                public Key Key
                {
                    get { return _key; }
                }
            }


            public event EventHandler<HotKeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }
    }
}
