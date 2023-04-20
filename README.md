# GsyncSwitch
Simple Windows App to switch G-Sync or HDR on/off with one click in taskbar

If you take the release (here on the right), just put the files in the same folder and launch GsyncSitch.exe for toolbar app

To switch Gsync mode, just double click on icon in taskbar :
![image](https://user-images.githubusercontent.com/71530061/163377488-4f60ebdc-3005-47ec-89d9-f47d475a3db5.png)

Or use right click for menu :
![image](https://user-images.githubusercontent.com/71530061/163563377-569ec630-a67e-4d23-9330-11b757626d89.png)

Note : as a gamer I don't want an app using resources on background, so the app does nothing unless you click on it (no timed status update nor stuff like that)

----------------------------------------------------------------------------------------------------------------------------                                                                                                              
If you want to build the projects yourself :

- GsyncSwitchEXE : C++ project to make an exe to switch Gsync (using NVAPI)
- GsyncSwitch : C# project for the simple app in taskbar (it has GsyncSwitchEXE.exe in it)

To compile GsyncSwitchEXE project, you need NVAPI (not included for copyright purpose) available here :
https://developer.nvidia.com/nvapi


V1.1 : added HDR switch (at least for W11, not sure if shortcut win+alt+b works in W10 now : app just sends shortcut)

v1.3 : added creator info so W11 antivirus doesn't get crazy for no reason + personal needs in bar :

v1.4 : added quick shortcut to sound control in menu

v2.0 : migrated app to .NET6.0, cause previous version couldn't access advanced sound settings
release also contains .NET6.0 framework, so there are more files, the one to launch is still GsyncSwitch.exe
<br>.NET Desktop Runtime 6.0.10 needed :
https://dotnet.microsoft.com/en-us/download/dotnet/6.0 

also added sound control info :
![Capture d’écran 2022-06-08 094926](https://user-images.githubusercontent.com/71530061/172562388-3d66311c-6547-4a5b-bbd0-5d260276441b.png)

v3.0 : removed the  exe call to switch Gsync

now all the NVAPI calls are made throught CPP linked with dll : this will allow me to add multiple others NVAPI calls

new one is current Gsync state, shown in label :
![2023-04-19](https://user-images.githubusercontent.com/71530061/233081007-6b3bdaf0-e4d3-4d29-8497-7efe9540e6ab.png)

v3.0.1 : replaced the annoying Gsync state label by icon color : red = OFF, green = ON
![Capture d’écran 2023-04-19 203808](https://user-images.githubusercontent.com/71530061/233169377-33200148-4f22-4ef7-ad66-e565785f182c.png)
![Capture d’écran 2023-04-19 203838](https://user-images.githubusercontent.com/71530061/233169431-67fb912e-ec80-4e62-b419-846ebbc0ebb0.png)
