using System;
using System.Windows.Forms;

namespace KurdspellForWord.Glue
{
    internal class KeyboardHookEventArgs : EventArgs
    {
        public KeyboardHookEventArgs(Keys key, int position, int activeDocumentId)
        {
            Key = key;
            Position = position;
            KeysConverter kc = new KeysConverter();
            Text = kc.ConvertToString(key);
            ActiveDocumentId = activeDocumentId;
        }

        public Keys Key { get; }
        public string Text { get; set; }
        public int Position { get; }
        public int ActiveDocumentId { get; }
    }

    // https://stackoverflow.com/a/33897595/7003797
    // https://gist.github.com/Lunchbox4K/291f9c8a2501170221d11d29d1355ee1
    class KeyboardHook
    {
        // NOTE: We need a backing field to prevent the delegate being garbage collected
        private readonly SafeNativeMethods.HookProc _keyboardProc;
        private readonly IntPtr _hookIdKeyboard;

        private bool _disposed;

        public KeyboardHook()
        {
            _keyboardProc = KeyboardHookCallback;

            uint threadId = (uint)SafeNativeMethods.GetCurrentThreadId();

            _hookIdKeyboard =
                SafeNativeMethods.SetWindowsHookEx(
                    (int)SafeNativeMethods.HookType.WH_KEYBOARD,
                    _keyboardProc,
                    IntPtr.Zero,
                    threadId);
        }

        public void ThisAddIn_Shutdown()
        {
            UnhookWindowsHooks();
        }

        private void UnhookWindowsHooks()
        {
            SafeNativeMethods.UnhookWindowsHookEx(_hookIdKeyboard);
        }

        public event EventHandler<KeyboardHookEventArgs> KeyPressed;

        private IntPtr KeyboardHookCallback(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && nCode != 3 && wParam != 162 && wParam != 160)
            {
                Keys pressedKey = (Keys)wParam;
                var range = Globals.ThisAddIn.Application.Selection.Range;

                OnKeyPressed(pressedKey, range.Start, range.Document.DocID);
            }

            return SafeNativeMethods.CallNextHookEx(
                _hookIdKeyboard,
                nCode,
                wParam,
                lParam);
        }

        // https://stackoverflow.com/a/1916241/7003797
        private void OnKeyPressed(Keys key, int position, int documentId)
        {
            if (KeyPressed != null)
            {
                var eventListeners = KeyPressed.GetInvocationList();

                for (int index = 0; index < eventListeners.Length; index++)
                {
                    var methodToInvoke = (EventHandler<KeyboardHookEventArgs>)eventListeners[index];

                    methodToInvoke.BeginInvoke(
                        this,
                        new KeyboardHookEventArgs(key, position, documentId),
                        EndOnKeyPressed,
                        null
                        );
                }
            }
        }

        private void EndOnKeyPressed(IAsyncResult iar)
        {
            var ar = (System.Runtime.Remoting.Messaging.AsyncResult)iar;
            var invokedMethod = (EventHandler<KeyboardHookEventArgs>)ar.AsyncDelegate;

            try
            {
                invokedMethod.EndInvoke(iar);
            }
            catch
            {
                // Handle any exceptions that were thrown by the invoked method
                Console.WriteLine("An event listener went kaboom!");
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_hookIdKeyboard != IntPtr.Zero)
                {
                    UnhookWindowsHooks();
                }

                _disposed = true;
            }
        }

        ~KeyboardHook()
        {
            Dispose(false);
        }
    }
}