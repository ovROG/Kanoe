using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Kanoe.Services
{
    public partial class AIMPService
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);


        private const int WM_AIMP_COMMAND = 0x0400 + 0x75; // 0x0400 - WM_USER <winuser.h> + WM_AIMP_COMMAND - 0x75
        private const int AIMP_RA_CMD_BASE = 10;

        private const int AIMP_RA_CMD_NEXT = AIMP_RA_CMD_BASE + 7;
        private const int AIMP_RA_CMD_PREV = AIMP_RA_CMD_BASE + 8;

        public unsafe void NextTrack()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr AIMPWindow = FindWindow("AIMP2_RemoteInfo", "AIMP2_RemoteInfo");
                _ = SendMessage(AIMPWindow, WM_AIMP_COMMAND, AIMP_RA_CMD_NEXT, 0);
            }
        }

        public void PrevTrack()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr AIMPWindow = FindWindow("AIMP2_RemoteInfo", "AIMP2_RemoteInfo");
                _ = SendMessage(AIMPWindow, WM_AIMP_COMMAND, AIMP_RA_CMD_PREV, 0);
            }
        }

        public static void ChangeTrackHandel(int s)
        {
            Console.WriteLine(s.ToString() + "wpasdasd");
        }
    }
}
