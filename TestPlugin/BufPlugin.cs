using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using UnityEngine;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Rocket.Unturned;
using Rocket.Unturned.Skills;
using Logger = Rocket.Core.Logging.Logger;


namespace BuffSystem
{
    public class BuffSystem : RocketPlugin<Configuration>
    {
        static public BuffSystem Instance;
        static public BuffManager Manager;

        private List<UnturnedPlayer> m_Players;
        private List<Elexir> m_Elexirs;
        private DateTime m_LastCheck = DateTime.Now;
        private DateTime m_LastSave = DateTime.Now;

        public void FixedUpdate()
        {
            if (m_Players.Count == 0) return;
            UnturnedChat.Say(m_Players.Count.ToString());
            m_Players[0].Heal(100); // Delete
            if ((DateTime.Now - m_LastCheck).TotalSeconds > Configuration.Instance.DelayCheck)
            {
                foreach (UnturnedPlayer player in m_Players.Where(x => !x.Dead))
                    foreach (Elexir elex in m_Elexirs)
                        elex.CheckItem(player);
                foreach (UnturnedPlayer player in m_Players)
                    Manager.CheckEndBuff(player);
                m_LastCheck = DateTime.Now;
            }
            if ((DateTime.Now - m_LastSave).TotalSeconds > Configuration.Instance.DelaySaveBuffs)
            {
                Manager.SaveBuffsToXML();
                Logger.LogWarning("Config has been saved!");
                m_LastSave = DateTime.Now;
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            m_Players.Add(player);
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            m_Players.Remove(m_Players.Single(x => x.CSteamID == player.CSteamID));
        }

        protected override void Load()
        {
            BuffSystem.Instance = this;
            BuffSystem.Manager = new BuffManager();
            m_Players = new List<UnturnedPlayer>();
            m_Elexirs = new List<Elexir>();
            U.Events.OnPlayerConnected += player => m_Players.Add(player);
            U.Events.OnPlayerDisconnected += player => m_Players.Remove(player);
            Logger.LogWarning("\tTestPlugin loaded!");
            Configuration.Load(LoadItems);
            Configuration.Load(LoadBuffs);
        }

        public void LoadItems(IAsset<Configuration> config)
        {
            m_Elexirs.Clear();
            foreach (ElixirXML el in config.Instance.Elixirs)
                m_Elexirs.Add(new Elexir(el.ItemID, el.Time, el.Skills, el.BoostLevel));
            Logger.Log("\tItems reloaded!");
        }

        public void LoadBuffs(IAsset<Configuration> config)
        {
            Manager.ClearBuffs();
            foreach (BuffXML bu in config.Instance.Buffs)
                Manager.AddBuff(bu.BoostLevel.Select(x => { return (byte)x; }).ToArray(), bu.Time, bu.SteamID, Elexir.ConvertSkill(bu.Skills));
            Logger.Log("\tBuffs reloaded!");
        }

        protected override void Unload()
        {
            Manager.SaveBuffsToXML();
            BuffSystem.Instance = null;
            Manager.ClearBuffs();
            BuffSystem.Manager = null;
            m_Players = null;
            m_Elexirs = null;
            Logger.LogWarning("\tTestPlugin unloaded!");
        }
    }

    public class BuffManager
    {
        private List<Buff> buffs;

        public BuffManager()
        {
            buffs = new List<Buff>();
        }

        public void AddBuff(byte[] boostLevel, int time, ulong SteamId, UnturnedSkill[] skill)
        {
            buffs.Add(new Buff(boostLevel, time, SteamId, skill));
        }

        public void CheckEndBuff(UnturnedPlayer player)
        {
            if (buffs.Count == 0) return;
            foreach (var x in buffs)
            {
                if (x.SteamId == player.CSteamID.m_SteamID)
                {
                    if (!player.Dead && x.time > 0)
                        x.time--;
                    UnturnedChat.Say(x.time.ToString());
                    if (x.time == 0 || player.Dead)
                    {
                        x.time = -1;
                        Logger.Log("Done");
                        for (int t = 0; t < x.skill.Length; t++)
                        {
                            var skilllevel = player.GetSkill(x.skill[t]).level;
                            if (skilllevel < x.boostLevel[t])
                                player.SetSkillLevel(x.skill[t], 0);
                            else
                                player.SetSkillLevel(x.skill[t], (byte)(skilllevel - x.boostLevel[t]));
                        }
                    }
                }
            }
            buffs = buffs.Where(x => x.time >= 0).ToList();
        }

        public void SaveBuffsToXML()
        {
            BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
            BuffSystem.Instance.Configuration.Instance.Buffs = new BuffXML[buffs.Count]; 
            for (int i = 0; i < buffs.Count; i ++)
            {
                BuffSystem.Instance.Configuration.Instance.Buffs[i] = new BuffXML(buffs[i].boostLevel.Select(x => { return (ushort)x; }).ToArray(), buffs[i].time, buffs[i].SteamId, Elexir.ConvertSkill(buffs[i].skill));
            }
            BuffSystem.Instance.Configuration.Save();
        }

        public int Count()
        {
            return buffs.Count;
        }

        public void ClearBuffs()
        {
            buffs.Clear();
        }

        private class Buff
        {
            public byte[] boostLevel;
            public int time;
            public ulong SteamId;
            public UnturnedSkill[] skill;

            public Buff(byte[] boostLevel, int time, ulong SteamId, UnturnedSkill[] skill)
            {
                this.boostLevel = boostLevel;
                this.time = time;
                this.SteamId = SteamId;
                this.skill = skill;
            }
        }
    }
}
