using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace HealingStaff
{
    public class HealingStaff : RocketPlugin<Configuration>
    {
        public void TriggerSend(SteamPlayer player, string name, ESteamCall mode, ESteamPacket type, params object[] arguments)
        {
            if (arguments.Length == 1 && name == "askSwing" && mode == ESteamCall.NOT_OWNER)
            {
                var staff = Configuration.Instance;
                var id = player.player.equipment.itemID;
                var mod = (ESwingMode)Convert.ToInt32(arguments[0]);
                if (id == staff.HealItemID && mod == ESwingMode.STRONG)
                {
                    Ray ray = new Ray(player.player.look.aim.position, player.player.look.aim.forward);
                    RaycastInfo raycastInfo = DamageTool.raycast(ray, staff.HealDistance, RayMasks.DAMAGE_CLIENT);
                    if ((UnityEngine.Object)raycastInfo.player != (UnityEngine.Object)null)
                        if (raycastInfo.player != player.player)
                            raycastInfo.player.life.askHeal((byte)staff.HealAmount, staff.HealBleeding, staff.HealBroken);
                }
            }
        }

        protected override void Load()
        {
            SteamChannel.onTriggerSend += TriggerSend;
            Logger.LogWarning("\tTestPlugin loaded!");
        }
        protected override void Unload()
        {
            SteamChannel.onTriggerSend -= TriggerSend;
            Logger.LogWarning("\tTestPlugin unloaded!");
        }
    }
}
