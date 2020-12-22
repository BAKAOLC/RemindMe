using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using RemindMe.JsonConverters;

namespace RemindMe.Config {

    public class MonitorDisplay {

        [JsonIgnore] private List<DisplayTimer> cachedTimerList;
        [JsonIgnore] private readonly Stopwatch cacheTimerListStopwatch = new Stopwatch();
        [JsonIgnore]
        public List<DisplayTimer> TimerList {
            get {
                if (cachedTimerList == null) return null;
                if (!cacheTimerListStopwatch.IsRunning) return null;
                if (cacheTimerListStopwatch.ElapsedMilliseconds > UpdateInterval) return null;
                return cachedTimerList;
            }
            set {
                cachedTimerList = value;
                cacheTimerListStopwatch.Restart();
            }
        }

        [JsonIgnore]
        public TimeSpan CacheAge => cacheTimerListStopwatch.Elapsed;
        
        private static readonly string[] _displayTypes = new string[] {
            "水平",
            "垂直",
            "图标",
        };

        public bool DirectionRtL = false;
        public bool DirectionBtT = false;
        public bool IconVerticalStack = false;

        public int UpdateInterval = 50;

        public bool Enabled = false;
        public Guid Guid;
        public string Name = "New Display";

        public bool Locked = false;
        public bool AllowClicking = false;

        public bool OnlyShowReady = false;
        public bool OnlyShowCooldown = false;

        public int RowSize = 32;
        public float TextScale = 1;
        public int BarSpacing = 5;

        public bool ShowActionIcon = true;
        public float ActionIconScale = 0.9f;
        public bool ReverseSideIcon = false;

        public bool OnlyInCombat = true;
        public bool KeepVisibleOutsideCombat = false;
        public int KeepVisibleOutsideCombatSeconds = 15;

        public bool ShowSkillName = true;
        public bool ShowStatusEffectTarget = true;
        public bool SkillNameRight = false;
        public bool ShowCountdown = false;
        public bool ShowCountdownReady = false;
        public bool ReverseCountdownSide = false;

        public bool PulseReady = false;
        public float PulseSpeed = 1.0f;
        public float PulseIntensity = 1.0f;

        public bool FillToComplete = false;
        public bool ReverseFill = false;
        public RemindMe.FillDirection IconDisplayFillDirection = RemindMe.FillDirection.FromBottom;

        public bool LimitDisplayTime = false;
        public int LimitDisplayTimeSeconds = 10;

        public bool LimitDisplayReadyTime;
        public int LimitDisplayReadyTimeSeconds = 15;

        public List<CooldownMonitor> Cooldowns = new List<CooldownMonitor>();

        public List<StatusMonitor> StatusMonitors = new List<StatusMonitor>();

        public List<GeneralReminder> GeneralReminders = new List<GeneralReminder>();

        public Vector4 AbilityReadyColor = new Vector4(0.70f, 0.25f, 0.25f, 0.75f);
        public Vector4 AbilityCooldownColor = new Vector4(0.75f, 0.125f, 0.665f, 0.75f);
        public Vector4 StatusEffectColor = new Vector4(1f, 0.5f, 0.1f, 0.75f);
        public Vector4 TextColor = new Vector4(1f, 1f, 1f, 1f);
        public Vector4 BarBackgroundColor = new Vector4(0.3f, 0.3f, 0.3f, 0.5f);

        public int DisplayType = 0;

        [JsonIgnore] private bool tryDelete;
        [JsonIgnore] internal bool IsClickableHovered;
        

