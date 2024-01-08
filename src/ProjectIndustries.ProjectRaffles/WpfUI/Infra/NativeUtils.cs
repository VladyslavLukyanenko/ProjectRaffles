using System;
using System.Runtime.InteropServices;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Infra
{
    internal static class NativeUtils
    {
        internal static uint TPM_LEFTALIGN;

        internal static uint TPM_RETURNCMD;

        static NativeUtils()
        {
            NativeUtils.TPM_LEFTALIGN = 0;
            NativeUtils.TPM_RETURNCMD = 256;
        }

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);
    }
}
