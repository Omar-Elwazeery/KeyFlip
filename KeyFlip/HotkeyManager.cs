using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyFlip;

[Flags]
public enum HotkeyModifiers : uint
{
    None = 0, Alt = 1, Control = 2, Shift = 4, Win = 8, NoRepeat = 0x4000
}

/// <summary>Registers one global hotkey and raises HotkeyPressed when it is pressed.</summary>
public sealed class HotkeyManager : NativeWindow, IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const int HotkeyId = 1;
    private const int ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public event Action? HotkeyPressed;
    public bool IsRegistered { get; private set; }

    public HotkeyManager()
    {
        // Message-only window: invisible, only receives messages.
        CreateHandle(new CreateParams { Parent = new IntPtr(-3) });
    }

    public bool Register(HotkeyModifiers mods, Keys key, out string error)
    {
        Unregister();
        bool ok = RegisterHotKey(Handle, HotkeyId, (uint)(mods | HotkeyModifiers.NoRepeat), (uint)key);
        if (!ok)
        {
            int code = Marshal.GetLastWin32Error();
            error = code == ERROR_HOTKEY_ALREADY_REGISTERED
                ? "This shortcut is already used by another program."
                : $"Could not register the shortcut (Windows error {code}).";
            return false;
        }
        IsRegistered = true;
        error = "";
        return true;
    }

    public void Unregister()
    {
        if (IsRegistered)
        {
            UnregisterHotKey(Handle, HotkeyId);
            IsRegistered = false;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HotkeyId)
            HotkeyPressed?.Invoke();
        base.WndProc(ref m);
    }

    public void Dispose()
    {
        Unregister();
        if (Handle != IntPtr.Zero) DestroyHandle();
    }
}