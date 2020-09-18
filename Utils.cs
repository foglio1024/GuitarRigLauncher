using System;
using System.Text;
using Nostrum.WinAPI;

namespace GuitarRigLauncher
{
    public static class Utils
    {
        public static string GetWindowTitle(IntPtr hwnd)
        {
            var sb = new StringBuilder(256);
            User32.GetWindowText(hwnd, sb, 256);
            return sb.ToString();
        }

        public static void MakeWindowUnfocusable(IntPtr win)
        {
            var newStyle = User32.GetWindowLong(win, (int)User32.GWL.GWL_EXSTYLE) | (int)User32.ExtendedWindowStyles.WS_EX_NOACTIVATE;
            User32.SetWindowLong(win, (int)User32.GWL.GWL_EXSTYLE, newStyle);
        }

        public static void MakeWindowFocusable(IntPtr win)
        {
            var newStyle = User32.GetWindowLong(win, (int)User32.GWL.GWL_EXSTYLE) & ~(uint)User32.ExtendedWindowStyles.WS_EX_NOACTIVATE;
            User32.SetWindowLong(win, (int)User32.GWL.GWL_EXSTYLE, newStyle);
        }
    }
}