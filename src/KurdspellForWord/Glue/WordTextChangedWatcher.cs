using System;
using System.Windows.Forms;
using KurdspellForWord;
using KurdspellForWord.Glue;

internal class WordTextChangedEventArgs : EventArgs
{
    public WordTextChangedEventArgs(Keys key, int position, int end)
    {
        Key = key;
        Start = position;
        End = end;
        KeysConverter kc = new KeysConverter();
        Text = kc.ConvertToString(key);
    }

    public Keys Key { get; }
    public string Text { get; set; }
    public int Start { get; }
    public int End { get; }
}

// https://stackoverflow.com/a/33897595/7003797
// https://gist.github.com/Lunchbox4K/291f9c8a2501170221d11d29d1355ee1
class WindowsKeyboardHook
{
    // NOTE: We need a backing field to prevent the delegate being garbage collected
    private readonly SafeNativeMethods.HookProc _keyboardProc;
    private readonly IntPtr _hookIdKeyboard;

    private bool _disposed;

    public WindowsKeyboardHook()
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

    public event EventHandler<WordTextChangedEventArgs> KeyPressed;

    private IntPtr KeyboardHookCallback(int nCode, int wParam, IntPtr lParam)
    {
        if (nCode >= 0 && nCode != 3 && wParam != 162 && wParam != 160)
        {
            Keys pressedKey = (Keys)wParam;
            var range = Globals.ThisAddIn.Application.Selection.Range;

            KeyPressed?.Invoke(this, new WordTextChangedEventArgs(pressedKey, range.Start, range.End));
        }

        return SafeNativeMethods.CallNextHookEx(
            _hookIdKeyboard,
            nCode,
            wParam,
            lParam);
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

    ~WindowsKeyboardHook()
    {
        Dispose(false);
    }
}