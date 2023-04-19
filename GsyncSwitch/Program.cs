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

//        const string GsyncSwitchNVAPI_DLL = @"..\..\..\..\x64\Debug\GsyncSwitchNVAPI.dll";
        const string GsyncSwitchNVAPI_DLL = @"GsyncSwitchNVAPI.dll";

        [DllImport(GsyncSwitchNVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public static extern string NVAPIWrapperSwitchGsync(bool doSwitch);


        public string newGsyncStatus = "";
        ControlContainer container = new ControlContainer();
        public NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem switchGsync;
        private ToolStripMenuItem switchHDR;
        private ToolStripMenuItem to144Hz;
        private ToolStripMenuItem to120Hz;
        private ToolStripMenuItem monitorQN95B;
        private ToolStripMenuItem monitorQN95A;
        private ToolStripMenuItem monitorClone;
        private ToolStripMenuItem monitorExtend;
        private ToolStripMenuItem speakerStatus;
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
            //            debugSpeakerStatus();

            newGsyncStatus = NVAPIWrapperSwitchGsync(false);

            this.notifyIcon1 = new NotifyIcon(container);
            this.notifyIcon1.Icon = new Icon(this.GetType(), "Letter_G.ico");
            this.notifyIcon1.Text = "Gsync Switch by KwizatZ";

            this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipTitle = "Gsync Switch";
            this.notifyIcon1.BalloonTipText = "made by KwizatZ";

            this.notifyIcon1.Visible = true;

            contextMenu = new ContextMenuStrip();
            switchGsync = new ToolStripMenuItem();
            switchHDR = new ToolStripMenuItem();
            to120Hz = new ToolStripMenuItem();
            to144Hz = new ToolStripMenuItem();
            monitorQN95B = new ToolStripMenuItem();
            monitorQN95A = new ToolStripMenuItem();
            monitorClone = new ToolStripMenuItem();
            monitorExtend = new ToolStripMenuItem();
            speakerStatus = new ToolStripMenuItem();
            exitApplication = new ToolStripMenuItem();
            launchAtStartup = new ToolStripMenuItem();

            contextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
//            contextMenu.Renderer = new KwizatZMenuRenderer();

            this.notifyIcon1.ContextMenuStrip = contextMenu;

            this.notifyIcon1.DoubleClick += NotifyIcon_DoubleClick;
            this.notifyIcon1.Click += NotifyIcon_Click;

            switchGsync.Text = "Switch Gsync ("+ newGsyncStatus+")";
            switchGsync.Image = GsyncSwitch.Properties.Resources.nvidia_logo ;  
            switchGsync.Click += new EventHandler(SwitchGsync_Click);
            contextMenu.Items.Add(switchGsync);

            switchHDR.Text = "Switch HDR";
            switchHDR.Image = GsyncSwitch.Properties.Resources.hdr;
            switchHDR.Click += new EventHandler(SwitchHDR_Click);
            contextMenu.Items.Add(switchHDR);

            contextMenu.Items.Add(new ToolStripSeparator());

            speakerStatus.Text = "Speaker status : " + getSpeakerStatus();
            speakerStatus.Image = GsyncSwitch.Properties.Resources.control_volume_blue;
            speakerStatus.Click += new EventHandler(SpeakerStatus_Click);
            contextMenu.Items.Add(speakerStatus);


            contextMenu.Items.Add(new ToolStripSeparator());

            to144Hz.Text = "To 144 Hz";
            to144Hz.Click += new EventHandler(To144Hz_Click);
            contextMenu.Items.Add(to144Hz);

            to120Hz.Text = "To 120 Hz";
            to120Hz.Click += new EventHandler(To120Hz_Click);
            contextMenu.Items.Add(to120Hz);

            contextMenu.Items.Add(new ToolStripSeparator());

            monitorQN95B.Text = "Monitor QN95B";
            monitorQN95B.Click += new EventHandler(MonitorQN95B_Click);
            contextMenu.Items.Add(monitorQN95B);

            monitorQN95A.Text = "Monitor QN95A";
            monitorQN95A.Click += new EventHandler(MonitorQN95A_Click);
            contextMenu.Items.Add(monitorQN95A);

            monitorClone.Text = "Monitor Clone";
            monitorClone.Click += new EventHandler(MonitorClone_Click);
            contextMenu.Items.Add(monitorClone);

            monitorExtend.Text = "Monitor Extend";
            monitorExtend.Click += new EventHandler(MonitorExtend_Click);
            contextMenu.Items.Add(monitorExtend);

            contextMenu.Items.Add(new ToolStripSeparator());

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
            speakerStatus.Text = "Speaker status : " + getSpeakerStatus();
            switchGsync.Text = "Switch Gsync (" + newGsyncStatus + ")";
        }

        private void MonitorExtend_Click(object sender, EventArgs e)
        {
            MonitorSwitch.ExtendDisplays();
        }

        private void MonitorClone_Click(object sender, EventArgs e)
        {
            MonitorSwitch.CloneDisplays();
        }

        private void MonitorQN95A_Click(object sender, EventArgs e)
        {
            MonitorSwitch.InternalDisplay();
        }

        private void MonitorQN95B_Click(object sender, EventArgs e)
        {
            MonitorSwitch.ExternalDisplay();
        }

        private void To120Hz_Click(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Process nircmdEXE = new Process();
            nircmdEXE.StartInfo.FileName = "nircmd.exe";
            nircmdEXE.StartInfo.Arguments = "setdisplay 3840 2160 32 120";
            nircmdEXE.Start();
        }

        private void To144Hz_Click(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Process nircmdEXE = new Process();
            nircmdEXE.StartInfo.FileName = "nircmd.exe";
            nircmdEXE.StartInfo.Arguments = "setdisplay 3840 2160 32 144";
            nircmdEXE.Start();
        }

        private void SwitchHDR_Click(object sender, EventArgs e)
        {
            var simu = new InputSimulator();
            simu.Keyboard.ModifiedKeyStroke(new[] { VirtualKeyCode.LWIN, VirtualKeyCode.LMENU }, VirtualKeyCode.VK_B);
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


        private void ExitApplication_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;

            System.Windows.Forms.Application.Exit();
        }

        private void SwitchGsync(object sender, EventArgs e)
        {
            /* OLD METHOD, calling an EXE file
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                        Process gsyncSwitchEXE = new Process();
                        gsyncSwitchEXE.StartInfo.FileName = "GsyncSwitchEXE.exe";
                        gsyncSwitchEXE.Start();
            */
            newGsyncStatus = NVAPIWrapperSwitchGsync(true);
        }

        private void SwitchGsync_Click(object sender, EventArgs e)
        {
            SwitchGsync(sender,e);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            SwitchGsync(sender, e);
        }
    }

}
