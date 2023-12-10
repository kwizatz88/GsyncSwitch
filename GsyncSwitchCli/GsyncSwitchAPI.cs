using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace GsyncSwitchCli
{


    public static class GsyncSwitchAPI
    {
        #if DEBUG
                const string GsyncSwitchNVAPI_DLL = @"..\..\..\..\x64\Debug\GsyncSwitchNVAPI.dll";
        #else
        const string GsyncSwitchNVAPI_DLL = @"GsyncSwitchNVAPI.dll";
        #endif


        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchGsync(bool doSwitch);

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperEnableGsync();

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperDisableGsync();

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchVsync(bool doSwitch);

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchFrameLimiter(bool doSwitch, int maxFPS);

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchHDR(bool doSwitch);
    }
}
