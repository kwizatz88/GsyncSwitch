# GsyncSwitch
Simple Windows App to switch G-Sync on/off with one click in taskbar

If you take the release (here on the right), put the 2 files in the same folder and launch GsyncSitch.exe for toolbar app or GsyncSitchEXE.exe to switch directly Gsync mode

To switch Gsync mode, just double click on icon in taskbar :
![image](https://user-images.githubusercontent.com/71530061/163377488-4f60ebdc-3005-47ec-89d9-f47d475a3db5.png)

Or use right click for menu :
![image](https://user-images.githubusercontent.com/71530061/163377584-97295168-ca3b-4516-adb2-c59f8e1bb86b.png)


If you want to build the projects yourself :

GsyncSwitchEXE : C++ project to make an exe to switch Gsync (using NVAPI)
GsyncSwitch : C# project for the simple app in taskbar (it has GsyncSwitchEXE.exe in it, so you need to change it for x86 for example)

To compile GsyncSwitchEXE project, you need NVAPI (not included for copyright purpose) available here :
https://developer.nvidia.com/nvapi

