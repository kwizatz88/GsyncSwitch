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
	exit(-1);
}


extern "C" {
	GsyncSwitchNVAPIWrapper BSTR NVAPIWrapperSwitchGsync(bool doSwitch) {
        
        BSTR result = SysAllocString(L""); 

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
        // (4) Specify that we want the VSYNC disabled setting
         // first we fill the NVDRS_SETTING struct, then we call the function
        NVDRS_SETTING drsSetting = { 0 };
        drsSetting.version = NVDRS_SETTING_VER;
        /*
            drsSetting.settingId = VSYNCMODE_ID;
            drsSetting.settingType = NVDRS_DWORD_TYPE;
            drsSetting.u32CurrentValue = VSYNCMODE_FORCEOFF;
            */

        drsSetting.settingId = VRR_MODE_ID;
        drsSetting.settingType = NVDRS_DWORD_TYPE;

        status = NvAPI_DRS_GetSetting(hSession, hProfile, VRR_MODE_ID, &drsSetting);
        if (status != NVAPI_OK)
            PrintError2(status);

        if (drsSetting.u32CurrentValue == VRR_MODE_DISABLED) {
            if(doSwitch)
                result = SysAllocString(L"current state : ON");
            else
                result = SysAllocString(L"current state : OFF");
            drsSetting.u32CurrentValue = VRR_MODE_DEFAULT;
        }
        else {
            if (doSwitch)
                result = SysAllocString(L"current state : OFF");
            else
                result = SysAllocString(L"current state : ON");
            drsSetting.u32CurrentValue = VRR_MODE_DISABLED;
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
        
        return result;

	}

}