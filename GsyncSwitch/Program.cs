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
            Application.SetCompatibleTextRenderingDefault(false);

            IconClass sc = new IconClass();
//            sc.notifyIcon1.ShowBalloonTip(1000);

            Application.Run();
        }
    }

    class IconClass
    {
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
        private ToolStripMenuItem soundControl;
        private ToolStripMenuItem exitApplication;
        private ToolStripMenuItem launchAtStartup;

        // The path to the key where Windows looks for startup applications
        public RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public IconClass()
        {
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
            soundControl = new ToolStripMenuItem();
            exitApplication = new ToolStripMenuItem();
            launchAtStartup = new ToolStripMenuItem();

            contextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
//            contextMenu.Renderer = new KwizatZMenuRenderer();

            this.notifyIcon1.ContextMenuStrip = contextMenu;

            this.notifyIcon1.DoubleClick += NotifyIcon_DoubleClick;

            switchGsync.Text = "Switch Gsync";
            switchGsync.Image = GsyncSwitch.Properties.Resources.nvidia_logo ;  
            switchGsync.Click += new EventHandler(SwitchGsync_Click);
            contextMenu.Items.Add(switchGsync);

            switchHDR.Text = "Switch HDR";
            switchHDR.Image = GsyncSwitch.Properties.Resources.hdr;
            switchHDR.Click += new EventHandler(SwitchHDR_Click);
            contextMenu.Items.Add(switchHDR);

            to144Hz.Text = "To 144 Hz";
            to144Hz.Click += new EventHandler(To144Hz_Click);
            contextMenu.Items.Add(to144Hz);

            to120Hz.Text = "To 120 Hz";
            to120Hz.Click += new EventHandler(To120Hz_Click);
            contextMenu.Items.Add(to120Hz);

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

            soundControl.Text = "Sound Control";
            soundControl.Click += new EventHandler(SoundControl_Click);
            contextMenu.Items.Add(soundControl);

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


        private void SoundControl_Click(object sender, EventArgs e)
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
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Process gsyncSwitchEXE = new Process();
            gsyncSwitchEXE.StartInfo.FileName = "GsyncSwitchEXE.exe";
            //            gsyncSwitchEXE.StartInfo.Arguments = "DemoText";
            gsyncSwitchEXE.Start();
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
