using System.Runtime.InteropServices;

namespace Kanoe.Services
{
    public partial class AIMPService
    {
        private const int WM_AIMP_COMMAND = 0x0400 + 0x75; // 0x0400 - WM_USER <winuser.h> + WM_AIMP_COMMAND - 0x75
        private const int AIMP_RA_CMD_BASE = 10;

        private const int AIMP_RA_CMD_NEXT = AIMP_RA_CMD_BASE + 7;
        private const int AIMP_RA_CMD_PREV = AIMP_RA_CMD_BASE + 8;

        readonly NativeOSMethodsService nativeOSMethodsService;

        public AIMPService(NativeOSMethodsService _nativeOSMethodsService)
        {
            nativeOSMethodsService = _nativeOSMethodsService;
        }

        public void NextTrack()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr AIMPWindow = nativeOSMethodsService.FindNativeWindow("AIMP2_RemoteInfo", "AIMP2_RemoteInfo");
                _ = nativeOSMethodsService.SendNativeMessage(AIMPWindow, WM_AIMP_COMMAND, AIMP_RA_CMD_NEXT, 0);
            }
        }

        public void PrevTrack()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr AIMPWindow = nativeOSMethodsService.FindNativeWindow("AIMP2_RemoteInfo", "AIMP2_RemoteInfo");
                _ = nativeOSMethodsService.SendNativeMessage(AIMPWindow, WM_AIMP_COMMAND, AIMP_RA_CMD_PREV, 0);
            }
        }
    }
}
