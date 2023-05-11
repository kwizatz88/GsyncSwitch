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
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;

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
        private bool showControllerStatus = false;
        private bool use2monitors = false;
        private String controllerStatusText = "unavailable";

        ControlContainer container = new ControlContainer();
        public NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem switchGsync;
        private ToolStripMenuItem switchVsync;
        private ToolStripMenuItem switchFrameLimiter;
        private ToolStripMenuItem switchHDR;
        private ToolStripMenuItem monitor2;
        private ToolStripMenuItem monitor1;
        private ToolStripMenuItem monitorClone;
        private ToolStripMenuItem monitorExtend;
        private ToolStripMenuItem speakerStatus;
        private ToolStripMenuItem controllerStatus;
        private ToolStripMenuItem editConfig;
        private ToolStripMenuItem editOldConfig;
        private ToolStripMenuItem exitApplication;
        private ToolStripMenuItem settings;
        private ToolStripMenuItem launchAtStartup;
        private ToolStripMenuItem openLocationFolder;
        private ToolStripMenuItem openProjectWebsite;
        private Color defaultSpeakerStatusColor;
        private Peripherals peripherals;

        // The path to the key where Windows looks for startup applications
        public RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private string getSpeakerStatus()
        {

            string audioSpeakersValue = "N/A";
            var deviceEnum = new MMDeviceEnumerator();
            MMDevice currentDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            WaveFormat waveFormat = null;

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
            if (currentDevice.FriendlyName.Contains("EP-HDMI") && waveFormat != null)
            {
                if (waveFormat.Channels < 8)
                {
                    if (speakerStatus.ForeColor != null && !speakerStatus.ForeColor.Equals(Color.Red))
                    {
                        defaultSpeakerStatusColor = speakerStatus.ForeColor;
                    }
                    speakerStatus.ForeColor = Color.Red;
                }
                else
                {
                    speakerStatus.ForeColor = defaultSpeakerStatusColor;
                }
            }
            else if (speakerStatus.ForeColor != null && speakerStatus.ForeColor.Equals(Color.Red))
            {
                speakerStatus.ForeColor = defaultSpeakerStatusColor;
            }

            return audioSpeakersValue;
        }



        private string getControllerStatus()
        {
            int batteryLevel = peripherals.GetControllerBatteryLevel();
            if (batteryLevel != -1)
            {
                if (batteryLevel >= 75)
                    controllerStatusText = "High";
                else if (batteryLevel >=45)
                    controllerStatusText = "Medium";
                else
                    controllerStatusText = "Low";
            }
            else
                controllerStatusText = "unavailable";

            return controllerStatusText;
        }

        public IconClass()
        {
            peripherals = new Peripherals();
            Dictionary<string, string> frequencies = new Dictionary<string, string>();

            // Load settings from registry or ini file
            if (RegistryHelper.RegistryKeyExists(Registry.CurrentUser, RegistryHelper.REG_KEY))
            {
                use2monitors = RegistryHelper.GetBoolValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Use2Monitors");
                monitor1Label = RegistryHelper.GetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor1Label");
                monitor2Label = RegistryHelper.GetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor2Label");
                monitor1Id = RegistryHelper.GetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor1Id");
                monitor2Id = RegistryHelper.GetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor2Id");
                maxFps = RegistryHelper.GetIntValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "MaxFps");
                showControllerStatus = RegistryHelper.GetBoolValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "ShowControllerStatus");
                frequencies = RegistryHelper.GetDictionary(Registry.CurrentUser, RegistryHelper.REG_KEY, "Frequencies");
            }
            else
            {
                // read ini file config.ini to get some values
                string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
                IniFile iniFile = new IniFile(iniFilePath);

                bool.TryParse(iniFile.Read("monitors", "use2monitors"), out use2monitors);
                monitor1Label = iniFile.Read("monitors", "monitor1Label");
                monitor2Label = iniFile.Read("monitors", "monitor2Label");
                monitor1Id = iniFile.Read("monitors", "monitor1Id");
                monitor2Id = iniFile.Read("monitors", "monitor2Id");

                int.TryParse(iniFile.Read("frameLimiterMaxFps", "maxFps"), out maxFps);
                bool.TryParse(iniFile.Read("others", "showControllerStatus"), out showControllerStatus);

                // Load parameters from the INI file
                string[] sectionNames = new string[] { "frequencies" }; // section names from config file

                foreach (string sectionName in sectionNames)
                {
                    foreach (string key in iniFile.GetKeys(sectionName))
                    {
                        string value = iniFile.Read(sectionName, key);
                        frequencies.Add(key, value);
                    }
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
            controllerStatus = new ToolStripMenuItem();
            editConfig = new ToolStripMenuItem();
            editOldConfig = new ToolStripMenuItem();
            settings = new ToolStripMenuItem(); 
            exitApplication = new ToolStripMenuItem();
            launchAtStartup = new ToolStripMenuItem();
            openLocationFolder = new ToolStripMenuItem();
            openProjectWebsite = new ToolStripMenuItem();

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

            if (showControllerStatus)
            {
                controllerStatus.Text = "XBox Controller status : " + getControllerStatus();
                if(controllerStatusText != null&&!controllerStatusText.Equals("unavailable"))
                    controllerStatus.Image = GsyncSwitch.Properties.Resources.controller_on;
                else
                    controllerStatus.Image = GsyncSwitch.Properties.Resources.controller_off;
                controllerStatus.Click += new EventHandler(ControllerStatus_Click);
                contextMenu.Items.Add(controllerStatus);

                contextMenu.Items.Add(new ToolStripSeparator());
            }

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

            if (use2monitors)
            {
                monitor2.Text = "Monitor " + monitor2Label;
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
            }

            settings.Text = "Settings";

            editConfig.Text = "Edit configuration";
            editConfig.Click += new EventHandler(EditConfig_Click);
            settings.DropDownItems.Add(editConfig);
            settings.DropDownItems.Add(new ToolStripSeparator());

            editOldConfig.Text = "Try to open config.ini from previous version";
            editOldConfig.Click += new EventHandler(EditOldConfig_Click);
            settings.DropDownItems.Add(editOldConfig);

            openLocationFolder.Text = "Open GsyncSwitch location folder";
            openLocationFolder.Click += new EventHandler(OpenLocationFolder_Click);
            settings.DropDownItems.Add(openLocationFolder);

            openProjectWebsite.Text = "View Project Website";
            openProjectWebsite.Click += new EventHandler(OpenProjectWebsite_Click);
            settings.DropDownItems.Add(openProjectWebsite);
            settings.DropDownItems.Add(new ToolStripSeparator());

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
                // to update the value as ClickOnce deploy keeps 2 versions, to avoid wrong one starts
                rkApp.DeleteValue("GsyncSwitch", false);
            }
            // to update the value as ClickOnce deploy keeps 2 versions, to avoid wrong one starts
            // Add the new value
            rkApp.SetValue("GsyncSwitch", Application.ExecutablePath);

            launchAtStartup.Click += new EventHandler(LaunchAtStartup_Click);
            settings.DropDownItems.Add(launchAtStartup);

            contextMenu.Items.Add(settings);

            contextMenu.Items.Add(new ToolStripSeparator());

            exitApplication.Text = "Exit..";
            exitApplication.Click += new EventHandler(ExitApplication_Click);
            contextMenu.Items.Add(exitApplication);

        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            //            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            //            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            //            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false,maxFps);
            //            newHDRStatus = NVAPIWrapperSwitchHDR(false);

            if (showControllerStatus)
            {
                controllerStatus.Text = "XBox Controller status : " + getControllerStatus();
                if (controllerStatusText != null && !controllerStatusText.Equals("unavailable"))
                    controllerStatus.Image = GsyncSwitch.Properties.Resources.controller_on;
                else
                    controllerStatus.Image = GsyncSwitch.Properties.Resources.controller_off;
            }

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
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
        }

        private void MonitorClone_Click(object sender, EventArgs e)
        {
            MonitorSwitch.CloneDisplays(monitor1Id,monitor2Id);
            newGsyncStatus = NVAPIWrapperSwitchGsync(false);
            newVsyncStatus = NVAPIWrapperSwitchVsync(false);
            newFrameLimiterStatus = NVAPIWrapperSwitchFrameLimiter(false, maxFps);
            newHDRStatus = NVAPIWrapperSwitchHDR(false);
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

        private void ControllerStatus_Click(object sender, EventArgs e)
        {
            var simu = new InputSimulator();
            simu.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_G);

        }

        private void EditConfig_Click(object sender, EventArgs e)
        {
            /*
                        string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
                        Process.Start("notepad.exe", configFilePath);
            */

            // Create a new instance of the SettingsForm
            SettingsForm settingsForm = new SettingsForm();

            // Show the form as a modal dialog
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                // The user clicked "Save", so save the updated settings from the form
                var confirmResult = MessageBox.Show("Settings saved successfully to registry (config.ini is deprecated).\nDo you want to restart app to load new settings ?", "Confirm Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (confirmResult == DialogResult.Yes)
                {
                    Application.Restart();
                }
            }
            else
            {
                // The user clicked "Cancel", so don't do anything
                MessageBox.Show("Changes discarded.", "Info");
            }
        }
        private void EditOldConfig_Click(object sender, EventArgs e)
        {
            string appDataDir = "";
            string errorMsg = "Previous version of config.ini not found. You can try using the \"Open GsyncSwitch location folder\" menu instead and going one folder upper to check if previous installation with config.ini is available";
            string errorMsgTitle = "Previous configuration not found";

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                appDataDir = Path.Combine(baseDir, "..");

                string[] dirs = Directory.GetDirectories(appDataDir, "gsyn..tion_*_*_*");

                if (dirs.Length >= 2)
                {
                    string previousVersionDir = dirs[dirs.Length - 2];
                    Process.Start("notepad.exe", previousVersionDir + "\\config.ini");
                }
                else
                {
                    MessageBox.Show(errorMsg, errorMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception)
            {
                MessageBox.Show(errorMsg, errorMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void OpenLocationFolder_Click(object sender, EventArgs e)
        {
            // Get the path of the current domain base directory
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            // Open the directory in Windows Explorer
            Process.Start("explorer.exe", path);
        }
        private void OpenProjectWebsite_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/kwizatz88/GsyncSwitch") { UseShellExecute = true });
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
