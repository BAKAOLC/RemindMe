using System;
using System.Numerics;
using ImGuiNET;
using RemindMe.Config;

namespace RemindMe {
    public partial class RemindMeConfig {
        public void DrawDisplaysTab() {
            ImGui.BeginChild("###displaysScroll", ImGui.GetWindowSize() - (ImGui.GetStyle().WindowPadding * 2) - new Vector2(0, ImGui.GetCursorPosY()));
            if (ImGui.Button("添加新的显示区域")) {
                var guid = Guid.NewGuid();
                MonitorDisplays.Add(guid, new MonitorDisplay { Guid = guid, Name = $"Display {MonitorDisplays.Count + 1}" });
                Save();
            }

            if (MonitorDisplays.Count == 0) {
                ImGui.Text("添加一个显示区域来开始使用。");
            }

            Guid? deletedMonitor = null;
            foreach (var m in MonitorDisplays.Values) {
                if (ImGui.CollapsingHeader($"{m.Name}###configDisplay{m.Guid}")) {
                    m.DrawConfigEditor(this, plugin, ref deletedMonitor);
                }
            }

            if (deletedMonitor.HasValue) {
                MonitorDisplays.Remove(deletedMonitor.Value);
                Save();
            }
            ImGui.EndChild();
        }
    }
}
