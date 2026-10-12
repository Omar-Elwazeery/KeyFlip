using Microsoft.Win32;

namespace KeyFlip;

/// <summary>Optional "start with Windows". Uses the per-user Run key: no administrator rights needed.</summary>
public static class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "KeyFlip";

    /// <summary>Passed on the command line when Windows starts KeyFlip at sign-in.</summary>
    public const string StartupArgument = "--startup";

    /// <summary>True only if the startup entry points to this copy of KeyFlip.exe.</summary>
    public static bool IsEnabled()
    {
        using var k = Registry.CurrentUser.OpenSubKey(RunKey);
        return k?.GetValue(ValueName) is string command && IsThisExe(ExePath(command));
    }

    public static void Set(bool enable)
    {
        using var k = Registry.CurrentUser.OpenSubKey(RunKey, writable: true)
                      ?? Registry.CurrentUser.CreateSubKey(RunKey);
        if (enable)
        {
            string path = Environment.ProcessPath
                          ?? throw new InvalidOperationException("Cannot find the path of KeyFlip.exe.");
            k.SetValue(ValueName, Command(path));
        }
        else
        {
            k.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }

    /// <summary>
    /// Keeps an existing startup entry working: points it here if the KeyFlip.exe it names no longer
    /// exists (the file was moved), and adds the startup argument to entries written by older versions.
    /// </summary>
    public static void RefreshEntry()
    {
        using var k = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        if (k?.GetValue(ValueName) is not string command) return;
        string? current = Environment.ProcessPath;
        if (current == null) return;
        string exe = ExePath(command);
        bool moved = !IsThisExe(exe) && !File.Exists(exe);
        if ((moved || IsThisExe(exe)) && command != Command(current))
            k.SetValue(ValueName, Command(current));
    }

    private static string Command(string exePath) => "\"" + exePath + "\" " + StartupArgument;

    // The value is "C:\path\KeyFlip.exe" [arguments], with or without quotes.
    private static string ExePath(string command)
    {
        command = command.Trim();
        if (command.StartsWith('"'))
        {
            int end = command.IndexOf('"', 1);
            return end > 0 ? command[1..end] : command[1..];
        }
        int space = command.IndexOf(' ');
        return space > 0 ? command[..space] : command;
    }

    private static bool IsThisExe(string path)
        => string.Equals(path, Environment.ProcessPath, StringComparison.OrdinalIgnoreCase);
}
