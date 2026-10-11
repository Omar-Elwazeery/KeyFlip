using System.Drawing;
using System.Windows.Forms;

namespace KeyFlip;

public sealed class SettingsForm : Form
{
    private readonly TextBox _hotkeyBox = new();
    private readonly Label _hint = new();
    private readonly CheckBox _startupBox = new();
    private HotkeyModifiers _mods;
    private Keys _key;

    public HotkeyModifiers SelectedModifiers => _mods;
    public Keys SelectedKey => _key;
    public bool RunAtStartup => _startupBox.Checked;

    public SettingsForm(HotkeyModifiers mods, Keys key, bool runAtStartup)
    {
        _mods = mods;
        _key = key;

        Text = "KeyFlip Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(380, 200);

        var label = new Label
        {
            Text = "Shortcut: click the box, then press the new keys",
            Left = 16, Top = 16, Width = 350, Height = 20
        };

        _hotkeyBox.Left = 16;
        _hotkeyBox.Top = 40;
        _hotkeyBox.Width = 346;
        _hotkeyBox.ReadOnly = true;
        _hotkeyBox.BackColor = SystemColors.Window;
        _hotkeyBox.Text = HotkeyFormat.Describe(_mods, _key);

        _hint.Left = 16; _hint.Top = 68; _hint.Width = 350; _hint.Height = 20;
        _hint.Text = "Use Ctrl or Alt together with another key.";
        _hint.ForeColor = SystemColors.GrayText;

        _startupBox.Left = 16; _startupBox.Top = 100; _startupBox.Width = 350;
        _startupBox.Text = "Start KeyFlip when I sign in to Windows";
        _startupBox.Checked = runAtStartup;

        var save = new Button { Text = "Save", Left = 190, Top = 150, Width = 80, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Cancel", Left = 282, Top = 150, Width = 80, DialogResult = DialogResult.Cancel };
        AcceptButton = save;
        CancelButton = cancel;

        Controls.AddRange(new Control[] { label, _hotkeyBox, _hint, _startupBox, save, cancel });
    }

    // Captures the key combination while the shortcut box has focus.
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (!_hotkeyBox.Focused) return base.ProcessCmdKey(ref msg, keyData);

        var mods = HotkeyModifiers.None;
        if ((keyData & Keys.Control) != 0) mods |= HotkeyModifiers.Control;
        if ((keyData & Keys.Alt) != 0) mods |= HotkeyModifiers.Alt;
        if ((keyData & Keys.Shift) != 0) mods |= HotkeyModifiers.Shift;
        Keys code = keyData & Keys.KeyCode;

        // No modifier: let Tab / Enter / Esc behave normally so the dialog stays usable.
        if (mods == HotkeyModifiers.None) return base.ProcessCmdKey(ref msg, keyData);

        // Only modifier keys pressed so far: wait for the real key.
        if (code is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin) return true;

        if ((mods & (HotkeyModifiers.Control | HotkeyModifiers.Alt)) == 0)
        {
            _hint.Text = "Please include Ctrl or Alt.";
            return true;
        }

        _mods = mods;
        _key = code;
        _hotkeyBox.Text = HotkeyFormat.Describe(_mods, _key);
        _hint.Text = "Use Ctrl or Alt together with another key.";
        return true;
    }
}
