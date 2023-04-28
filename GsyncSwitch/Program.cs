using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using Microsoft.Win32;
using WindowsInput;
using WindowsInput.Native;
using System.Runtime.InteropServices;
using System.IO;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace GsyncSwitch
{

    class ControlContainer : IContainer
    {

        ComponentCollection _components;

        public ControlContainer()
        {
            _components = new ComponentCollection(new IComponent[] { });
        }

        public void Add(IComponent component) { }
        public void Add(IComponent component, string Name) { }
        public void Remove(IComponent component) { }

        public ComponentCollection Components
        {
            get { return _components; }
        }

        public void Dispose()
        {
            _components = null;
        }
    }

    static class Program
    {
        
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            IconClass sc = new IconClass();
//            sc.notifyIcon1.ShowBalloonTip(1000);

            Application.Run();
        }
    }

    class IconClass
    {
      #if DEBUG
        const string GsyncSwitchNVAPI_DLL = @"..\..\..\..\x64\Debug\GsyncSwitchNVAPI.dll";
      #else
        const string GsyncSwitchNVAPI_DLL = @"GsyncSwitchNVAPI.dll";
      #endif

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchGsync(bool doSwitch);

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchVsync(bool doSwitch);

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchFrameLimiter(bool doSwitch, int maxFPS);

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NVAPIWrapperSwitchHDR(bool doSwitch);


        public int newGsyncStatus = 0;
        public int newVsyncStatus = 0;
        public int newFrameLimiterStatus = 0;
        public int maxFps = 0;
        public int newHDRStatus = 0;

        private String monitor1Label = "";
        private String monitor2Label = "";
        private String monitor1Id = "";
        private String monitor2Id = "";

        ControlContainer container = new ControlContainer();
        public NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem switchGsync;
        private ToolStripMenuItem switchVsync;
        private ToolStripMenuItem switchFrameLimiter;
        private ToolStripMenuItem switchHDR;
/*
        private ToolStripMenuItem to144Hz;
        private ToolStripMenuItem to120Hz;
*/
        private ToolStripMenuItem monitor2;
        private ToolStripMenuItem monitor1;
        private ToolStripMenuItem monitorClone;
        private ToolStripMenuItem monitorExtend;
        private ToolStripMenuItem speakerStatus;
        private ToolStripMenuItem editConfig;
        private ToolStripMenuItem exitApplication;
        private ToolStripMenuItem launchAtStartup;
        private Color defaultSpeakerStatusColor;

        const string fileNameDAHTwaveFormat = "dolbyAtmosDefaultWaveFormat.dat";

        // The path to the key where Windows looks for startup applications
        public RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private void setFormatToDolbyAtmosForHomeTheater(MMDevice currentDevice)
        {
            WaveFormat waveFormat = null;
            
            using (var stream = File.Open(fileNameDAHTwaveFormat, FileMode.Open))
            {
                if(stream != null)
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        waveFormat = new WaveFormat(reader);
                    }
                }
            }

            if(waveFormat != null)
            {
                PropVariant p = new PropVariant();

                IntPtr formatPointer = Marshal.AllocHGlobal(Marshal.SizeOf(waveFormat));
                Marshal.StructureToPtr(waveFormat, formatPointer, false);
                p.pointerValue = formatPointer;

                currentDevice.GetPropertyInformation(StorageAccessMode.ReadWrite);
                currentDevice.Properties.SetValue(PropertyKeys.PKEY_AudioEngine_DeviceFormat, p);
            }
        }

        private string getSpeakerStatus()
        {

            string audioSpeakersValue = "N/A";
            var deviceEnum = new MMDeviceEnumerator();
            MMDevice currentDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            WaveFormat waveFormat =null;

            if (currentDevice.Properties.Contains(PropertyKeys.PKEY_AudioEngine_DeviceFormat))
            {
                var value = currentDevice.Properties[PropertyKeys.PKEY_AudioEngine_DeviceFormat].Value as byte[];

                IntPtr unmanagedPointer = Marshal.AllocHGlobal(value.Length);
                Marshal.Copy(value, 0, unmanagedPointer, value.Length);
                Marshal.FreeHGlobal(unmanagedPointer);
                waveFormat = WaveFormat.MarshalFromPtr(unmanagedPointer);

                audioSpeakersValue = waveFormat.SampleRate.ToString() + "Hz " + waveFormat.Channels.ToString() + " channels " + waveFormat.BitsPerSample.ToString() + " bits";
            }

            // special for my sound card, if Dolby Atmos for Home Theater isn't used
            if (currentDevice.FriendlyName.Contains("EP-HDMI")&&waveFormat!=null)
            {
                if (waveFormat.Channels < 8)
                {
                    if (speakerStatus.ForeColor != null && !speakerStatus.ForeColor.Equals(Color.Red))
                    {
                        defaultSpeakerStatusColor = speakerStatus.ForeColor ;
                    }
                    speakerStatus.ForeColor = Color.Red;
//                    setFormatToDolbyAtmosForHomeTheater(currentDevice);
                }
                else
                {
                    speakerStatus.ForeColor = defaultSpeakerStatusColor;

                    //write defaut DolbyAmtosForHomeTheater default waveFormat in a file, so we can set it back if not selected
                    //commented : doesn't work
                    /*
                    if (!File.Exists(fileNameDAHTwaveFormat))
                    {
                        using (var stream = File.Open(fileNameDAHTwaveFormat, FileMode.Create))
                        {
                            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                            {
                                waveFormat.Serialize(writer);
                            }
                        }
                    }
                    */
                }
            }else if (speakerStatus.ForeColor != null && speakerStatus.ForeColor.Equals(Color.Red))
            {
                speakerStatus.ForeColor = defaultSpeakerStatusColor;
            }


                /*
                            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
                            foreach (MMDevice device in devices)
                            {

                                if (device.FriendlyName.Contains("EP-HDMI"))
                                {

                                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEngine_DeviceFormat))
                                    {
                                        var value = device.Properties[PropertyKeys.PKEY_AudioEngine_DeviceFormat].Value as byte[];

                                        IntPtr unmanagedPointer = Marshal.AllocHGlobal(value.Length);
                                        Marshal.Copy(value, 0, unmanagedPointer, value.Length);
                                        Marshal.FreeHGlobal(unmanagedPointer);
                                        var waveFormat = WaveFormat.MarshalFromPtr(unmanagedPointer);

                                        audioSpeakersValue = waveFormat.SampleRate.ToString() +"Hz "+waveFormat.Channels.ToString() +" channels " +waveFormat.BitsPerSample.ToString()+ " bits";
                                    }
                                }
                            }
                */

                return audioSpeakersValue;
        }


        private void debugSpeakerStatus()
        {
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach (MMDevice device in devices)
            {
                System.Diagnostics.Debug.WriteLine("device.FriendlyName: " + device.FriendlyName.ToString());
                System.Diagnostics.Debug.WriteLine("device.State: "+ device.State.ToString());
                System.Diagnostics.Debug.WriteLine("device.AudioClient: " + device.AudioClient.ToString());


                if (device.FriendlyName.Contains("EP-HDMI"))
                {
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_Association))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_Association] ;
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_Association: "+value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_ControlPanelPageProvider))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_ControlPanelPageProvider];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_ControlPanelPageProvider: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_Disable_SysFx))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_Disable_SysFx];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_Disable_SysFx: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_FormFactor))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_FormFactor];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_FormFactor: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_FullRangeSpeakers))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_FullRangeSpeakers];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_FullRangeSpeakers: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_GUID))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_GUID];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_GUID: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_JackSubType))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_JackSubType];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_JackSubType: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_PhysicalSpeakers))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_PhysicalSpeakers];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_PhysicalSpeakers: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_Supports_EventDriven_Mode))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_Supports_EventDriven_Mode];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_Supports_EventDriven_Mode: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEngine_DeviceFormat))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEngine_DeviceFormat].Value as byte[];

                        IntPtr unmanagedPointer = Marshal.AllocHGlobal(value.Length);
                        Marshal.Copy(value, 0, unmanagedPointer, value.Length);
                        Marshal.FreeHGlobal(unmanagedPointer);
                        var waveFormat = WaveFormat.MarshalFromPtr(unmanagedPointer);

                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEngine_DeviceFormat: " + waveFormat.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEngine_OEMFormat))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEngine_OEMFormat].Value as byte[];

                        IntPtr unmanagedPointer = Marshal.AllocHGlobal(value.Length);
                        Marshal.Copy(value, 0, unmanagedPointer, value.Length);
                        Marshal.FreeHGlobal(unmanagedPointer);
                        var waveFormat = WaveFormat.MarshalFromPtr(unmanagedPointer);

                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEngine_OEMFormat: " + waveFormat.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_DeviceInterface_FriendlyName];
                        System.Diagnostics.Debug.WriteLine("PKEY_DeviceInterface_FriendlyName: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_ControllerDeviceId))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_ControllerDeviceId];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_ControllerDeviceId: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_DeviceDesc))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_DeviceDesc];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_DeviceDesc: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_FriendlyName))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_FriendlyName];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_FriendlyName: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_IconPath))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_IconPath];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_IconPath: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_InstanceId))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_InstanceId];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_InstanceId: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_InterfaceKey))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_InterfaceKey];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_InterfaceKey: " + value.Value.ToString());
                    }

                }

            }

        }

        public IconClass()
        {

            // read ini file config.ini to get some values
            string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IniFile iniFile = new IniFile(iniFilePath);

            monitor1Label = iniFile.Read("monitors", "monitor1Label");
            monitor2Label = iniFile.Read("monitors", "monitor2Label");
            monitor1Id = iniFile.Read("monitors", "monitor1Id");
            monitor2Id = iniFile.Read("monitors", "monitor2Id");

            int.TryParse(iniFile.Read("frameLimiterMaxFps", "maxFps"), out maxFps) ;

            // Load parameters from the INI file
            Dictionary<string, string> frequencies = new Dictionary<string, string>();
            string[] sectionNames = new string[] { "frequencies" }; // section names from config file

            foreach (string sectionName in sectionNames)
            {
                foreach (string key in iniFile.GetKeys(sectionName))
                {
                    string value = iniFile.Read(sectionName, key);
                    frequencies.Add(key, value);
                }
            }

            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);

            this.notifyIcon1 = new NotifyIcon(container);
            this.notifyIcon1.Icon = new Icon(this.GetType(), "Letter_G.ico");
            this.notifyIcon1.Text = "Gsync Switch by KwizatZ";

            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipTitle = "Gsync Switch";
            this.notifyIcon1.BalloonTipText = "made by KwizatZ";

            this.notifyIcon1.Visible = true;

            contextMenu = new ContextMenuStrip();
            switchGsync = new ToolStripMenuItem();
            switchVsync = new ToolStripMenuItem();
            switchFrameLimiter = new ToolStripMenuItem();
            switchHDR = new ToolStripMenuItem();
            monitor2 = new ToolStripMenuItem();
            monitor1 = new ToolStripMenuItem();
            monitorClone = new ToolStripMenuItem();
            monitorExtend = new ToolStripMenuItem();
            speakerStatus = new ToolStripMenuItem();
            editConfig = new ToolStripMenuItem();
            exitApplication = new ToolStripMenuItem();
            launchAtStartup = new ToolStripMenuItem();

            contextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
