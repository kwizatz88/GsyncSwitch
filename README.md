# GsyncSwitch
Simple Windows App to switch G-Sync or HDR on/off with one click in taskbar

If you take the release (here on the right), put the 2 files in the same folder and launch GsyncSitch.exe for toolbar app or GsyncSitchEXE.exe to switch directly Gsync mode

To switch Gsync mode, just double click on icon in taskbar :
![image](https://user-images.githubusercontent.com/71530061/163377488-4f60ebdc-3005-47ec-89d9-f47d475a3db5.png)

Or use right click for menu :
![image](https://user-images.githubusercontent.com/71530061/163563377-569ec630-a67e-4d23-9330-11b757626d89.png)

----------------------------------------------------------------------------------------------------------------------------                                                                                                              
If you want to build the projects yourself :

- GsyncSwitchEXE : C++ project to make an exe to switch Gsync (using NVAPI)
- GsyncSwitch : C# project for the simple app in taskbar (it has GsyncSwitchEXE.exe in it)

To compile GsyncSwitchEXE project, you need NVAPI (not included for copyright purpose) available here :
https://developer.nvidia.com/nvapi


V1.1 : added HDR switch
