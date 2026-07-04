using sugoides.HWiNFO64_Plugin;
using sugoides.HWiNFO64_Plugin.Language;
using sugoides.HWiNFO64_Plugin.Utils;
using SuchByte.MacroDeck.Plugins;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace HWiNFO64_Plugin
{
    public partial class PluginConfigurationView : Form
    {
        public PluginConfigurationView()
        {
            InitializeComponent();
            this.ApplyMacroDeckFont();
            ApplyStrings();
        }

        private void ApplyStrings()
        {
            var s = PluginLanguageManager.PluginStrings;
            this.Text = s.FormTitle;
            label1.Text = s.FoundSensors;
            label2.Text = s.RefreshInterval;
            SaveSettingsButton.Text = s.SaveSettings;
            button1.Text = s.ShowSensors;
            linkLabel2.Text = s.ClickHereForSetupInfo;
            label3.Text = s.IconByLiuQQ;
            linkLabel1.Text = s.PoweredByHWiNFO64;
            textBox1.Text = s.ImportantNotice;
            columnHeader1.Text = s.ColumnID;
            columnHeader2.Text = s.ColumnSensor;
            columnHeader3.Text = s.ColumnLabel;
            columnHeader4.Text = s.ColumnValue;
            columnHeader5.Text = s.ColumnRawValue;
            columnHeader6.Text = s.ColumnVariable;
        }

        private void PluginConfigurationView_Shown(object sender, EventArgs e)
        {
            sensorsCountLabel.Text = "" + HWiNFO64Plugin.sensors;

            var refreshTimeFromRegistry = PluginConfiguration.GetValue(HWiNFO64Plugin.Instance, "refreshTime");
            if (int.TryParse(refreshTimeFromRegistry, out var refreshTime) == false)
                refreshTime = 2000;

            refreshTimeInput.Value = refreshTime;

            // 打开即自动填充传感器列表
            PopulateSensorList();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://www.hwinfo.com");
        }

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            PluginConfiguration.SetValue(HWiNFO64Plugin.Instance, "refreshTime", refreshTimeInput.Value.ToString());
            var s = PluginLanguageManager.PluginStrings;
            MessageBox.Show(s.SettingsSaved, s.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://github.com/sugoides/macro-deck-HWiNFO64-plugin");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PopulateSensorList();
        }

        private void PopulateSensorList()
        {
            listView1.BeginUpdate();
            try
            {
                listView1.Items.Clear();
                Microsoft.Win32.RegistryKey registryPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\HWiNFO64\VSB");
                if (registryPath != null)
                {
                    for (int i = 0; i < HWiNFO64Plugin.sensors; i++)
                    {
                        var item = new ListViewItem { Text = i.ToString() };
                        item.SubItems.Add(registryPath.GetValue("Sensor" + i)?.ToString() ?? string.Empty);
                        item.SubItems.Add(registryPath.GetValue("Label" + i)?.ToString() ?? string.Empty);
                        item.SubItems.Add(registryPath.GetValue("Value" + i)?.ToString() ?? string.Empty);
                        item.SubItems.Add(registryPath.GetValue("ValueRaw" + i)?.ToString() ?? string.Empty);
                        // 展示插件实际发布到 Macro Deck 的变量名（去重后的稳定命名）
                        item.SubItems.Add(HWiNFO64Plugin.VariableNames.TryGetValue(i, out var varName) ? varName : string.Empty);
                        item.Font = new Font(item.Font, FontStyle.Regular);
                        listView1.Items.Add(item);
                    }
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
        }
    }

}
