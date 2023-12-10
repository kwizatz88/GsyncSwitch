using System;using System.Collections.Generic;using System.Linq;using System.Runtime.InteropServices;using System.Text;using System.Threading.Tasks;namespace GsyncSwitchCli{


    /// <summary>
    /// The GsyncSwitchAPI class provides a set of static methods to control G-Sync, V-Sync, frame limiter, and HDR settings via NVAPI.
    /// </summary>

    public static class GsyncSwitchAPI    {
#if DEBUG        const string GsyncSwitchNVAPI_DLL = @"..\..\..\..\x64\Debug\GsyncSwitchNVAPI.dll";
#else        const string GsyncSwitchNVAPI_DLL = @"GsyncSwitchNVAPI.dll";
#endif
        /// <summary>
        /// Switches the G-Sync state using the NVAPI. If G-Sync is currently enabled, it will be disabled, and vice versa.
        /// </summary>
        /// <param name="doSwitch">A boolean value indicating whether to switch the G-Sync state.</param>
        /// <returns>An integer representing the new state of G-Sync after the switch. 1 if G-Sync is enabled, 0 otherwise.</returns>
        /// <remarks>
        /// The function is declared with DllImport attribute, indicating that it is implemented in an external DLL.
        /// The actual implementation of the function is expected to be in the 'GsyncSwitchNVAPI.dll' library.
        /// The CallingConvention is set to Cdecl, which specifies the calling convention for the unmanaged function.
        /// </remarks>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]        public static extern int NVAPIWrapperSwitchGsync(bool doSwitch);

        /// <summary>
        /// Enables G-Sync using the NVAPIWrapper.
        /// </summary>
        /// <returns>An integer indicating the result of the operation.</returns>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]        public static extern int NVAPIWrapperEnableGsync();

        /// <summary>
        /// Disables G-Sync using the NVAPIWrapper.
        /// </summary>
        /// <returns>An integer indicating the result of the operation.</returns>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]        public static extern int NVAPIWrapperDisableGsync();

        /// <summary>
        /// P/Invoke wrapper for NVAPI function to switch Vsync state.
        /// </summary>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]        public static extern int NVAPIWrapperSwitchVsync(bool doSwitch);

        /// <summary>
        /// Switches the frame limiter on or off and sets the maximum FPS if switching on.
        /// </summary>
        /// <param name="doSwitch">A boolean value indicating whether to switch the frame limiter on or off.</param>
        /// <param name="maxFPS">An integer specifying the maximum frames per second to limit to when enabling the frame limiter.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]        public static extern int NVAPIWrapperSwitchFrameLimiter(bool doSwitch, int maxFPS);

        /// <summary>
        /// P/Invoke wrapper for the NVAPI function to switch HDR on or off.
        /// </summary>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]        public static extern int NVAPIWrapperSwitchHDR(bool doSwitch);    }}