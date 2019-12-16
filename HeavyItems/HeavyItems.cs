using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace HeavyItems
{
    public class HeavyItems : RocketPlugin<Configuration>
    {
        public static HeavyItems Instance;

        private DateTime m_LastDehydrate = DateTime.Now;
        private DateTime m_LastSave = DateTime.Now;
        private Dictionary<ulong, Coroutine> delayInventory = new Dictionary<ulong, Coroutine>();
        private Dictionary<ulong, Coroutine> delayEffectUI = new Dictionary<ulong, Coroutine>();

        public void FixedUpdate()
        {
            if ((DateTime.Now - m_LastDehydrate).TotalSeconds > 3)
            {
                foreach (SteamPlayer player in Provider.clients)
                    if ((Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.playerID.steamID.m_SteamID)
                        || Configuration.Instance.OverloadedByArmorPlayers.Contains(player.playerID.steamID.m_SteamID)) && player.player != null)
                        player.player.life.askDehydrate(1);
                m_LastDehydrate = DateTime.Now;
            }
            if ((DateTime.Now - m_LastSave).TotalSeconds > Configuration.Instance.DelayAutoSave)
            {
                Configuration.Save();
                Logger.LogWarning("\tConfig has been saved!");
                m_LastSave = DateTime.Now;
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            if (Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID)
                || Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID))
                if (!player.Player.life.isBroken)
                {
                    var prop = player.Player.life.GetType().GetField("lastBroken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    prop.SetValue(player.Player.life, player.Player.input.simulation);
                    player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                    player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                }
        }

        private void UnturnedPlayerEvents_OnPlayerUpdateBroken(UnturnedPlayer player, bool broken)
        {
            if (player.CSteamID == null) return;
            if ((Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID)
                || Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID)) && !broken)
                StartCoroutine(DelayedInvoke(1f, () =>
                {
                    if (player.CSteamID != null)
                        if ((Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID)
                           || Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID)) && !player.Player.life.isBroken)
                        {
                            var prop = player.Player.life.GetType().GetField("lastBroken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            prop.SetValue(player.Player.life, player.Player.input.simulation);
                            player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                            player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                        }
                }));
        }
        private void UnturnedPlayerEvents_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID))
                Configuration.Instance.OverloadedByBackpackPlayers.Remove(player.CSteamID.m_SteamID);
            if (Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID))
                Configuration.Instance.OverloadedByArmorPlayers.Remove(player.CSteamID.m_SteamID);
        }

        private void UnturnedPlayerEvents_OnPlayerWear(UnturnedPlayer player, UnturnedPlayerEvents.Wearables wear, ushort id, byte? quality)
        {
            bool overload = false;
            ushort shirtWear = 0;
            ushort vestWear = 0;
            if (wear == UnturnedPlayerEvents.Wearables.Backpack && id == 0)
            {
                StartCoroutine(DelayedInvoke(1f, () =>
                {
                    if (Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID))
                        Configuration.Instance.OverloadedByBackpackPlayers.Remove(player.CSteamID.m_SteamID);
                    if (!Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID) && player.Player.life.isBroken)
                    {
                        player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, false);
                        player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, false);
                    }
                }));
                return;
            }
            if (wear == UnturnedPlayerEvents.Wearables.Shirt)
            {
                shirtWear = id;
                vestWear = player.Player.clothing.vest;
            }
            else if (wear == UnturnedPlayerEvents.Wearables.Vest)
            {
                shirtWear = player.Player.clothing.shirt;
                vestWear = id;
            }
            
            foreach (short shirt in Configuration.Instance.ShirtID)
            {
                if (shirtWear == shirt)
                {
                    if (!Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID))
                    {
                        Configuration.Instance.OverloadedByArmorPlayers.Add(player.CSteamID.m_SteamID);
                        EffectManager.sendUIEffect(60129, 0, player.CSteamID, false, Configuration.Instance.UIColorArmor, Configuration.Instance.UISizeArmor, Configuration.Instance.UIColorArmor);
                        if (delayEffectUI.ContainsKey(player.CSteamID.m_SteamID))
                        {
                            StopCoroutine(delayEffectUI[player.CSteamID.m_SteamID]);
                            delayEffectUI[player.CSteamID.m_SteamID] = StartCoroutine(DelayedInvoke(15, () =>
                            {
                                EffectManager.askEffectClearByID(60129, player.CSteamID);
                            }));
                        }
                        else
                            delayEffectUI.Add(player.CSteamID.m_SteamID, StartCoroutine(DelayedInvoke(15, () =>
                            {
                                EffectManager.askEffectClearByID(60129, player.CSteamID);
                            })));
                    }
                    if (!player.Player.life.isBroken)
                    {
                        var prop = player.Player.life.GetType().GetField("lastBroken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        prop.SetValue(player.Player.life, player.Player.input.simulation);
                        player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                        player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                    }
                    overload = true;
                    break;
                }
            }
            if (!overload)
                foreach (short vest in Configuration.Instance.VestID)
                {
                    if (vestWear == vest)
                    {
                        if (!Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID))
                        {
                            Configuration.Instance.OverloadedByArmorPlayers.Add(player.CSteamID.m_SteamID);
                            EffectManager.sendUIEffect(60129, 0, player.CSteamID, false, Configuration.Instance.UIColorArmor, Configuration.Instance.UISizeArmor, Configuration.Instance.UIColorArmor);
                            if (delayEffectUI.ContainsKey(player.CSteamID.m_SteamID))
                            {
                                StopCoroutine(delayEffectUI[player.CSteamID.m_SteamID]);
                                delayEffectUI[player.CSteamID.m_SteamID] = StartCoroutine(DelayedInvoke(15, () =>
                                {
                                    EffectManager.askEffectClearByID(60129, player.CSteamID);
                                }));
                            }
                            else
                                delayEffectUI.Add(player.CSteamID.m_SteamID, StartCoroutine(DelayedInvoke(15, () =>
                                {
                                    EffectManager.askEffectClearByID(60129, player.CSteamID);
                                })));
                        }
                        if (!player.Player.life.isBroken)
                        {
                            var prop = player.Player.life.GetType().GetField("lastBroken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            prop.SetValue(player.Player.life, player.Player.input.simulation);
                            player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                            player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                        }
                        overload = true;
                        break;
                    }
                }
            if (!overload && Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID))
            {
                Configuration.Instance.OverloadedByArmorPlayers.Remove(player.CSteamID.m_SteamID);
                if (!Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID) && player.Player.life.isBroken)
                {
                    player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, false);
                    player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, false);
                }
            }
        }

        private void UnturnedPlayerEvents_OnPlayerInventoryAdded(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            if (inventoryGroup == Rocket.Unturned.Enumerations.InventoryGroup.Backpack)
                OverloadPlayer(player, CheckSpace(player.Player.inventory));
        }
        private void UnturnedPlayerEvents_OnPlayerInventoryRemoved(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            if (inventoryGroup == Rocket.Unturned.Enumerations.InventoryGroup.Backpack)
                OverloadPlayer(player, CheckSpace(player.Player.inventory));
        }

        private bool CheckSpace(PlayerInventory inventory)
        {
            var sum = inventory.getHeight(PlayerInventory.BACKPACK) * inventory.getWidth(PlayerInventory.BACKPACK);
            var empty = 0;
            for (byte i = 0; i < inventory.getWidth(PlayerInventory.BACKPACK); i = (byte)(i + 1))
                for (byte j = 0; j < inventory.getHeight(PlayerInventory.BACKPACK); j = (byte)(j + 1))
                    if (inventory.checkSpaceEmpty(PlayerInventory.BACKPACK, i, j, 1, 1, 0))
                        empty++;
            if (empty <= Math.Ceiling(sum * .2))
                return false;
            return true;
        }
        private void OverloadPlayer(UnturnedPlayer player, bool overload)
        {
            if (!overload)
            {
                if (delayInventory.ContainsKey(player.CSteamID.m_SteamID))
                {
                    StopCoroutine(delayInventory[player.CSteamID.m_SteamID]);
                    delayInventory.Remove(player.CSteamID.m_SteamID);
                }
                if (!Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID))
                {
                    Configuration.Instance.OverloadedByBackpackPlayers.Add(player.CSteamID.m_SteamID);
                    EffectManager.sendUIEffect(60129, 0, player.CSteamID, false, Configuration.Instance.UITextBackpack, Configuration.Instance.UISizeBackpack, Configuration.Instance.UIColorBackpack);
                    if (delayEffectUI.ContainsKey(player.CSteamID.m_SteamID))
                    {
                        StopCoroutine(delayEffectUI[player.CSteamID.m_SteamID]);
                        delayEffectUI[player.CSteamID.m_SteamID] = StartCoroutine(DelayedInvoke(7, () =>
                        {
                            EffectManager.askEffectClearByID(60129, player.CSteamID);
                        }));
                    }
                    else
                        delayEffectUI.Add(player.CSteamID.m_SteamID, StartCoroutine(DelayedInvoke(7, () =>
                        {
                            EffectManager.askEffectClearByID(60129, player.CSteamID);
                        })));
                }
                if (!player.Player.life.isBroken)
                {
                    var prop = player.Player.life.GetType().GetField("lastBroken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    prop.SetValue(player.Player.life, player.Player.input.simulation);
                    player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                    player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, true);
                }
            }
            else
            {
                if (!delayInventory.ContainsKey(player.CSteamID.m_SteamID))
                {
                    delayInventory.Add(player.CSteamID.m_SteamID, StartCoroutine(DelayedInvoke(1, () =>
                    {
                        if (Configuration.Instance.OverloadedByBackpackPlayers.Contains(player.CSteamID.m_SteamID))
                            Configuration.Instance.OverloadedByBackpackPlayers.Remove(player.CSteamID.m_SteamID);
                        if (!Configuration.Instance.OverloadedByArmorPlayers.Contains(player.CSteamID.m_SteamID) && player.Player.life.isBroken)
                        {
                            player.Player.channel.send("tellBroken", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, false);
                            player.Player.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, false);
                        }
                    })));
                }
            }
        }

        private IEnumerator DelayedInvoke(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        protected override void Load()
        {
            Instance = this;
            Configuration.Load();
            Logger.LogWarning("\tHeavyItems loaded!");
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerInventoryRemoved += UnturnedPlayerEvents_OnPlayerInventoryRemoved;
            UnturnedPlayerEvents.OnPlayerInventoryAdded += UnturnedPlayerEvents_OnPlayerInventoryAdded;
            UnturnedPlayerEvents.OnPlayerWear += UnturnedPlayerEvents_OnPlayerWear;
            UnturnedPlayerEvents.OnPlayerUpdateBroken += UnturnedPlayerEvents_OnPlayerUpdateBroken;
            UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
        }
        protected override void Unload()
        {
            Instance = null;
            Configuration.Save();
            Logger.LogWarning("\tHeavyItems unloaded!");
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerInventoryRemoved -= UnturnedPlayerEvents_OnPlayerInventoryRemoved;
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= UnturnedPlayerEvents_OnPlayerInventoryAdded;
            UnturnedPlayerEvents.OnPlayerWear -= UnturnedPlayerEvents_OnPlayerWear;
            UnturnedPlayerEvents.OnPlayerUpdateBroken -= UnturnedPlayerEvents_OnPlayerUpdateBroken;
            UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
        }
    }
}
