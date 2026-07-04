namespace HWiNFO64_Plugin
{
    partial class PluginConfigurationView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginConfigurationView));
            SaveSettingsButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            sensorsCountLabel = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            textBox1 = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            refreshTimeInput = new System.Windows.Forms.NumericUpDown();
            linkLabel2 = new System.Windows.Forms.LinkLabel();
            button1 = new System.Windows.Forms.Button();
            listView1 = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)refreshTimeInput).BeginInit();
            SuspendLayout();

            //
            // label1 (已发现传感器:)
            //
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(12, 15);
            label1.Name = "label1";
            label1.TabIndex = 1;
            label1.Text = "Found Sensors:";
            //
            // sensorsCountLabel
            //
            sensorsCountLabel.AutoSize = true;
            sensorsCountLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            sensorsCountLabel.ForeColor = System.Drawing.Color.White;
            sensorsCountLabel.Location = new System.Drawing.Point(140, 15);
            sensorsCountLabel.Name = "sensorsCountLabel";
            sensorsCountLabel.TabIndex = 2;
            sensorsCountLabel.Text = "0";
            //
            // label2 (刷新间隔:)
            //
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(200, 15);
            label2.Name = "label2";
            label2.TabIndex = 3;
            label2.Text = "Refresh interval:";
            //
            // refreshTimeInput
            //
            refreshTimeInput.BackColor = System.Drawing.Color.FromArgb(24, 24, 24);
            refreshTimeInput.ForeColor = System.Drawing.Color.White;
            refreshTimeInput.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            refreshTimeInput.Location = new System.Drawing.Point(300, 12);
            refreshTimeInput.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            refreshTimeInput.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            refreshTimeInput.Name = "refreshTimeInput";
            refreshTimeInput.Size = new System.Drawing.Size(80, 23);
            refreshTimeInput.TabIndex = 9;
            refreshTimeInput.Value = new decimal(new int[] { 2000, 0, 0, 0 });
            //
            // SaveSettingsButton (正常尺寸)
            //
            SaveSettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            SaveSettingsButton.Location = new System.Drawing.Point(12, 45);
            SaveSettingsButton.Name = "SaveSettingsButton";
            SaveSettingsButton.Size = new System.Drawing.Size(100, 28);
            SaveSettingsButton.TabIndex = 0;
            SaveSettingsButton.Text = "Save Settings";
            SaveSettingsButton.UseVisualStyleBackColor = true;
            SaveSettingsButton.Click += SaveSettingsButton_Click;
            //
            // button1 (查看传感器 / 刷新)
            //
            button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button1.Location = new System.Drawing.Point(120, 45);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(100, 28);
            button1.TabIndex = 11;
            button1.Text = "Show sensors";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            //
            // linkLabel2 (点击此处查看配置说明)
            //
            linkLabel2.AutoSize = true;
            linkLabel2.LinkColor = System.Drawing.Color.Cyan;
            linkLabel2.Location = new System.Drawing.Point(230, 51);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.TabIndex = 10;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Click here for setup info";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            //
            // textBox1 (右上：重要提示)
            //
            textBox1.BackColor = System.Drawing.Color.Black;
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBox1.ForeColor = System.Drawing.Color.OrangeRed;
            textBox1.Location = new System.Drawing.Point(500, 12);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new System.Drawing.Size(388, 65);
            textBox1.TabIndex = 7;
            textBox1.Text = "Important: Make sure you have HWiNFO64 installed and please restart Macro-Deck if you change any settings or add new sensors in HWiNFO64";
            textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // listView1 (中央：全宽表格)
            //
            listView1.BackColor = System.Drawing.Color.FromArgb(24, 24, 24);
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            listView1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            listView1.ForeColor = System.Drawing.Color.White;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HideSelection = false;
            listView1.Location = new System.Drawing.Point(12, 90);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(876, 450);
            listView1.TabIndex = 12;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            //
            // columnHeader1 (编号)
            //
            columnHeader1.Text = "ID";
            columnHeader1.Width = 60;
            //
            // columnHeader2 (传感器)
            //
            columnHeader2.Text = "Sensor";
            columnHeader2.Width = 260;
            //
            // columnHeader3 (标签)
            //
            columnHeader3.Text = "Label";
            columnHeader3.Width = 220;
            //
            // columnHeader4 (数值)
            //
            columnHeader4.Text = "Value";
            columnHeader4.Width = 160;
            //
            // columnHeader5 (原始数值)
            //
            columnHeader5.Text = "Raw Value";
            columnHeader5.Width = 160;
            //
            // label3 (左下：图标作者)
            //
            label3.AutoSize = true;
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(12, 552);
            label3.Name = "label3";
            label3.TabIndex = 8;
            label3.Text = "Icon by LiuQQ";
            //
            // linkLabel1 (右下：Powered by HWiNFO64)
            //
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = System.Drawing.Color.Cyan;
            linkLabel1.Location = new System.Drawing.Point(730, 552);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.TabIndex = 6;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Powered by HWiNFO64";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            //
            // PluginConfigurationView
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(900, 580);
            Controls.Add(listView1);
            Controls.Add(button1);
            Controls.Add(linkLabel2);
            Controls.Add(SaveSettingsButton);
            Controls.Add(refreshTimeInput);
            Controls.Add(label3);
            Controls.Add(textBox1);
            Controls.Add(linkLabel1);
            Controls.Add(label2);
            Controls.Add(sensorsCountLabel);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PluginConfigurationView";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "HWiNFO64 Plugin Config";
            Shown += PluginConfigurationView_Shown;
            ((System.ComponentModel.ISupportInitialize)refreshTimeInput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button SaveSettingsButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label sensorsCountLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown refreshTimeInput;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}
