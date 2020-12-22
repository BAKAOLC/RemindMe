using Dalamud.Configuration;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using RemindMe.Config;

namespace RemindMe
{
    public partial class RemindMeConfig : IPluginConfiguration {
        public uint InstallNoticeDismissed = 0;

        [NonSerialized] private float debugFraction = 0;

        [NonSerialized]
        private DalamudPluginInterface pluginInterface;

        [NonSerialized]
        private RemindMe plugin;

        public int Version { get; set; } = 2;

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None)]
        public Dictionary<Guid, Config.MonitorDisplay> MonitorDisplays = new Dictionary<Guid, MonitorDisplay>();

        [JsonIgnore]
        public List<GeneralReminder> GeneralReminders = new List<GeneralReminder>();

        private bool showGlobalCooldowns;
        public long PollingRate = 100;
        private const int GlobalCooldownGroup = 58;

        public RemindMeConfig() { }

        public void Init(RemindMe plugin, DalamudPluginInterface pluginInterface)
        {
            this.plugin = plugin;
            this.pluginInterface = pluginInterface;
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(GeneralReminder)))) {
                GeneralReminders.Add((GeneralReminder) Activator.CreateInstance(t));
            }

            if (Version == 1) {
                // Update to Version 2
                // Remove Status Monitors with ClassJob of 0
                Version = 2;
                if (MonitorDisplays.Count > 0) {
                    foreach (var a in MonitorDisplays.Values) {
                        a.StatusMonitors.RemoveAll(a => a.ClassJob == 0);
                    }
                }
                Save();
            }
        }

        public void Save()
        {
            pluginInterface.SavePluginConfig(this);
        }
        
        public bool DrawConfigUI() {

            bool drawConfig = true;
            ImGui.SetNextWindowSizeConstraints(new Vector2(400, 400), new Vector2(1200, 1200));
            if (!ImGui.Begin($"{plugin.Name} - Configuration###cooldownMonitorSetup", ref drawConfig)) return drawConfig;
            if (InstallNoticeDismissed != 1) {
                
                ImGui.TextWrapped($"感谢您安装 {plugin.Name}.\n我正在重写插件，如果有任何问题，请立即向我反馈。插件目前相对稳定，但仍然有可能出现突发的错误。");
                
                if (ImGui.SmallButton("消除")) {
                    InstallNoticeDismissed = 1;
                    Save();
                }
                ImGui.Separator();
            }

            ImGui.BeginTabBar("###remindMeConfigTabs");

            if (ImGui.BeginTabItem("显示区")) {
                DrawDisplaysTab();
                ImGui.EndTabItem();
            }

            if (MonitorDisplays.Count > 0) {
                if (ImGui.BeginTabItem("技能")) {
                    DrawActionsTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("状态效果")) {
                    DrawStatusEffectsTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("战斗效果")) {
                    DrawRaidEffectsTab();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("提醒")) {
                    DrawRemindersTab();
                    ImGui.EndTabItem();
                }
            }

#if DEBUG
            if (ImGui.BeginTabItem("Debug")) {
                DrawDebugTab();
                ImGui.EndTabItem();

            }
#endif
            ImGui.EndTabBar();
            ImGui.End();

            return drawConfig;
        }

        private void StatusMonitorConfigDisplay(uint statusId, float maxDuration, string note = null, bool raid = false, bool selfOnly = false, uint[] statusList = null, string forcedName = null, ushort limitedZone = 0) {

            var status = pluginInterface.Data.GetExcelSheet<Status>().GetRow(statusId);
            if (status == null) return;
            var statusMonitor = new StatusMonitor {Status = status.RowId, ClassJob = pluginInterface.ClientState.LocalPlayer.ClassJob.Id, MaxDuration = maxDuration, SelfOnly = selfOnly, StatusList = statusList, IsRaid = raid, LimitedZone = limitedZone};
            
            var statusIcon = plugin.IconManager.GetIconTexture(status.Icon);
            if (statusIcon != null) {
                ImGui.Image(statusIcon.ImGuiHandle, new Vector2(18, 24));
            } else {
                ImGui.Dummy(new Vector2(18, 24));
            }

            if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip(status.Description);
            }

            if (statusList != null) {
                foreach (var s in statusList) {
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 10);
                    var extraStatus = pluginInterface.Data.GetExcelSheet<Status>().GetRow(s);
                    if (extraStatus == null) continue;
                    var extraStatusIcon = plugin.IconManager.GetIconTexture(extraStatus.Icon);
                    if (extraStatusIcon == null) continue;
                    ImGui.Image(extraStatusIcon.ImGuiHandle, new Vector2(18, 24));
                    if (ImGui.IsItemHovered()) {
                        ImGui.SetTooltip(extraStatus.Description);
                    }
                }
            }

            ImGui.SameLine();

            ImGui.Text(forcedName ?? status.Name);

            if (!string.IsNullOrEmpty(note)) {
                ImGui.SameLine();
                ImGui.Text($"({note})");
            }

            if (selfOnly) {
                var selfTextSize = ImGui.CalcTextSize("[自身效果]");
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetColumnWidth() - selfTextSize.X);
                ImGui.TextDisabled("[自身效果]");
            }


            ImGui.NextColumn();

            foreach (var s in MonitorDisplays.Values) {
                var enabled = s.StatusMonitors.Contains(statusMonitor);
                if (ImGui.Checkbox($"###statusToggle{s.Guid}_{status.RowId}", ref enabled)) {
                    if (enabled) {
                        s.StatusMonitors.Add(statusMonitor);
                    } else {
                        s.StatusMonitors.Remove(statusMonitor);
                    }
                    Save();
                }
                ImGui.NextColumn();
            }

            ImGui.Separator();

        }

    }
}