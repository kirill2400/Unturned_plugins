using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace AutoDoor
{
    public class AutoDoor : RocketPlugin<Configuration>
    {
        public void TriggerSend(SteamPlayer player, string name, ESteamCall mode, ESteamPacket type, params object[] arguments)
        {
            if (mode == ESteamCall.SERVER)
                UnturnedChat.Say(name);
            if (mode == ESteamCall.ALL)
                UnturnedChat.Say("All: " + name);
            if (arguments.Length == 4 && name == "askToggleDoor" && mode == ESteamCall.SERVER)
            {
                byte x = (byte)arguments[0];
                byte y = (byte)arguments[1];
                ushort plant = (ushort)arguments[2];
                ushort index = (ushort)arguments[3];

                BarricadeRegion region;
                if (BarricadeManager.tryGetRegion(x, y, plant, out region) && index < region.drops.Count)
                {
                    InteractableDoor interactableDoor = region.drops[index].interactable as InteractableDoor;
                    if ((UnityEngine.Object)interactableDoor != (UnityEngine.Object)null)
                    {
                        UnturnedChat.Say(region.drops[index].instanceID.ToString());
                    }
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
