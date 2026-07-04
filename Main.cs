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

        // 传感器索引 → 最终变量名。Enable() 扫描时一次性建立，之后各 tick 只查表。
        // 供配置界面读取，让用户直接看到实际使用的变量名。
        public static readonly Dictionary<int, string> VariableNames = new();

        // 传感器索引 → 变量描述（Sensor + Label 组合），写入到 Macro Deck Variable.Description
        public static readonly Dictionary<int, string> VariableDescriptions = new();

        // 记录已设置过描述的变量名，避免每次 tick 重复写 Description
        private readonly HashSet<string> _descriptionsApplied = new();

        int refreshTime = 2000;

        readonly Microsoft.Win32.RegistryKey registryPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\HWiNFO64\VSB");

        internal static MacroDeckPlugin Instance { get; set; }

        // 变量名规范化：保留所有 Unicode 字母/数字/下划线（含中日韩字符），其他字符统一替换为 _
        // .NET Regex 默认支持 \p{L}（Unicode 字母类别）与 \p{N}（数字类别）
        private static readonly Regex VarNameSanitizer = new Regex(@"[^\p{L}\p{N}_]+", RegexOptions.Compiled);
        // 折叠连续下划线为单个下划线，避免变量名出现 __ 或更多
        private static readonly Regex UnderscoreCollapser = new Regex("_{2,}", RegexOptions.Compiled);

        private readonly Dictionary<string, string> _lastStringValues = new();
        private readonly Dictionary<string, float> _lastFloatValues = new();

        private int _tickBusy = 0;
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

                BuildVariableNameMap();
                CleanupOrphanVariables();
            }

            var refreshTimeFromRegistry = PluginConfiguration.GetValue(HWiNFO64Plugin.Instance, "refreshTime");
            if (int.TryParse(refreshTimeFromRegistry, out refreshTime) == false)
                refreshTime = 2000;

            _sensorTimer = new System.Timers.Timer(refreshTime) { AutoReset = true };
            _sensorTimer.Elapsed += SensorTimer_Elapsed;
            _sensorTimer.Start();
        }

        /// <summary>
        /// 扫描所有传感器，建立稳定的变量名映射：
        /// - Label 唯一 → hwi64_{Label}
        /// - Label 冲突 → hwi64_{Label}_{SensorDiscriminator}
        ///   优先从 Sensor 名末尾的 " - XXX" 提取；无则用最后一段 ": XXX"；再无回退整段清洗。
        /// - 万一去重后仍冲突（极端情况），追加数字尾缀。
        /// </summary>
        private void BuildVariableNameMap()
        {
            VariableNames.Clear();
            VariableDescriptions.Clear();
            _descriptionsApplied.Clear();

            // 收集所有传感器的原始 Label 与 Sensor
            var entries = new List<(int Index, string Label, string Sensor)>();
            var labelCount = new Dictionary<string, int>();

            for (int i = 0; i < sensors; i++)
            {
                var label = registryPath.GetValue("Label" + i) as string ?? string.Empty;
                var sensor = registryPath.GetValue("Sensor" + i) as string ?? string.Empty;
                entries.Add((i, label, sensor));

                var key = Sanitize(label);
                labelCount[key] = labelCount.TryGetValue(key, out var c) ? c + 1 : 1;
            }

            // 生成变量名，冲突项追加 Sensor 尾部标识
            var used = new HashSet<string>();
            foreach (var (index, label, sensor) in entries)
            {
                var baseName = "hwi64_" + Sanitize(label);
                string finalName;

                if (labelCount[Sanitize(label)] == 1)
                {
                    finalName = baseName;
                }
                else
                {
                    var discriminator = ExtractDiscriminator(sensor);
                    finalName = string.IsNullOrEmpty(discriminator)
                        ? $"{baseName}_{index}"
                        : $"{baseName}_{discriminator}";
                }

                // 极端情况：加了标识仍冲突，追加数字直到唯一
                var uniqueName = finalName;
                int suffix = 2;
                while (!used.Add(uniqueName))
                {
                    uniqueName = $"{finalName}_{suffix++}";
                }

                VariableNames[index] = uniqueName;
                // 描述用 "Label — Sensor" 组合，保留原始可读文本，与变量名的清洗结果分离
                VariableDescriptions[index] = string.IsNullOrEmpty(sensor)
                    ? label
                    : $"{label} — {sensor}";
            }
        }

        /// <summary>
        /// 清理注册表里已不存在的旧变量（例如变量名规则变更后遗留的 hwi64_____ 之类）。
        /// 保留当前 VariableNames 映射中的所有变量（及其 _raw 变体）。
        /// </summary>
        private void CleanupOrphanVariables()
        {
            try
            {
                var expected = new HashSet<string>();
                foreach (var name in VariableNames.Values)
                {
                    expected.Add(name);
                    expected.Add(name + "_raw");
                }

                var existing = VariableManager.GetVariables(this);
                if (existing == null) return;

                var toDelete = new List<string>();
                foreach (var v in existing)
                {
                    if (!expected.Contains(v.Name)) toDelete.Add(v.Name);
                }
                foreach (var name in toDelete)
                {
                    VariableManager.DeleteVariable(name);
                }
                if (toDelete.Count > 0)
                {
                    MacroDeckLogger.Information(this, "Cleaned up {0} orphan variable(s): {1}", toDelete.Count, string.Join(", ", toDelete));
                }
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(this, "Cleanup orphan variables failed: {0}", ex.Message);
            }
        }

        /// <summary>
        /// 首次为某个变量设置描述后记入 _descriptionsApplied，避免每个 tick 重复写。
        /// </summary>
        private void ApplyDescriptionOnce(string variableName, string description)
        {
            if (_descriptionsApplied.Contains(variableName)) return;
            try
            {
                var v = VariableManager.GetVariable(this, variableName);
                if (v != null)
                {
                    v.Description = description;
                    _descriptionsApplied.Add(variableName);
                }
            }
            catch { /* 描述写失败不影响主流程 */ }
        }

        /// <summary>
        /// 从 Sensor 名提取有区分度的短标识。
        /// 例：
        ///   "网络: Intel Wi-Fi 6 AX200 160MHz - WLAN"  → "WLAN"
        ///   "网络: Intel Ethernet Controller I226-V - NET2"  → "NET2"
        ///   "CPU [#0]: Intel Core i9-13900HK: DTS"  → "DTS"
        ///   "系统: AZW GTi"  → "AZW_GTi"
        /// </summary>
        private static string ExtractDiscriminator(string sensor)
        {
            if (string.IsNullOrWhiteSpace(sensor)) return string.Empty;

            // 优先取 " - " 之后的段（网卡习惯把接口标识放这里）
            int dash = sensor.LastIndexOf(" - ", StringComparison.Ordinal);
            if (dash >= 0)
            {
                var tail = sensor.Substring(dash + 3).Trim();
                if (!string.IsNullOrEmpty(tail)) return Sanitize(tail);
            }

            // 其次取最后一个 ": " 之后的段
            int colon = sensor.LastIndexOf(": ", StringComparison.Ordinal);
            if (colon >= 0)
            {
                var tail = sensor.Substring(colon + 2).Trim();
                if (!string.IsNullOrEmpty(tail)) return Sanitize(tail);
            }

            // 回退：整段清洗
            return Sanitize(sensor);
        }

        private static string Sanitize(string raw)
        {
            var replaced = VarNameSanitizer.Replace(raw ?? string.Empty, "_");
            return UnderscoreCollapser.Replace(replaced, "_").Trim('_');
        }

        private void SensorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.CompareExchange(ref _tickBusy, 1, 0) != 0) return;
            try
            {
                if (registryPath == null) return;

                for (int i = 0; i < sensors; i++)
                {
                    try
                    {
                        if (!VariableNames.TryGetValue(i, out var variableName)) continue;
                        VariableDescriptions.TryGetValue(i, out var description);

                        var value = registryPath.GetValue("Value" + i) as string ?? string.Empty;
                        var valueRawStr = registryPath.GetValue("ValueRaw" + i) as string;

                        if (!_lastStringValues.TryGetValue(variableName, out var cachedStr) || cachedStr != value)
                        {
                            _lastStringValues[variableName] = value;
                            VariableManager.SetValue(variableName, value, VariableType.String, HWiNFO64Plugin.Instance, (string[])null);
                            ApplyDescriptionOnce(variableName, description);
                        }

                        if (float.TryParse(valueRawStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var raw))
                        {
                            var rawKey = variableName + "_raw";
                            if (!_lastFloatValues.TryGetValue(rawKey, out var cachedRaw) || Math.Abs(cachedRaw - raw) > float.Epsilon)
                            {
                                _lastFloatValues[rawKey] = raw;
                                VariableManager.SetValue(rawKey, raw, VariableType.Float, HWiNFO64Plugin.Instance, (string[])null);
                                ApplyDescriptionOnce(rawKey, string.IsNullOrEmpty(description) ? "(raw)" : description + " (raw)");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
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
