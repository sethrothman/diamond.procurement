// FormBoundsPersistence.cs
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace Diamond.Procurement.Win.Helpers;

public static class FormBoundsPersistence
{
    public static void Attach(Form form, string? key = null)
    {
        if (form is null) throw new ArgumentNullException(nameof(form));
        key ??= form.Name ?? form.GetType().Name;

        form.Load += (_, __) => RestoreNow(form, key);
        form.FormClosing += (_, __) => SaveNow(form, key);
    }

    public static void RestoreNow(Form form, string key)
    {
        var s = new FormBoundsSettings(key);
        TryUpgrade(s);

        form.StartPosition = FormStartPosition.Manual;

        // Restore size
        var sz = s.MainFormSize;
        if (sz.Width > 0 && sz.Height > 0)
            form.Size = sz;

        // Restore location only if it's on a visible screen
        var loc = s.MainFormLocation;
        if (loc.X != 0 || loc.Y != 0)
        {
            var rect = new Rectangle(loc, form.Size);
            if (IsOnAnyScreen(rect))
            {
                form.Location = loc;
            }
            else
            {
                CenterOnCurrentScreen(form); // fallback
            }
        }
        else
        {
            CenterOnCurrentScreen(form); // first-run default
        }

        // Restore window state (avoid starting minimized)
        var ws = s.MainFormWindowState;
        if (ws == FormWindowState.Minimized) ws = FormWindowState.Normal;
        form.WindowState = ws;
    }

    public static void SaveNow(Form form, string key)
    {
        var s = new FormBoundsSettings(key);

        if (form.WindowState == FormWindowState.Normal)
        {
            s.MainFormLocation = form.Location;
            s.MainFormSize = form.Size;
        }
        else if (form.WindowState == FormWindowState.Maximized)
        {
            s.MainFormLocation = form.RestoreBounds.Location;
            s.MainFormSize = form.RestoreBounds.Size;
        }

        s.MainFormWindowState = form.WindowState;
        s.Save();
    }

    private static bool IsOnAnyScreen(Rectangle rect)
    {
        foreach (var sc in Screen.AllScreens)
            if (sc.WorkingArea.IntersectsWith(rect))
                return true;
        return false;
    }

    // Public CenterToScreen() is protected on Form; do it ourselves.
    private static void CenterOnCurrentScreen(Form form)
    {
        var wa = Screen.FromPoint(Cursor.Position).WorkingArea;

        var x = wa.Left + (wa.Width - form.Width) / 2;
        var y = wa.Top + (wa.Height - form.Height) / 2;

        // Clamp to working area in case the form is larger than the screen
        x = Math.Max(wa.Left, Math.Min(x, wa.Right - form.Width));
        y = Math.Max(wa.Top, Math.Min(y, wa.Bottom - form.Height));

        form.Location = new Point(x, y);
    }

    private static void TryUpgrade(ApplicationSettingsBase s)
    {
        try
        {
            var upgraded = (bool?)s["__Upgraded"] ?? false;
            if (!upgraded)
            {
                s.Upgrade();
                s["__Upgraded"] = true;
                s.Save();
            }
        }
        catch { /* ignore */ }
    }

    private sealed class FormBoundsSettings : ApplicationSettingsBase
    {
        public FormBoundsSettings(string key) : base(key) => SettingsKey = key;

        [UserScopedSetting, DefaultSettingValue("0,0")]
        public Point MainFormLocation
        {
            get => this[nameof(MainFormLocation)] is Point p ? p : new Point(0, 0);
            set => this[nameof(MainFormLocation)] = value;
        }

        [UserScopedSetting, DefaultSettingValue("0,0")]
        public Size MainFormSize
        {
            get => this[nameof(MainFormSize)] is Size s ? s : new Size(0, 0);
            set => this[nameof(MainFormSize)] = value;
        }

        [UserScopedSetting, DefaultSettingValue("Normal")]
        public FormWindowState MainFormWindowState
        {
            get => this[nameof(MainFormWindowState)] is FormWindowState ws ? ws : FormWindowState.Normal;
            set => this[nameof(MainFormWindowState)] = value;
        }

        [UserScopedSetting, DefaultSettingValue("False")]
        public bool __Upgraded
        {
            get => this[nameof(__Upgraded)] is bool b && b;
            set => this[nameof(__Upgraded)] = value;
        }
    }
}
