using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace p1eXu5.Wpf.CustomControls.Helpers
{
    internal class WindowHelpers
    {

        [ DllImport( "User32.dll" ) ]
        internal static extern IntPtr MonitorFromWindow( IntPtr hwnd, uint dwFlags );

        [ DllImport( "User32.dll" ) ]
        internal static extern bool GetMonitorInfoA( IntPtr hMonitor, ref MONITORINFO lpmi );

        internal static class MonitorDwFlags
        {
            internal const uint MONITOR_DEFAULTTONULL = 0x00000000;
            internal const uint MONITOR_DEFAULTTOPRIMARY = 0x00000001;
            internal const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

            internal const uint MONITORINFOF_PRIMARY = 0x00000001;
        }

        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout( LayoutKind.Sequential )]
        internal struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }
    }
}
