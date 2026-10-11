using Microsoft.Win32;

namespace KeyFlip;

/// <summary>Optional "start with Windows". Uses the per-user Run key: no administrator rights needed.</summary>
public static class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "KeyFlip";

    public static bool IsEnabled()
    {
        using var k = Registry.CurrentUser.OpenSubKey(RunKey);
        return k?.GetValue(ValueName) != null;
    }

    public static void Set(bool enable)
    {
        using var k = Registry.CurrentUser.OpenSubKey(RunKey, writable: true)
                      ?? Registry.CurrentUser.CreateSubKey(RunKey);
        if (enable)
        {
            string path = Environment.ProcessPath
                          ?? throw new InvalidOperationException("Cannot find the path of KeyFlip.exe.");
            k.SetValue(ValueName, "\"" + path + "\"");
        }
        else
        {
            k.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }
}
