/// <summary>
/// The GsyncSwitchNVAPIWrapper class provides a C# wrapper for the NVAPI functions related to G-Sync, V-Sync, Frame Limiter, and HDR settings.
/// It allows toggling these settings on or off and provides helper functions for initializing and finalizing G-Sync settings.
/// </summary>

#define GsyncSwitchNVAPIWrapper _declspec(dllexport)

using namespace std;
#include<string>
#include <Windows.h>
#include <stdlib.h>
#include <stdio.h>
#include "nvapi.h"
#include "NvApiDriverSettings.h"
#include "GsyncSwitchEXE.h"

/*
 This function is used to print to the command line a text message
 describing the nvapi error and quits
*/
void PrintError2(NvAPI_Status status)
{
	NvAPI_ShortString szDesc = { 0 };
	NvAPI_GetErrorMessage(status, szDesc);
	printf(" NVAPI error: %s\n", szDesc);
	//	exit(-1);
}


extern "C" {
	// Forward declarations of helper functions
	void InitializeGsync(NvDRSSessionHandle* hSession, NvDRSProfileHandle* hProfile);
	void FinalizeGsync(NvDRSSessionHandle hSession);
	int SetGsyncState(bool enable);
	int ToggleGsync(bool doSwitch);

	// Original function - toggles G-Sync based on current state
	GsyncSwitchNVAPIWrapper int NVAPIWrapperSwitchGsync(bool doSwitch) {
		int result = ToggleGsync(doSwitch);
		return result;
	}

	// Function to toggle G-Sync based on a flag: true enables, false disables
	int NVAPIWrapperToggleGsync(bool enable) {
		int result = ToggleGsync(enable);
		return result;
	}

	// Function to enable G-Sync
	GsyncSwitchNVAPIWrapper int NVAPIWrapperEnableGsync() {
		int result = SetGsyncState(true);
		return result;
	}

	// Function to disable G-Sync
	GsyncSwitchNVAPIWrapper int NVAPIWrapperDisableGsync() {
		int result = SetGsyncState(false);
		return result;
	}

	// Helper function to set G-Sync state: true enables, false disables
	int SetGsyncState(bool enable) {
		NvDRSSessionHandle hSession = 0;
		NvDRSProfileHandle hProfile = 0;
		InitializeGsync(&hSession, &hProfile);

		NVDRS_SETTING drsSetting = { 0 };
		drsSetting.version = NVDRS_SETTING_VER;
		drsSetting.settingId = VRR_MODE_ID;
		drsSetting.settingType = NVDRS_DWORD_TYPE;
		drsSetting.u32CurrentValue = enable ? VRR_MODE_DEFAULT : VRR_MODE_DISABLED;

		NvAPI_Status status = NvAPI_DRS_SetSetting(hSession, hProfile, &drsSetting);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		status = NvAPI_DRS_SaveSettings(hSession);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		FinalizeGsync(hSession);

		return enable ? 1 : 0;
	}

	// Helper function to toggle G-Sync based on the current state
	int ToggleGsync(bool doSwitch) {
		NvDRSSessionHandle hSession = 0;
		NvDRSProfileHandle hProfile = 0;
		InitializeGsync(&hSession, &hProfile);

		NVDRS_SETTING drsSetting = { 0 };
		drsSetting.version = NVDRS_SETTING_VER;
		drsSetting.settingId = VRR_MODE_ID;
		drsSetting.settingType = NVDRS_DWORD_TYPE;

		NvAPI_Status status = NvAPI_DRS_GetSetting(hSession, hProfile, VRR_MODE_ID, &drsSetting);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		int result = 0;
		if (drsSetting.u32CurrentValue == VRR_MODE_DISABLED && doSwitch) {
			drsSetting.u32CurrentValue = VRR_MODE_DEFAULT;
			result = 1;
		}
		else if (drsSetting.u32CurrentValue != VRR_MODE_DISABLED && doSwitch) {
			drsSetting.u32CurrentValue = VRR_MODE_DISABLED;
			result = 0;
		}

		status = NvAPI_DRS_SetSetting(hSession, hProfile, &drsSetting);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		status = NvAPI_DRS_SaveSettings(hSession);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		FinalizeGsync(hSession);

		return result;
	}

	// Helper function to initialize G-Sync settings
	void InitializeGsync(NvDRSSessionHandle* hSession, NvDRSProfileHandle* hProfile) {
		NvAPI_Status status = NvAPI_Initialize();
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		status = NvAPI_DRS_CreateSession(hSession);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		status = NvAPI_DRS_LoadSettings(*hSession);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}

		status = NvAPI_DRS_GetBaseProfile(*hSession, hProfile);
		if (status != NVAPI_OK) {
			PrintError2(status);
		}
	}

	// Helper function to finalize G-Sync settings
	void FinalizeGsync(NvDRSSessionHandle hSession) {
		NvAPI_DRS_DestroySession(hSession);
		NvAPI_Unload();
	}



	GsyncSwitchNVAPIWrapper int NVAPIWrapperSwitchVsync(bool doSwitch) {
		int result = 0;
		NvAPI_Status status;

		// (0) Initialize NVAPI. This must be done first of all.
		status = NvAPI_Initialize();
		if (status != NVAPI_OK)
			PrintError2(status);

		// (1) Create the session handle to access driver settings.
		NvDRSSessionHandle hSession = 0;
		status = NvAPI_DRS_CreateSession(&hSession);
		if (status != NVAPI_OK)
			PrintError2(status);

		// (2) Load all the system settings into the session.
		status = NvAPI_DRS_LoadSettings(hSession);
		if (status != NVAPI_OK)
			PrintError2(status);

		// (3) Obtain the Base profile. Any setting needs to be inside
		// a profile, putting a setting on the Base Profile enforces it
		// for all the processes on the system.
		NvDRSProfileHandle hProfile = 0;
		status = NvAPI_DRS_GetBaseProfile(hSession, &hProfile);
		if (status != NVAPI_OK)
			PrintError2(status);

		// (4) Specify that we want the VSYNC setting.
		NVDRS_SETTING drsSetting = { 0 };
		drsSetting.version = NVDRS_SETTING_VER;
		drsSetting.settingId = VSYNCMODE_ID;
		drsSetting.settingType = NVDRS_DWORD_TYPE;

		status = NvAPI_DRS_GetSetting(hSession, hProfile, VSYNCMODE_ID, &drsSetting);
		if (status != NVAPI_OK)
			PrintError2(status);

		if (drsSetting.u32CurrentValue == VSYNCMODE_FORCEOFF) {
			if (doSwitch)
				result = 1;
			else
				result = 0;
			drsSetting.u32CurrentValue = VSYNCMODE_FORCEON;
		}
		else {
			if (doSwitch)
				result = 0;
			else
				result = 1;
			drsSetting.u32CurrentValue = VSYNCMODE_FORCEOFF;
		}

		if (doSwitch) {
			status = NvAPI_DRS_SetSetting(hSession, hProfile, &drsSetting);
			if (status != NVAPI_OK)
				PrintError2(status);
			// (5) Now we apply (or save) our changes to the system.
			status = NvAPI_DRS_SaveSettings(hSession);
		}

		if (status != NVAPI_OK)
			PrintError2(status);

		// (6) We clean up. This is analogous to doing a free().
		NvAPI_DRS_DestroySession(hSession);
		hSession = 0;
		NvAPI_Unload();

		return result;
	}

	GsyncSwitchNVAPIWrapper int NVAPIWrapperSwitchFrameLimiter(bool doSwitch, int maxFPS) {

		int result = 0;

		NvAPI_Status status;
		// (0) Initialize NVAPI. This must be done first of all
		status = NvAPI_Initialize();
		if (status != NVAPI_OK)
			PrintError2(status);

		// (1) Create the session handle to access driver settings
		NvDRSSessionHandle hSession = 0;
		status = NvAPI_DRS_CreateSession(&hSession);
		if (status != NVAPI_OK)
			PrintError2(status);

		// (2) load all the system settings into the session
		status = NvAPI_DRS_LoadSettings(hSession);
		if (status != NVAPI_OK)
			PrintError2(status);

		// (3) Obtain the Base profile. Any setting needs to be inside
		// a profile, putting a setting on the Base Profile enforces it
		// for all the processes on the system
		NvDRSProfileHandle hProfile = 0;
		status = NvAPI_DRS_GetBaseProfile(hSession, &hProfile);
		if (status != NVAPI_OK)
			PrintError2(status);

		// (4) Specify that we want the frame limiter setting
		// first we fill the NVDRS_SETTING struct, then we call the function
		NVDRS_SETTING drsSetting = { 0 };
		drsSetting.version = NVDRS_SETTING_VER;
		drsSetting.settingId = FRL_FPS_ID;
		drsSetting.settingType = NVDRS_DWORD_TYPE;

		status = NvAPI_DRS_GetSetting(hSession, hProfile, FRL_FPS_ID, &drsSetting);
		if (status != NVAPI_OK)
			PrintError2(status);

		if (drsSetting.u32CurrentValue == FRL_FPS_DISABLED) {
			if (doSwitch)
				result = 1;
			else
				result = 0;
			drsSetting.u32CurrentValue = maxFPS;
		}
		else {
			if (doSwitch)
				result = 0;
			else
				result = 1;
			drsSetting.u32CurrentValue = FRL_FPS_DISABLED;
		}

		if (doSwitch) {
			status = NvAPI_DRS_SetSetting(hSession, hProfile, &drsSetting);
			if (status != NVAPI_OK)
				PrintError2(status);

			// (5) Now we apply (or save) our changes to the system
			status = NvAPI_DRS_SaveSettings(hSession);
		}

		if (status != NVAPI_OK)
			PrintError2(status);

		// (6) We clean up. This is analogous to doing a free()
		NvAPI_DRS_DestroySession(hSession);
		hSession = 0;
		NvAPI_Unload();

		return result;
	}

	GsyncSwitchNVAPIWrapper int NVAPIWrapperSwitchHDR(bool doSwitch) {

		int result = 0;

		NvAPI_Status status = NvAPI_Initialize();
		if (status != NVAPI_OK)
			PrintError2(status);

		NvU32 gpuCount = 0;
		NvPhysicalGpuHandle ahGPU[NVAPI_MAX_PHYSICAL_GPUS] = {};

		status = NvAPI_EnumPhysicalGPUs(ahGPU, &gpuCount);
		if (status != NVAPI_OK)
			PrintError2(status);

		for (NvU32 i = 0; i < gpuCount; ++i)
		{
			NvU32 displayIdCount = 16;
			NvU32 flags = 0;
			NV_GPU_DISPLAYIDS displayIdArray[16] = {};
			displayIdArray[0].version = NV_GPU_DISPLAYIDS_VER;

			// Query list of displays connected to this GPU
			status = NvAPI_GPU_GetConnectedDisplayIds(ahGPU[i], displayIdArray,
				&displayIdCount, flags);

			if ((status != NVAPI_OK) || (displayIdCount == 0))
			{
				PrintError2(status);
			}

			NV_GPU_DISPLAYIDS* dispIds = NULL;
			dispIds = new NV_GPU_DISPLAYIDS[displayIdCount];
			dispIds->version = NV_GPU_DISPLAYIDS_VER;

			status = NvAPI_GPU_GetConnectedDisplayIds(ahGPU[i], dispIds, &displayIdCount, 0);
			if (status != NVAPI_OK)
			{
				delete[] dispIds;
				PrintError2(status);
			}
			int hdrStatusResolved = -1; // to be sure multiple screens get same status and don't switch both if different HDR at start 
			// Iterate over displays to test for HDR capabilities
			for (NvU32 dispIndex = 0; (dispIndex < displayIdCount) && dispIds[dispIndex].isActive; dispIndex++)
			{
				NV_HDR_CAPABILITIES hdrCapabilities = {};
				hdrCapabilities.version = NV_HDR_CAPABILITIES_VER;

				if (NVAPI_OK == NvAPI_Disp_GetHdrCapabilities(dispIds[dispIndex].displayId, &hdrCapabilities))
				{
					if (hdrCapabilities.isST2084EotfSupported)
					{

						NV_HDR_COLOR_DATA hdrColorData = {};
						memset(&hdrColorData, 0, sizeof(hdrColorData));

						hdrColorData.version = NV_HDR_COLOR_DATA_VER;
						hdrColorData.cmd = NV_HDR_CMD_GET;
						hdrColorData.static_metadata_descriptor_id = NV_STATIC_METADATA_TYPE_1;

						status = NvAPI_Disp_HdrColorControl(dispIds[dispIndex].displayId, &hdrColorData);
						if (status == NVAPI_OK) {
							if (hdrColorData.hdrMode == NV_HDR_MODE_OFF) {
								if (doSwitch) {
									hdrColorData.version = NV_HDR_COLOR_DATA_VER;
									hdrColorData.cmd = NV_HDR_CMD_SET;
									hdrColorData.static_metadata_descriptor_id = NV_STATIC_METADATA_TYPE_1;
									hdrColorData.hdrMode = NV_HDR_MODE_UHDA;
									if (hdrStatusResolved > 0)
										hdrColorData.hdrMode = (NV_HDR_MODE)hdrStatusResolved;

									status = NvAPI_Disp_HdrColorControl(dispIds[dispIndex].displayId, &hdrColorData);
									if (status != NVAPI_OK)
										PrintError2(status);
									if (hdrColorData.hdrMode > 0)
										result = 1;
									hdrStatusResolved = hdrColorData.hdrMode;
								}
								else {
									result = 0;
									hdrStatusResolved = hdrColorData.hdrMode;
								}
							}
							else {
								if (doSwitch) {
									hdrColorData.version = NV_HDR_COLOR_DATA_VER;
									hdrColorData.cmd = NV_HDR_CMD_SET;
									hdrColorData.static_metadata_descriptor_id = NV_STATIC_METADATA_TYPE_1;
									hdrColorData.hdrMode = NV_HDR_MODE_OFF;
									if (hdrStatusResolved > 0)
										hdrColorData.hdrMode = (NV_HDR_MODE)hdrStatusResolved;

									status = NvAPI_Disp_HdrColorControl(dispIds[dispIndex].displayId, &hdrColorData);
									if (status != NVAPI_OK)
										PrintError2(status);
									if (hdrColorData.hdrMode == 0)
										result = 0;
									hdrStatusResolved = hdrColorData.hdrMode;
								}
								else {
									result = 1;
									hdrStatusResolved = hdrColorData.hdrMode;
								}
							}
						}
					}
				}
			}
			delete[] dispIds;
		}



		NvAPI_Unload();


		return result;

	}



}