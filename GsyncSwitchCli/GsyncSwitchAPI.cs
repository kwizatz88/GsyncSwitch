﻿using System;


    /// <summary>
    /// The GsyncSwitchAPI class provides a set of static methods to control G-Sync, V-Sync, frame limiter, and HDR settings via NVAPI.
    /// </summary>

    public static class GsyncSwitchAPI
#if DEBUG
#else
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
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]

        /// <summary>
        /// Enables G-Sync using the NVAPIWrapper.
        /// </summary>
        /// <returns>An integer indicating the result of the operation.</returns>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]

        /// <summary>
        /// Disables G-Sync using the NVAPIWrapper.
        /// </summary>
        /// <returns>An integer indicating the result of the operation.</returns>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]

        /// <summary>
        /// P/Invoke wrapper for NVAPI function to switch Vsync state.
        /// </summary>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]

        /// <summary>
        /// Switches the frame limiter on or off and sets the maximum FPS if switching on.
        /// </summary>
        /// <param name="doSwitch">A boolean value indicating whether to switch the frame limiter on or off.</param>
        /// <param name="maxFPS">An integer specifying the maximum frames per second to limit to when enabling the frame limiter.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]

        /// <summary>
        /// P/Invoke wrapper for the NVAPI function to switch HDR on or off.
        /// </summary>
        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]