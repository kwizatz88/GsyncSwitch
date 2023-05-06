# GsyncSwitch
Simple and light Windows App to switch G-Sync, HDR on/off, Vsync, Frame limiter, resolutions/frequencies, monitors with one click in taskbar

.NET 6.0 runtime needed to run the app :
https://dotnet.microsoft.com/en-us/download/dotnet/6.0<br/>
(just take what's under ".NET Desktop Runtime 6.x.x.x")

(Vsync and Frame limiter are the global 3D settings you can change in NVCP)

Icons color shows the current status (green = On, black = Off) 

As a gamer I don't want an app using resources on background, so the app does nothing unless you click on it (no timed status update nor stuff like that) :

![GsyncSwitchApp](https://user-images.githubusercontent.com/71530061/234526161-30ff53a2-28a0-4593-b728-964c106f88c4.png)

If you take the release (here on the right), just put the files in the same folder, update the config.ini file as needed : you can remove, add or change the resolutions/frequencies switches and labels shown in app as you want (the line "To 144 Hz=3840 2160 32 144" can be changed to "Toto 2K 24 Hz=2560 1440 32 24" if you want): 
![GsyncSwitchConfig](https://user-images.githubusercontent.com/71530061/234523003-381c3f50-365e-4108-bc86-3bd48ba8651d.png)
"try to open previous configuration" menu doesn't revert anything, it just opens your previous config.ini file, so you can copy what you want here to manually put it in new configuration

Monitors Ids are the name they have in NVCP -note that part of the name works in ini file, if you have huge monitor names - (you can use the label you want):
![GsyncSwitchMonitorsIds](https://user-images.githubusercontent.com/71530061/234523192-99138158-c3e0-4b37-aca2-724b84d631e9.png)


and launch GsyncSwitch.exe for toolbar app (you can check launch on startup if needed)

Best use the setup.exe file to install the app and get auto updates at launch

Icons color shows the current status (green = On, black = Off) : note that the status aren't updated on right click, so if values are changed out of the app, desync my occured, until one switch is done or app reloaded.

To switch both Gsync mode and HDR mode at same time, you can also double click on icon in taskbar :
![image](https://user-images.githubusercontent.com/71530061/163377488-4f60ebdc-3005-47ec-89d9-f47d475a3db5.png)
(if their status is unsync, HDR will be switched first to sync them)


----------------------------------------------------------------------------------------------------------------------------                                                                                                              
If you want to build the projects yourself :

- GsyncSwitchNVAPI : C++ project to make a dll wrapper to switch Gsync (using NVAPI), or for other NVAPI calls 
- GsyncSwitch : C# project for the simple app in taskbar

To compile GsyncSwitchNVAPI project, you need NVAPI (not included for copyright purpose) available here :
https://developer.nvidia.com/nvapi

Note: for frequencies/resolutions switches, the app uses a free tool : nircmd.exe
