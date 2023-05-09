using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GsyncSwitch
{
    public partial class SettingsForm : Form
    {
        private const string INI_FILE = "config.ini";

        private bool use2monitors;
        private string monitor1Label;
        private string monitor2Label;
        private string monitor1Id;
        private string monitor2Id;
        private int maxFps;
        private bool showControllerStatus;
        private Dictionary<string, string> frequencies = new Dictionary<string, string>();


        public SettingsForm()
        {
            InitializeComponent();
            // Load settings from registry or ini file
            if (RegistryHelper.RegistryKeyExists(Registry.CurrentUser, RegistryHelper.REG_KEY))
            {
                labelSettingsMode.Text = "Settings loaded from registry, click Clear Reg button to clear them from registry and use config.ini instead (deprecated)";
                btnClearRegistry.Visible = true;
                LoadSettingsFromRegistry();
            }
            else
            {
                labelSettingsMode.Text = "Settings loaded from config.ini (deprecated), click Save button to save them in registry";
                btnClearRegistry.Visible = false;
                LoadSettingsFromIniFile();
            }

            // Populate form fields with loaded settings
            cbUse2Monitors.Checked = use2monitors;
            tbMonitor1Label.Text = monitor1Label;
            tbMonitor2Label.Text = monitor2Label;
            tbMonitor1Id.Text = monitor1Id;
            tbMonitor2Id.Text = monitor2Id;
            nudMaxFps.Value = maxFps;
            cbShowControllerStatus.Checked = showControllerStatus;

            foreach (var freq in frequencies)
            {
                dgvFrequencies.Rows.Add(freq.Key, freq.Value);
            }

        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            // Save settings to registry
            SaveSettingsToRegistry();
            // Set DialogResult to OK and close the form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void LoadSettingsFromRegistry()
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

        private void LoadSettingsFromIniFile()
        {
            string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, INI_FILE);
            IniFile iniFile = new IniFile(iniFilePath);

            bool.TryParse(iniFile.Read("monitors", "use2monitors"), out use2monitors);
            monitor1Label = iniFile.Read("monitors", "monitor1Label");
            monitor2Label = iniFile.Read("monitors", "monitor2Label");
            monitor1Id = iniFile.Read("monitors", "monitor1Id");
            monitor2Id = iniFile.Read("monitors", "monitor2Id");

            int.TryParse(iniFile.Read("frameLimiterMaxFps", "maxFps"), out maxFps);
            bool.TryParse(iniFile.Read("others", "showControllerStatus"), out showControllerStatus);

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

        private void SaveSettingsToRegistry()
        {

            RegistryHelper.SetBoolValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Use2Monitors", cbUse2Monitors.Checked);
            RegistryHelper.SetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor1Label", tbMonitor1Label.Text);
            RegistryHelper.SetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor2Label", tbMonitor2Label.Text);
            RegistryHelper.SetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor1Id", tbMonitor1Id.Text);
            RegistryHelper.SetStringValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "Monitor2Id", tbMonitor2Id.Text);
            RegistryHelper.SetIntValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "MaxFps", (int)nudMaxFps.Value);
            RegistryHelper.SetBoolValue(Registry.CurrentUser, RegistryHelper.REG_KEY, "ShowControllerStatus", cbShowControllerStatus.Checked);

            frequencies = new Dictionary<string, string>();
            foreach (DataGridViewRow row in dgvFrequencies.Rows)
            {
                string key = row.Cells[0].Value?.ToString();
                string value = row.Cells[1].Value?.ToString();

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    frequencies[key] = value;
                }
            }

            RegistryHelper.SetDictionary(Registry.CurrentUser, RegistryHelper.REG_KEY, "Frequencies", frequencies);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnClearRegistry_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to clear all settings from registry and load config.ini instead (deprecated) ?", "Confirm Clearing Settings From Registry", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                RegistryHelper.ClearRegistryValues(Registry.CurrentUser, RegistryHelper.REG_KEY);
                MessageBox.Show("All settings cleared successfully from registry. The application will now restart to load settings from config.ini file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
            }
        }


    }
}
