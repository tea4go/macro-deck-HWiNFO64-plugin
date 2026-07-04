using HWiNFO64_Plugin;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using sugoides.HWiNFO64_Plugin.Language;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace sugoides.HWiNFO64_Plugin
{
    public class HWiNFO64Plugin : MacroDeckPlugin
    {
        public override bool CanConfigure => true;

        public static int sensors = 0;

        int refreshTime = 2000;

        readonly Microsoft.Win32.RegistryKey registryPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\HWiNFO64\VSB");

        internal static MacroDeckPlugin Instance { get; set; }

        // 变量名规范化：Macro Deck 变量名只允许字母、数字、下划线，其余字符统一替换为 _
        private static readonly Regex VarNameSanitizer = new Regex("[^A-Za-z0-9_]", RegexOptions.Compiled);

        // 缓存已发布的值，避免值未变化时的无谓 SetValue 广播——这是主程序卡死的关键防线
        private readonly Dictionary<string, string> _lastStringValues = new();
        private readonly Dictionary<string, float> _lastFloatValues = new();

        // 重入保护：Elapsed 在 ThreadPool 线程触发，需要防止上一 tick 未完成时又进入
        private int _tickBusy = 0;

        // 持有 Timer 引用防止被 GC 回收
        private System.Timers.Timer _sensorTimer;

        public HWiNFO64Plugin()
        {
            Instance = this;
        }

        public override void Enable()
        {
            PluginLanguageManager.Initialize();

            if (registryPath != null)
            {
                sensors = 0;
                while (registryPath.GetValue("Label" + sensors) != null)
                {
                    sensors++;
                }
            }

            var refreshTimeFromRegistry = PluginConfiguration.GetValue(HWiNFO64Plugin.Instance, "refreshTime");
            if (int.TryParse(refreshTimeFromRegistry, out refreshTime) == false)
                refreshTime = 2000;

            _sensorTimer = new System.Timers.Timer(refreshTime) { AutoReset = true };
            _sensorTimer.Elapsed += SensorTimer_Elapsed;
            _sensorTimer.Start();
        }

        private void SensorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 重入保护：上一 tick 还没跑完就直接跳过，避免并发写 VariableManager
            if (Interlocked.CompareExchange(ref _tickBusy, 1, 0) != 0) return;
            try
            {
                if (registryPath == null) return;

                for (int i = 0; i < sensors; i++)
                {
                    try
                    {
                        var label = registryPath.GetValue("Label" + i) as string;
                        if (string.IsNullOrEmpty(label)) continue;

                        var value = registryPath.GetValue("Value" + i) as string ?? string.Empty;
                        var valueRawStr = registryPath.GetValue("ValueRaw" + i) as string;

                        var variableName = "hwi64_" + VarNameSanitizer.Replace(label, "_");

                        // 值有变化才发布：显著降低 WebSocket 广播频率
                        if (!_lastStringValues.TryGetValue(variableName, out var cachedStr) || cachedStr != value)
                        {
                            _lastStringValues[variableName] = value;
                            VariableManager.SetValue(variableName, value, VariableType.String, HWiNFO64Plugin.Instance, (string[])null);
                        }

                        // 原始值：注册表存的是字符串，必须解析为 float 后再传，否则类型不匹配会抛异常
                        if (float.TryParse(valueRawStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var raw))
                        {
                            var rawKey = variableName + "_raw";
                            if (!_lastFloatValues.TryGetValue(rawKey, out var cachedRaw) || Math.Abs(cachedRaw - raw) > float.Epsilon)
                            {
                                _lastFloatValues[rawKey] = raw;
                                VariableManager.SetValue(rawKey, raw, VariableType.Float, HWiNFO64Plugin.Instance, (string[])null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 单个传感器失败不影响其余传感器；也不允许异常向上冒泡
                        MacroDeckLogger.Warning(this, "HWiNFO64 sensor [{0}] update failed: {1}", i, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Error(this, "HWiNFO64 tick failed: {0}", ex);
            }
            finally
            {
                Interlocked.Exchange(ref _tickBusy, 0);
            }
        }

        public override void OpenConfigurator()
        {
            using (var configurator = new PluginConfigurationView())
            {
                configurator.ShowDialog();
            }
        }
    }

}
