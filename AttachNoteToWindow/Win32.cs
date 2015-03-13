using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AttachNoteToWindow
{
    static class Win32
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        //  int GetWindowText(
        //      __in   HWND hWnd,
        //      __out  LPTSTR lpString,
        //      __in   int nMaxCount
        //  );
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        //  DWORD GetWindowThreadProcessId(
        //      __in   HWND hWnd,
        //      __out  LPDWORD lpdwProcessId
        //  );
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static string GetTopWindowText()
        {
            IntPtr hWnd = GetForegroundWindow();
            int length = GetWindowTextLength(hWnd);
            StringBuilder text = new StringBuilder(length + 1);
            GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }

        public static string GetTopWindowName()
        {
            IntPtr hWnd = GetForegroundWindow();
            uint lpdwProcessId;
            GetWindowThreadProcessId(hWnd, out lpdwProcessId);

            var p = Process.GetProcessById((int)lpdwProcessId);
            return p.ProcessName;
        }

        [DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public static Rectangle GetTopWindowBounds()
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT rct;

            GetWindowRect(hWnd, out rct);

            Rectangle bounds = new Rectangle();
            bounds.X = rct.Left;
            bounds.Y = rct.Top;
            bounds.Width = rct.Right - rct.Left + 1;
            bounds.Height = rct.Bottom - rct.Top + 1;

            return bounds;
        }

    }
}
