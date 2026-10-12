using System.Windows.Forms;
using KeyFlip;
using Xunit;

public class HotkeyRulesTests
{
    [Fact] public void DefaultShortcut_IsAllowed() =>
        Assert.Null(HotkeyRules.Problem(new AppSettings().Mods, new AppSettings().KeyCode));

    [Theory]
    [InlineData(HotkeyModifiers.Control, Keys.C)]              // KeyFlip's own copy
    [InlineData(HotkeyModifiers.Control, Keys.V)]              // KeyFlip's own paste
    [InlineData(HotkeyModifiers.Control, Keys.Z)]
    [InlineData(HotkeyModifiers.Alt, Keys.F4)]
    [InlineData(HotkeyModifiers.Alt, Keys.Tab)]
    [InlineData(HotkeyModifiers.Shift, Keys.K)]                // needs Ctrl, Alt or Win
    [InlineData(HotkeyModifiers.Control, Keys.ControlKey)]     // modifier only
    [InlineData(HotkeyModifiers.Control | HotkeyModifiers.Alt, Keys.None)]
    public void Rejected(HotkeyModifiers mods, Keys key) => Assert.NotNull(HotkeyRules.Problem(mods, key));

    [Theory]
    [InlineData(HotkeyModifiers.Control | HotkeyModifiers.Shift, Keys.C)]
    [InlineData(HotkeyModifiers.Control | HotkeyModifiers.Alt, Keys.K)]
    [InlineData(HotkeyModifiers.Win | HotkeyModifiers.Alt, Keys.Space)]
    [InlineData(HotkeyModifiers.Control, Keys.Q)]
    public void Allowed(HotkeyModifiers mods, Keys key) => Assert.Null(HotkeyRules.Problem(mods, key));

    [Fact] public void Describe_IncludesWin() =>
        Assert.Equal("Alt+Win+K", HotkeyFormat.Describe(HotkeyModifiers.Win | HotkeyModifiers.Alt, Keys.K));
}
