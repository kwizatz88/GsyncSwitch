namespace GsyncSwitch
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            cbUse2Monitors = new System.Windows.Forms.CheckBox();
            tbMonitor1Label = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            tbMonitor2Label = new System.Windows.Forms.TextBox();
            tbMonitor1Id = new System.Windows.Forms.TextBox();
            tbMonitor2Id = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            nudMaxFps = new System.Windows.Forms.NumericUpDown();
            label5 = new System.Windows.Forms.Label();
            cbShowControllerStatus = new System.Windows.Forms.CheckBox();
            dgvFrequencies = new System.Windows.Forms.DataGridView();
            Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            label6 = new System.Windows.Forms.Label();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            labelSettingsMode = new System.Windows.Forms.Label();
            btnClearRegistry = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)nudMaxFps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvFrequencies).BeginInit();
            SuspendLayout();
            // 
            // cbUse2Monitors
            // 
            cbUse2Monitors.AutoSize = true;
            cbUse2Monitors.Location = new System.Drawing.Point(107, 53);
            cbUse2Monitors.Name = "cbUse2Monitors";
            cbUse2Monitors.Size = new System.Drawing.Size(306, 52);
            cbUse2Monitors.TabIndex = 0;
            cbUse2Monitors.Text = "Use 2 monitors";
            cbUse2Monitors.UseVisualStyleBackColor = true;
            // 
            // tbMonitor1Label
            // 
            tbMonitor1Label.Location = new System.Drawing.Point(107, 152);
            tbMonitor1Label.Name = "tbMonitor1Label";
            tbMonitor1Label.Size = new System.Drawing.Size(516, 55);
            tbMonitor1Label.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(654, 159);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(263, 48);
            label1.TabIndex = 2;
            label1.Text = "Monitor 1 label";
            // 
            // tbMonitor2Label
            // 
            tbMonitor2Label.Location = new System.Drawing.Point(107, 249);
            tbMonitor2Label.Name = "tbMonitor2Label";
            tbMonitor2Label.Size = new System.Drawing.Size(516, 55);
            tbMonitor2Label.TabIndex = 3;
            // 
            // tbMonitor1Id
            // 
            tbMonitor1Id.Location = new System.Drawing.Point(107, 359);
            tbMonitor1Id.Name = "tbMonitor1Id";
            tbMonitor1Id.Size = new System.Drawing.Size(516, 55);
            tbMonitor1Id.TabIndex = 4;
            // 
            // tbMonitor2Id
            // 
            tbMonitor2Id.Location = new System.Drawing.Point(107, 461);
            tbMonitor2Id.Name = "tbMonitor2Id";
            tbMonitor2Id.Size = new System.Drawing.Size(516, 55);
            tbMonitor2Id.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(654, 256);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(263, 48);
            label2.TabIndex = 6;
            label2.Text = "Monitor 2 label";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(654, 366);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(222, 48);
            label3.TabIndex = 7;
            label3.Text = "Monitor 1 ID";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(654, 468);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(222, 48);
            label4.TabIndex = 8;
            label4.Text = "Monitor 2 ID";
            // 
            // nudMaxFps
            // 
            nudMaxFps.Location = new System.Drawing.Point(107, 563);
            nudMaxFps.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            nudMaxFps.Name = "nudMaxFps";
            nudMaxFps.Size = new System.Drawing.Size(186, 55);
            nudMaxFps.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(326, 570);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(376, 48);
            label5.TabIndex = 10;
            label5.Text = "Max fps for frame lock";
            // 
            // cbShowControllerStatus
            // 
            cbShowControllerStatus.AutoSize = true;
            cbShowControllerStatus.Location = new System.Drawing.Point(107, 659);
            cbShowControllerStatus.Name = "cbShowControllerStatus";
            cbShowControllerStatus.Size = new System.Drawing.Size(416, 52);
            cbShowControllerStatus.TabIndex = 11;
            cbShowControllerStatus.Text = "Show sontroller status";
            cbShowControllerStatus.UseVisualStyleBackColor = true;
            // 
            // dgvFrequencies
            // 
            dgvFrequencies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFrequencies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Key, Value });
            dgvFrequencies.Location = new System.Drawing.Point(1086, 168);
            dgvFrequencies.Name = "dgvFrequencies";
            dgvFrequencies.RowHeadersWidth = 123;
            dgvFrequencies.RowTemplate.Height = 57;
            dgvFrequencies.Size = new System.Drawing.Size(726, 450);
            dgvFrequencies.TabIndex = 12;
            // 
            // Key
            // 
            Key.HeaderText = "Label";
            Key.MinimumWidth = 15;
            Key.Name = "Key";
            Key.Width = 300;
            // 
            // Value
            // 
            Value.HeaderText = "Nircmd value";
            Value.MinimumWidth = 15;
            Value.Name = "Value";
            Value.Width = 300;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(1308, 57);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(210, 48);
            label6.TabIndex = 13;
            label6.Text = "Frequencies";
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(1530, 850);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(225, 69);
            btnSave.TabIndex = 14;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(1221, 850);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(225, 69);
            btnCancel.TabIndex = 15;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // labelSettingsMode
            // 
            labelSettingsMode.AutoSize = true;
            labelSettingsMode.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelSettingsMode.Location = new System.Drawing.Point(107, 757);
            labelSettingsMode.Name = "labelSettingsMode";
            labelSettingsMode.Size = new System.Drawing.Size(1248, 45);
            labelSettingsMode.TabIndex = 16;
            labelSettingsMode.Text = "Settings loaded from config.ini (deprecated), click Save button to save them in registry";
            // 
            // btnClearRegistry
            // 
            btnClearRegistry.Location = new System.Drawing.Point(219, 821);
            btnClearRegistry.Name = "btnClearRegistry";
            btnClearRegistry.Size = new System.Drawing.Size(225, 69);
            btnClearRegistry.TabIndex = 17;
            btnClearRegistry.Text = "Clear Reg";
            btnClearRegistry.UseVisualStyleBackColor = true;
            btnClearRegistry.Visible = false;
            btnClearRegistry.Click += btnClearRegistry_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(20F, 48F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1871, 993);
            Controls.Add(btnClearRegistry);
            Controls.Add(labelSettingsMode);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(label6);
            Controls.Add(dgvFrequencies);
            Controls.Add(cbShowControllerStatus);
            Controls.Add(label5);
            Controls.Add(nudMaxFps);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(tbMonitor2Id);
            Controls.Add(tbMonitor1Id);
            Controls.Add(tbMonitor2Label);
            Controls.Add(label1);
            Controls.Add(tbMonitor1Label);
            Controls.Add(cbUse2Monitors);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SettingsForm";
            Text = "GsyncSwitch settings";
            ((System.ComponentModel.ISupportInitialize)nudMaxFps).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvFrequencies).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox cbUse2Monitors;
        private System.Windows.Forms.TextBox tbMonitor1Label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMonitor2Label;
        private System.Windows.Forms.TextBox tbMonitor1Id;
        private System.Windows.Forms.TextBox tbMonitor2Id;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudMaxFps;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbShowControllerStatus;
        private System.Windows.Forms.DataGridView dgvFrequencies;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label labelSettingsMode;
        private System.Windows.Forms.Button btnClearRegistry;
    }
}