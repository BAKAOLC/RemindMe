using System.Numerics;
using ImGuiNET;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RemindMe {
    public partial class RemindMeConfig {
        public void DrawRaidEffectsTab() {
            ImGui.Columns(1 + MonitorDisplays.Count, "###statusColumns", false);
            ImGui.SetColumnWidth(0, 220);
            for (var i = 1; i <= MonitorDisplays.Count; i++) {
                ImGui.SetColumnWidth(i, 100);
            }
            ImGui.Text("状态");
            ImGui.NextColumn();
            foreach (var m in MonitorDisplays.Values) {
                ImGui.Text(m.Name);
                ImGui.NextColumn();
            }

            ImGui.Separator();
            ImGui.Separator();
            ImGui.Columns(1);
            ImGui.BeginChild("###scrolling", new Vector2(-1));
            ImGui.Columns(1 + MonitorDisplays.Count, "###statusColumns", false);
            ImGui.SetColumnWidth(0, 220);
            for (var i = 1; i <= MonitorDisplays.Count; i++) {
                ImGui.SetColumnWidth(i, 100);
            }

            StatusMonitorConfigDisplay(638, 15, raid: true, note: pluginInterface.Data.GetExcelSheet<Action>().GetRow(2258)?.Name); // 目标 / 受伤加重 (忍者)

            StatusMonitorConfigDisplay(1221, 15, raid: true); // 目标 / 连环计 (学者)

            StatusMonitorConfigDisplay(1213, 15, raid: true, selfOnly: true); // 玩家 / 灵护 (召唤)

            StatusMonitorConfigDisplay(786, 20, raid: true, selfOnly: true); // 玩家 / 战斗连祷 (龙骑)

            StatusMonitorConfigDisplay(1184, 20, raid: true, selfOnly: true, statusList: new uint[] { 1183 },
                forcedName: pluginInterface.Data.GetExcelSheet<Action>().GetRow(7398)?.Name); // 玩家 / 巨龙视线 (龙骑)

            StatusMonitorConfigDisplay(1185, 20, raid: true, selfOnly: true); // 玩家 / 义结金兰：攻击 (武僧)

            StatusMonitorConfigDisplay(1297, 20, raid: true, selfOnly: true); // 玩家 / 鼓励 (赤魔)

            StatusMonitorConfigDisplay(1202, 20, raid: true, selfOnly: true); // 玩家 / 大地神的抒情恋歌 (诗人)

            StatusMonitorConfigDisplay(1876, 20, raid: true, selfOnly: true,
                statusList: new uint[] { 1882, 1884, 1885 }, forcedName: "近战卡"); // 玩家 / 王冠之领主 (占星)
            StatusMonitorConfigDisplay(1877, 20, raid: true, selfOnly: true,
                statusList: new uint[] { 1883, 1886, 1887 }, forcedName: "远程卡"); // 玩家 / 王冠之贵妇 (占星)

            ImGui.Columns(1);

            ImGui.TextWrapped("\n有东西缺失了？\n请到 goat place discord 向 Caraxi 反馈，之后它便会被添加。");

            ImGui.EndChild();
        }
    }
}
