using System;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Plugin;
using Newtonsoft.Json;
using RemindMe.Config;

namespace RemindMe.Reminder {
    internal class HotbarLockReminder : GeneralReminder {

        [JsonIgnore]
        public override string Name => "热键栏锁";

        [JsonIgnore]
        public override string Description => "提醒你锁定热键栏。\n(不包含十字热键栏)";

        public override string GetText(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            return "热键栏未锁定";
        }

        public override bool ShouldShow(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            var actionBar = pluginInterface.Framework.Gui.GetUiObjectByName("_ActionBar", 1);
            if (actionBar == IntPtr.Zero) return false;
            return Marshal.ReadByte(actionBar, 0x23F) == 0;
        }

        public override ushort GetIconID(DalamudPluginInterface pluginInterface, RemindMe plugin, MonitorDisplay display) {
            return 5; //60840;
        }

    }
}
