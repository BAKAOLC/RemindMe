using System.Numerics;
using ImGuiNET;

namespace RemindMe {
    public partial class RemindMeConfig {
        public void DrawStatusEffectsTab() {
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
            switch (pluginInterface.ClientState.LocalPlayer.ClassJob.Id) {
                case 20: {
                        // 武僧
                        StatusMonitorConfigDisplay(246, 18); // 破碎拳
                        break;
                    }
                case 21: {
                        // 战士
                        StatusMonitorConfigDisplay(90, 60, selfOnly: true); // 暴风斩
                        break;
                    }
                case 22: {
                        // 龙骑士
                        StatusMonitorConfigDisplay(1914, 30); // 开膛枪
                        StatusMonitorConfigDisplay(118, 24); // 樱花怒放
                        break;
                    }
                case 23: {
                        // 诗人
                        StatusMonitorConfigDisplay(1200, 30); // 烈毒咬箭
                        StatusMonitorConfigDisplay(1201, 30); // 狂风蚀箭
                        break;
                    }
                case 6:
                case 24: {
                        // 白魔
                        StatusMonitorConfigDisplay(143, 18); // 疾风
                        StatusMonitorConfigDisplay(144, 18); // 烈风
                        StatusMonitorConfigDisplay(1871, 30); // 天辉
                        StatusMonitorConfigDisplay(158, 18); // 再生
                        StatusMonitorConfigDisplay(150, 15); // 医济
                        break;
                    }
                case 25: {
                        // 黑魔
                        StatusMonitorConfigDisplay(161, 24); // 闪雷
                        StatusMonitorConfigDisplay(162, 24); // 震雷
                        StatusMonitorConfigDisplay(163, 24); // 暴雷
                        StatusMonitorConfigDisplay(1210, 18); // 霹雷
                        break;
                    }
                case 26:
                case 27: {
                        // 秘术师, 召唤
                        StatusMonitorConfigDisplay(179, 30); // 毒菌
                        StatusMonitorConfigDisplay(180, 30); // 瘴气
                        StatusMonitorConfigDisplay(189, 30); // 猛毒菌
                        StatusMonitorConfigDisplay(1214, 30); // 剧毒菌
                        StatusMonitorConfigDisplay(1215, 30); // 瘴暍
                        break;
                    }
                case 28: {
                        // 学者
                        StatusMonitorConfigDisplay(179, 30); // 毒菌
                        StatusMonitorConfigDisplay(189, 30); // 猛毒菌
                        StatusMonitorConfigDisplay(1895, 30); // 蛊毒法
                        break;
                    }
                case 30: {
                        // 忍者
                        StatusMonitorConfigDisplay(508, 30); // 影牙
                        break;
                    }
                case 31: {
                        // 机工
                        StatusMonitorConfigDisplay(1866, 15); // 毒菌冲击
                        break;
                    }
                case 33: {
                        // 占星
                        StatusMonitorConfigDisplay(838, 30); // 烧灼
                        StatusMonitorConfigDisplay(843, 30); // 炽灼
                        StatusMonitorConfigDisplay(1881, 30); // 焚灼
                        StatusMonitorConfigDisplay(835, 15, "日"); // 吉星相位(日)
                        StatusMonitorConfigDisplay(836, 15, "日"); // 阳星相位(日)
                        StatusMonitorConfigDisplay(1888, 15, "日"); // 天星交错(日)
                        break;
                    }
                case 34: {
                        // 武士
                        StatusMonitorConfigDisplay(1228, 60); // 彼岸花
                        break;
                    }
                default: {
                        ImGui.Columns(1);
                        ImGui.TextWrapped($"没有关于职业[{pluginInterface.ClientState.LocalPlayer.ClassJob.GameData.Name}]的状态可提供监视。");
                        break;
                    }
            }

            ImGui.Columns(1);
            ImGui.TextWrapped("\n有东西缺失了？\n请到 goat place discord 向 Caraxi 反馈，之后它便会被添加。");
            ImGui.EndChild();
            
        }
    }
}
