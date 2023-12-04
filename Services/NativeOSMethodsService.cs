using System.Runtime.InteropServices;

namespace Kanoe.Services
{
    public class NativeOSMethodsService //TODO: linux
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public int SendNativeMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return SendMessage(hWnd, wMsg, wParam, lParam);
            }
            return 0;
        }

        public int FindNativeWindow(string lpClassName, string lpWindowName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return FindWindow(lpClassName, lpWindowName);
            }
            return 0;
        }

    }
}