//            contextMenu.Renderer = new KwizatZMenuRenderer();

            this.notifyIcon1.ContextMenuStrip = contextMenu;

            this.notifyIcon1.DoubleClick += NotifyIcon_DoubleClick;
            this.notifyIcon1.Click += NotifyIcon_Click;

            switchGsync.Text = "Switch Gsync";
            if(newGsyncStatus==1)
                switchGsync.Image = GsyncSwitch.Properties.Resources.nvidia_logo ;  
            else
                switchGsync.Image = GsyncSwitch.Properties.Resources.nvidia_logo_off;
            switchGsync.Click += new EventHandler(SwitchGsync_Click);
            contextMenu.Items.Add(switchGsync);

            switchHDR.Text = "Switch HDR";
            if (newHDRStatus==1)
                switchHDR.Image = GsyncSwitch.Properties.Resources.hdr_on;
            else
                switchHDR.Image = GsyncSwitch.Properties.Resources.hdr;

            switchHDR.Image = GsyncSwitch.Properties.Resources.hdr_on;
            switchHDR.Click += new EventHandler(SwitchHDR_Click);
            contextMenu.Items.Add(switchHDR);

            contextMenu.Items.Add(new ToolStripSeparator());

            switchVsync.Text = "Switch Vsync";
            if (newVsyncStatus == 1)
                switchVsync.Image = GsyncSwitch.Properties.Resources.vsync_on;
            else
                switchVsync.Image = GsyncSwitch.Properties.Resources.vsync_off;
            switchVsync.Click += new EventHandler(SwitchVsync_Click);
            contextMenu.Items.Add(switchVsync);

            switchFrameLimiter.Text = "Switch Frame Limiter (" + maxFps + ")";
            if (newFrameLimiterStatus == 1)
                switchFrameLimiter.Image = GsyncSwitch.Properties.Resources.limiter_on;
            else
                switchFrameLimiter.Image = GsyncSwitch.Properties.Resources.limiter_off;
            switchFrameLimiter.Click += new EventHandler(SwitchFrameLimiter_Click);
            contextMenu.Items.Add(switchFrameLimiter);

            contextMenu.Items.Add(new ToolStripSeparator());

            speakerStatus.Text = "Speaker status : " + getSpeakerStatus();
            speakerStatus.Image = GsyncSwitch.Properties.Resources.control_volume_blue;
            speakerStatus.Click += new EventHandler(SpeakerStatus_Click);
            contextMenu.Items.Add(speakerStatus);


            contextMenu.Items.Add(new ToolStripSeparator());

            bool isFrequency = false;
            foreach (KeyValuePair<string, string> pair in frequencies)
            {
                // Get the key and value from the KeyValuePair object
                string key = pair.Key;
                string value = pair.Value;

                ToolStripMenuItem toFrequency = new ToolStripMenuItem();
                toFrequency.Text = key;
                toFrequency.Tag = value;
                toFrequency.Click += new EventHandler(ToFrequency_Click);
                contextMenu.Items.Add(toFrequency);

                isFrequency = true;
            }

            if(isFrequency) 
                contextMenu.Items.Add(new ToolStripSeparator());

            monitor2.Text = "Monitor "+monitor2Label;
            monitor2.Click += new EventHandler(Monitor2_Click);
            contextMenu.Items.Add(monitor2);

            monitor1.Text = "Monitor " + monitor1Label;
            monitor1.Click += new EventHandler(Monitor1_Click);
            contextMenu.Items.Add(monitor1);

            monitorClone.Text = "Monitor Clone";
            monitorClone.Click += new EventHandler(MonitorClone_Click);
            contextMenu.Items.Add(monitorClone);

            monitorExtend.Text = "Monitor Extend";
            monitorExtend.Click += new EventHandler(MonitorExtend_Click);
            contextMenu.Items.Add(monitorExtend);

            contextMenu.Items.Add(new ToolStripSeparator());

            editConfig.Text = "Edit configuration";
            editConfig.Click += new EventHandler(EditConfig_Click);
            contextMenu.Items.Add(editConfig);
            
            exitApplication.Text = "Exit..";
            exitApplication.Click += new EventHandler(ExitApplication_Click);
            contextMenu.Items.Add(exitApplication);

            launchAtStartup.Text = "Launch at Windows startup";
            // Check to see the current state (running at startup or not)
            if (rkApp.GetValue("GsyncSwitch") == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                launchAtStartup.Checked = false;
            }
            else
            {
                // The value exists, the application is set to run at startup
                launchAtStartup.Checked = true;
            }
            launchAtStartup.Click += new EventHandler(LaunchAtStartup_Click);
            contextMenu.Items.Add(launchAtStartup);

        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            //            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            //            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            //            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false,maxFps);
            //            newHDRStatus = NVAPIWrapperSwitchHDR(false);

            speakerStatus.Text = "Speaker status : " + getSpeakerStatus();
            if (newGsyncStatus==1)
                switchGsync.Image = GsyncSwitch.Properties.Resources.nvidia_logo;
            else
                switchGsync.Image = GsyncSwitch.Properties.Resources.nvidia_logo_off;
            if (newVsyncStatus == 1)
                switchVsync.Image = GsyncSwitch.Properties.Resources.vsync_on;
            else
                switchVsync.Image = GsyncSwitch.Properties.Resources.vsync_off;
            if (newFrameLimiterStatus == 1)
                switchFrameLimiter.Image = GsyncSwitch.Properties.Resources.limiter_on;
            else
                switchFrameLimiter.Image = GsyncSwitch.Properties.Resources.limiter_off;
            if (newHDRStatus==1)
                switchHDR.Image = GsyncSwitch.Properties.Resources.hdr_on;
            else
                switchHDR.Image = GsyncSwitch.Properties.Resources.hdr;

            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        private void MonitorExtend_Click(object sender, EventArgs e)
        {
            MonitorSwitch.ExtendDisplays();
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false,maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(true);
        }

        private void MonitorClone_Click(object sender, EventArgs e)
        {
            MonitorSwitch.CloneDisplays(monitor1Id,monitor2Id);
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(true);
        }

        private void Monitor1_Click(object sender, EventArgs e)
        {
            MonitorSwitch.InternalDisplay();
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
        }

        private void Monitor2_Click(object sender, EventArgs e)
        {
            MonitorSwitch.ExternalDisplay();
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
        }

        private void ToFrequency_Click(object sender, EventArgs e)
        {
            string menuItemCommand = (string)((ToolStripMenuItem)sender).Tag;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Process nircmdEXE = new Process();
            nircmdEXE.StartInfo.FileName = "nircmd.exe";
            nircmdEXE.StartInfo.Arguments = "setdisplay "+ menuItemCommand;
            nircmdEXE.Start();
        }

        private async void SwitchHDR_Click(object sender, EventArgs e)
        {
            newHDRStatus = NVAPIWrapperSwitchHDR(true);
            System.Threading.Thread.Sleep(2000);
            int currentHDRStatus = NVAPIWrapperSwitchHDR(false);
            // sometimes HDR doens't switch with NVAPI for some reason : force W11 switch with shortcut
            if (newHDRStatus != currentHDRStatus)
            {
                var simu = new InputSimulator();
                simu.Keyboard.ModifiedKeyStroke(new[] { VirtualKeyCode.LWIN, VirtualKeyCode.LMENU }, VirtualKeyCode.VK_B);
            }
        }

        private void LaunchAtStartup_Click(object sender, EventArgs e)
        {
            if (!launchAtStartup.Checked)
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("GsyncSwitch", Application.ExecutablePath);
                launchAtStartup.Checked = true;
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("GsyncSwitch", false);
                launchAtStartup.Checked = false;
            }
        }


        private void SpeakerStatus_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("control", "mmsys.cpl sounds"));
        }

        private void EditConfig_Click(object sender, EventArgs e)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            Process.Start("notepad.exe", configFilePath);

        }
        private void ExitApplication_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;

            System.Windows.Forms.Application.Exit();
        }

        private void SwitchGsync_Click(object sender, EventArgs e)
        {
            newGsyncStatus = NVAPIWrapperSwitchGsync(true);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
        }
        private void SwitchVsync_Click(object sender, EventArgs e)
        {
            newVsyncStatus = NVAPIWrapperSwitchVsync(true);
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
        }
        private void SwitchFrameLimiter_Click(object sender, EventArgs e)
        {
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(true,maxFps);
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);

            if (newGsyncStatus == newHDRStatus)
            {
                newGsyncStatus = NVAPIWrapperSwitchGsync(true);
                newHDRStatus = NVAPIWrapperSwitchHDR(true);
            }
            else
            {
                newHDRStatus = NVAPIWrapperSwitchHDR(true);
            }

            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
        }
    }

}
