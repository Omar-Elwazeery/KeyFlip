using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace KeyFlip;

public static class HotkeyFormat
{
    public static string Describe(HotkeyModifiers m, Keys k)
    {
        var parts = new List<string>();
        if (m.HasFlag(HotkeyModifiers.Control)) parts.Add("Ctrl");
        if (m.HasFlag(HotkeyModifiers.Alt)) parts.Add("Alt");
        if (m.HasFlag(HotkeyModifiers.Shift)) parts.Add("Shift");
        if (m.HasFlag(HotkeyModifiers.Win)) parts.Add("Win");
        parts.Add(k.ToString());
        return string.Join("+", parts);
    }
}

/// <summary>Saved in %AppData%\KeyFlip\settings.json</summary>
public sealed class AppSettings
{
    public int Modifiers { get; set; } = (int)(HotkeyModifiers.Control | HotkeyModifiers.Alt);
    public int Key { get; set; } = (int)Keys.Space;
    public bool Enabled { get; set; } = true;

    [JsonIgnore] public HotkeyModifiers Mods => (HotkeyModifiers)Modifiers;
    [JsonIgnore] public Keys KeyCode => (Keys)Key;

    private static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KeyFlip", "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
        }
        catch { /* corrupt file: fall back to defaults */ }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { /* settings are a convenience; never crash the app */ }
    }
}
