using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Notepad_RT
{
    class Program
    {
        const int SCRX = 80, SCRY = 60;
        static char[,] screen;
        static List<IntPtr> GetWindowHandles()
        {
            List<IntPtr> windowHandles = new List<IntPtr>();

            foreach (Process window in Process.GetProcesses())
            {
                window.Refresh();

                if (window.MainWindowHandle != IntPtr.Zero)
                {
                    if (window.ProcessName.ToLower() == "notepad")
                    {
                        windowHandles.Add(window.MainWindowHandle);
                    }
                }
            }
            return windowHandles;
        }

        static void CLS()
        {
            for (int y = 0; y < SCRY; y++)
            {
                for (int x = 0; x < SCRX; x++)
                {
                    screen[x, y] = '.';
                }
            }
        }
        static void plot(int x, int y)
        {
            if ((x >= 0) && (x < SCRX) && (y >= 0) && (y < SCRY))
            {
                screen[x, y] = '@';
            }
        }
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        static void send(IntPtr wh)
        {
            // Build string for screen
            var sb = new StringBuilder("", 2*SCRX*SCRY);
            for (int y=0; y < SCRY; y++)
            {
                for (int x = 0; x < SCRX; x++)
                {
                    sb.Append(screen[x, y]);
                }
                sb.Append('\n');
            }
            /* This isn't very quick.. 
            // Copy to clipboard
            Clipboard.SetText(sb.ToString());
            // Ctrl-A, Ctrl-V to Notepad
            SendKeys.SendWait("^A");
            SendKeys.SendWait("^V");
            */
            SendMessage(wh, 0x000C, 0, sb.ToString());
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [STAThread]
        static void Main(string[] args)
        {
            var wh = GetWindowHandles();
            if (wh.Count > 1)
            {
                Console.WriteLine("Multiple Notepad windows open");
                return;
            }

            // Find Notepad edit box handle
            IntPtr np = FindWindowEx(wh[0], IntPtr.Zero, "RichEditD2DPT", null);

            // Bring Notepad window to foreground - not needed when using WM_SETTEXT directly
            // SetForegroundWindow(wh[0]);

            // Elapsed time (in frames)
            int t = 0;
            // Character array for "screen"
            screen = new char[SCRX, SCRY];

            while (true)
            {
                CLS();
                var r = (t % 60);
                // Draw something
                for (int i = 1; i < r; i++)
                {
                    plot(SCRX / 2 - i, SCRY / 2 - (r - i));
                    plot(SCRX / 2 + i, SCRY / 2 - (r - i));
                    plot(SCRX / 2 - i, SCRY / 2 + (r - i));
                    plot(SCRX / 2 + i, SCRY / 2 + (r - i));
                }
                // Send frame
                send(np);

                // Next frame
                t++;
            }
        }
    }
}
