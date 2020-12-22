using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;
using Newtonsoft.Json;
using RemindMe.Config;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RemindMe.Reminder {
    internal class TankStanceReminder : GeneralReminder {

        private readonly uint[] tankStatusEffectIDs = { 79, 91, 743, 1833 };

        private readonly Dictionary<uint, uint> TankStanceActions = new Dictionary<uint, uint>() {
            { 1, 28 },
            { 3, 48 },
            { 19, 28 },
            { 21, 48 },
            { 32, 3629 },
            { 37, 16142 }
        };

        [JsonIgnore]
        public override string Name => "盾姿";

        [JsonIgnore]
        public override string Description => "当场地内没有其他坦克职业开启盾姿时\n提醒你开启盾姿。";

        public override string GetText(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            try {
                var action = pluginInterface.Data.Excel.GetSheet<Action>().GetRow(TankStanceActions[pluginInterface.ClientState.LocalPlayer.ClassJob.Id]);
                return $"盾姿未开启: {action.Name}";
            } catch {
                return "盾姿未开启";
            }
        }

        public override bool ShouldShow(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            try {
                if (pluginInterface.ClientState.LocalPlayer.ClassJob.GameData.Role != 1) return false;
                // Check have stance
                if (pluginInterface.ClientState.LocalPlayer.StatusEffects.Any(s => tankStatusEffectIDs.Contains((uint) s.EffectId))) return false;
                // Check other tanks have stance


                foreach (var a in pluginInterface.ClientState.Actors) {
                    if (!(a is PlayerCharacter pc)) continue;
                    if (pc.ClassJob.GameData.Role != 1 || pc.ActorId == pluginInterface.ClientState.LocalPlayer.ActorId) continue;
                    if (pc.StatusEffects.Any(s => tankStatusEffectIDs.Contains((uint) s.EffectId))) return false;
                }
                return true;
            } catch {
                return false;
            }

        }

        public override ushort GetIconID(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            try {
                var action = pluginInterface.Data.Excel.GetSheet<Action>().GetRow(TankStanceActions[pluginInterface.ClientState.LocalPlayer.ClassJob.Id]);
                return action.Icon;
            } catch {
                return 0;
            }
        }

    }
}