        public void DrawConfigEditor(RemindMeConfig mainConfig, RemindMe plugin, ref Guid? deletedMonitor) {
            ImGui.Indent(10);
            if (ImGui.Checkbox($"锁定显示区##{this.Guid}", ref this.Locked)) mainConfig.Save();
            ImGui.SameLine();
            ImGui.SetNextItemWidth(150);
            if (ImGui.InputText($"###displayName{this.Guid}", ref this.Name, 32)) mainConfig.Save();
            if (ImGui.Checkbox($"可点击操作##{this.Guid}", ref this.AllowClicking)) mainConfig.Save();
            ImGui.SameLine();
            ImGui.TextDisabled("一个可点击的显示区允许你点击状态来快捷选中目标\n但它可能会妨碍你执行其他操作。");
            ImGui.Separator();
            if (ImGui.Combo("显示类型", ref DisplayType, _displayTypes, _displayTypes.Length)) mainConfig.Save();
            

            if ((DisplayType == 1 || DisplayType == 2) && ImGui.Checkbox("从右到左", ref DirectionRtL)) mainConfig.Save();
            if ((DisplayType == 0 || DisplayType == 2) && ImGui.Checkbox("从下到上", ref DirectionBtT)) mainConfig.Save();
            if (DisplayType == 2 && ImGui.Checkbox("垂直堆叠", ref IconVerticalStack)) mainConfig.Save();

            ImGui.Separator();
            ImGui.Separator();

            ImGui.Text("列");
            ImGui.Separator();

            if (ImGui.ColorEdit4($"可用技能##{Guid}", ref AbilityReadyColor)) mainConfig.Save();
            if (ImGui.ColorEdit4($"CD中技能##{Guid}", ref AbilityCooldownColor)) mainConfig.Save();
            if (ImGui.ColorEdit4($"状态效果##{Guid}", ref StatusEffectColor)) mainConfig.Save();
            if (ImGui.ColorEdit4($"条背景色##{Guid}", ref BarBackgroundColor)) mainConfig.Save();
            if (ImGui.ColorEdit4($"文本##{Guid}", ref TextColor)) mainConfig.Save();

            ImGui.Separator();
            ImGui.Separator();
            ImGui.Text("显示设置");
            ImGui.Separator();
            if (ImGui.Checkbox($"脱战时隐藏##{this.Guid}", ref this.OnlyInCombat)) mainConfig.Save();

            if (OnlyInCombat) {
                ImGui.Indent(20);
                if (ImGui.Checkbox($"保持显示到脱战后###keepVisibleOutsideCombat{this.Guid}", ref this.KeepVisibleOutsideCombat)) mainConfig.Save();
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                if (ImGui.InputInt($"秒。###keepVisibleOutsideCombatSeconds{this.Guid}", ref KeepVisibleOutsideCombatSeconds)) mainConfig.Save();
                if (KeepVisibleOutsideCombatSeconds < 0) {
                    KeepVisibleOutsideCombatSeconds = 0;
                    mainConfig.Save();
                }
                ImGui.Indent(-20);
            }

            if (ImGui.Checkbox($"不显示可用技能##{this.Guid}", ref this.OnlyShowCooldown)) {
                OnlyShowReady = false;
                mainConfig.Save();
            }
            if (ImGui.Checkbox($"只显示可用技能##{this.Guid}", ref this.OnlyShowReady)) {
                OnlyShowCooldown = false;
                mainConfig.Save();
            }
            if (ImGui.Checkbox($"使用填充模式##{this.Guid}", ref this.FillToComplete)) mainConfig.Save();
            if (DisplayType < 2 && ImGui.Checkbox($"反转填充方向##{this.Guid}", ref this.ReverseFill)) mainConfig.Save();
            if (DisplayType == 2) { 
                ImGui.BeginGroup();
                plugin.DrawBar(ImGui.GetCursorScreenPos(), new Vector2(22, 22), 0.45f, IconDisplayFillDirection, new Vector4(0.3f, 0.3f, 0.3f, 1), new Vector4(0.8f, 0.8f, 0.8f, 1), 3); 
                ImGui.SameLine();
                ImGui.Text("填充方向");
                ImGui.EndGroup();
                if (ImGui.IsItemClicked(0)) {
                   IconDisplayFillDirection = (RemindMe.FillDirection) ((((int) IconDisplayFillDirection) + 1) % Enum.GetValues(typeof(RemindMe.FillDirection)).Length);
                }
            }
            

            if (ImGui.Checkbox($"显示技能图标##{this.Guid}", ref this.ShowActionIcon)) mainConfig.Save();
            if (this.ShowActionIcon) {
                switch (DisplayType) {
                    case 0: {
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(75);
                        var v = ReverseSideIcon ? 1 : 0;
                        var text = ReverseSideIcon ? "右侧" : "左侧";
                        ImGui.SliderInt("###actionIconReverse", ref v, 0, 1, text);
                        if (ImGui.IsItemClicked(0)) ReverseSideIcon = !ReverseSideIcon;
                        break;
                    }
                    case 1: {
                        ImGui.SameLine();
                        var v = ReverseSideIcon ? 1 : 0;
                        var text = ReverseSideIcon ? "上方" : "下方";
                        ImGui.VSliderInt("###actionIconReverse", new Vector2(60, 25), ref v, 0, 1, text);
                        if (ImGui.IsItemClicked(0)) ReverseSideIcon = !ReverseSideIcon;
                        break;
                    }
                }
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                if (ImGui.SliderFloat($"###actionIconScale{this.Guid}", ref this.ActionIconScale, 0.1f, 1f, "缩放", 1f)) mainConfig.Save();

            }

            if ((DisplayType == 0 || DisplayType == 1) && ImGui.Checkbox($"显示技能名称##{this.Guid}", ref this.ShowSkillName)) mainConfig.Save();

            if (DisplayType == 0 && this.ShowSkillName) {
                ImGui.SameLine();
                ImGui.SetNextItemWidth(75);
                var v = SkillNameRight ? 1 : 0;
                var text = SkillNameRight ? "右侧" : "左侧";
                ImGui.SliderInt("###skillNameAlign", ref v, 0, 1, text);
                if (ImGui.IsItemClicked(0)) SkillNameRight = !SkillNameRight;
            }

            if ((DisplayType == 0 || DisplayType == 1) && ShowSkillName && ImGui.Checkbox($"显示状态效果对象名称##{this.Guid}", ref this.ShowStatusEffectTarget)) mainConfig.Save();

            if (ImGui.Checkbox($"显示计时##{this.Guid}", ref this.ShowCountdown)) mainConfig.Save();
            if (ShowCountdown) {

                switch (DisplayType) {
                    case 0: {
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(75);
                        var v = ReverseCountdownSide ? 0 : 1;
                        var text = ReverseCountdownSide ? "左侧" : "右侧";
                        ImGui.SliderInt("###actionCountdownReverse", ref v, 0, 1, text);
                        if (ImGui.IsItemClicked(0)) ReverseCountdownSide = !ReverseCountdownSide;
                        break;
                    }
                    case 1: {
                        ImGui.SameLine();
                        var v = ReverseCountdownSide ? 0 : 1;
                        var text = ReverseCountdownSide ? "下方" : "上方";
                        ImGui.VSliderInt("###countdownReverse", new Vector2(60, 25), ref v, 0, 1, text);
                        if (ImGui.IsItemClicked(0)) ReverseCountdownSide = !ReverseCountdownSide;
                        break;
                    }
                }



                ImGui.Indent(20);
                if (ImGui.Checkbox($"准备就绪时仍然显示计时##{this.Guid}", ref this.ShowCountdownReady)) mainConfig.Save();
                ImGui.Indent(-20);

            }
            if (ImGui.Checkbox($"准备就绪时闪烁##{this.Guid}", ref this.PulseReady)) mainConfig.Save();

            if (this.PulseReady) {

                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                if (ImGui.SliderFloat($"###pulseSpeed{this.Guid}", ref this.PulseSpeed, 0.5f, 2f, "速度", 2f)) mainConfig.Save();
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                if (ImGui.SliderFloat($"###pulseIntensity{this.Guid}", ref this.PulseIntensity, 0.1f, 2f, "强度")) mainConfig.Save();


            }

            ImGui.Separator();
            if (ImGui.Checkbox($"###limitDisplay{this.Guid}", ref this.LimitDisplayTime)) mainConfig.Save();
            ImGui.SameLine();
            ImGui.Text("仅显示剩余");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(90);
            if (ImGui.InputInt($"秒内的效果##limitSeconds{this.Guid}", ref LimitDisplayTimeSeconds)) mainConfig.Save();

            if (ImGui.Checkbox($"###limitDisplayReady{this.Guid}", ref this.LimitDisplayReadyTime)) mainConfig.Save();
            ImGui.SameLine();
            ImGui.Text("不要显示超过");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(90);
            if (ImGui.InputInt($"秒以上CD的技能##limitReadySeconds{this.Guid}", ref LimitDisplayReadyTimeSeconds)) mainConfig.Save();

            ImGui.Separator();
            if (ImGui.InputInt($"条高度##{this.Guid}", ref this.RowSize, 1, 5)) {
                if (this.RowSize < 8) this.RowSize = 8;
                mainConfig.Save();
            }
            if (ImGui.InputInt($"条间距##{this.Guid}", ref this.BarSpacing, 1, 2)) {
                if (this.BarSpacing < 0) this.BarSpacing = 0;
                mainConfig.Save();
            }
            if (ImGui.InputFloat($"文本大小##{this.Guid}", ref this.TextScale, 0.01f, 0.1f)) {
                if (this.RowSize < 8) this.RowSize = 8;
                mainConfig.Save();
            }
            
            if (ImGui.InputInt($"更新间隔##{this.Guid}", ref this.UpdateInterval, 1, 50)) {
                if (this.UpdateInterval < 1) this.UpdateInterval = 1;
                mainConfig.Save();
            }
            
            ImGui.Separator();


            if (tryDelete) {

                ImGui.Text("删除显示区？");
                ImGui.SameLine();
                if (ImGui.Button("不要删除")) tryDelete = false;
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Button, 0x88000088);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0x99000099);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xAA0000AA);
                if (ImGui.Button("删除这个显示区")) deletedMonitor = Guid;
                ImGui.PopStyleColor(3);

            } else {
                ImGui.PushStyleColor(ImGuiCol.Button, 0x88000088);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0x99000099);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xAA0000AA);
                if (ImGui.Button("删除这个显示区")) {
                    tryDelete = true;
                }
                ImGui.PopStyleColor(3);
            }

            

            ImGui.Separator();
            ImGui.Indent(-10);
        }

    }
}
