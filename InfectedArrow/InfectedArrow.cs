using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace InfectedArrow
{
    public class InfectedArrow : RocketPlugin<Configuration>
    {
        private DateTime m_LastCall = DateTime.Now;
        private DateTime m_LastInfect = DateTime.Now;
        private DateTime m_LastSave = DateTime.Now;

        private Dictionary<ulong, Coroutine> delayEffectUI = new Dictionary<ulong, Coroutine>();

        public void FixedUpdate()
        {
            if ((DateTime.Now - m_LastCall).TotalSeconds > 1)
            {
                Configuration.Instance.InfectedPlayers = Configuration.Instance.InfectedPlayers.Select(x => { if (x.Time < 10) x.Time++; return x; }).ToList();
                m_LastCall = DateTime.Now;
            }
            if ((DateTime.Now - m_LastInfect).TotalSeconds > 2)
            {
                var tempList = Configuration.Instance.InfectedPlayers.Where(x => x.Time >= 10).ToList();
                foreach (SteamPlayer player in Provider.clients)
                {
                    if (tempList.Exists(x => x.SteamId == player.playerID.steamID.m_SteamID))
                    {
                        var tempInfectedPlayer = tempList.Single(x => x.SteamId == player.playerID.steamID.m_SteamID);
                        if (tempInfectedPlayer.Time == 10)
                        {
                            if (delayEffectUI.ContainsKey(player.playerID.steamID.m_SteamID))
                            {
                                StopCoroutine(delayEffectUI[player.playerID.steamID.m_SteamID]);
                                delayEffectUI[player.playerID.steamID.m_SteamID] = StartCoroutine(DelayedInvoke(15, () =>
                                {
                                    EffectManager.askEffectClearByID(60129, player.playerID.steamID);
                                }));
                            }
                            else
                                delayEffectUI.Add(player.playerID.steamID.m_SteamID, StartCoroutine(DelayedInvoke(15, () =>
                                {
                                    EffectManager.askEffectClearByID(60129, player.playerID.steamID);
                                })));
                            EffectManager.sendUIEffect(60129, 0, player.playerID.steamID, false, Configuration.Instance.UITextInfect, Configuration.Instance.UISizeInfect, Configuration.Instance.UIColorInfect);
                            tempInfectedPlayer.Time++;
                        }
                        player.player.life.askInfect(1);
                    }
                }
                m_LastInfect = DateTime.Now;
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
            player.Player.life.onHurt += Life_onHurt;
        }
        private void Life_onHurt(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            if (cause == EDeathCause.GUN)
            {
                UnturnedPlayer infectPlayer = UnturnedPlayer.FromPlayer(player);
                if (!Configuration.Instance.InfectedPlayers.Exists(x => x.SteamId == infectPlayer.CSteamID.m_SteamID))
                    Configuration.Instance.InfectedPlayers.Add(new InfectedPlayer(infectPlayer.CSteamID.m_SteamID));
            }
        }
        public void TriggerSend(SteamPlayer player, string name, ESteamCall mode, ESteamPacket type, params object[] arguments)
        {
            if (arguments.Length == 1 && name == "askConsume" && mode == ESteamCall.NOT_OWNER && player.player.equipment.asset is ItemMedicalAsset)
            {
                var mod = (EConsumeMode)Convert.ToInt32(arguments[0]);
                if (mod == EConsumeMode.USE)
                    Configuration.Instance.InfectedPlayers.RemoveAll(x => x.SteamId == player.playerID.steamID.m_SteamID);
                else if (mod == EConsumeMode.AID)
                {
                    Ray ray = new Ray(player.player.look.aim.position, player.player.look.aim.forward);
                    RaycastInfo raycastInfo = DamageTool.raycast(ray, 3f, RayMasks.DAMAGE_CLIENT);
                    if ((UnityEngine.Object)raycastInfo.player != (UnityEngine.Object)null)
                        if (raycastInfo.player != player.player)
                            Configuration.Instance.InfectedPlayers.RemoveAll(x => x.SteamId == UnturnedPlayer.FromPlayer(raycastInfo.player).CSteamID.m_SteamID);
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
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            SteamChannel.onTriggerSend += TriggerSend;
            Logger.LogWarning("\tInfectedArrow loaded!");
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            SteamChannel.onTriggerSend -= TriggerSend;
            Logger.LogWarning("\tInfectedArrow unloaded!");
        }
    }
}
