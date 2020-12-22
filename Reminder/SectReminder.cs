using System.Linq;
using Dalamud.Plugin;
using Newtonsoft.Json;
using RemindMe.Config;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RemindMe.Reminder {
    internal class SectReminder : GeneralReminder {

        [JsonIgnore]
        public override string Name => "学派";

        [JsonIgnore]
        public override string Description => "当你使用占星术士的时候\n提醒你启用学派。";

        public override string GetText(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            return "学派未启用";
        }

        public override bool ShouldShow(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            return pluginInterface.ClientState.LocalPlayer.ClassJob.Id == 33 &&
                   pluginInterface.ClientState.LocalPlayer.Level >= 30 &&
                   pluginInterface.ClientState.LocalPlayer.StatusEffects.All(s => s.EffectId != 839 && s.EffectId != 840);
        }

        public override ushort GetIconID(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            try {
                return pluginInterface.Data.Excel.GetSheet<Action>().GetRow(16559).Icon;
            } catch {
                return 0;
            }
        }

    }
}
