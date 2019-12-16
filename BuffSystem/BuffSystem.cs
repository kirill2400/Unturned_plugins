using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using Logger = Rocket.Core.Logging.Logger;


namespace BuffSystem
{
    public class BuffSystem : RocketPlugin<Configuration>
    {
        static public BuffSystem Instance;
        static public BuffManager Manager;

        private List<SteamPlayer> m_Players;
        private List<Elexir> m_Elexirs;
        private DateTime m_LastCheck = DateTime.Now;
        private DateTime m_LastSave = DateTime.Now;

        public void FixedUpdate()
        {
            if ((DateTime.Now - m_LastCheck).TotalSeconds > 1)
            {
                if (m_Players.Count == 0) return;
                foreach (SteamPlayer player in m_Players)
                    Manager.CheckEndBuff(UnturnedPlayer.FromSteamPlayer(player));
                m_LastCheck = DateTime.Now;
            }
            if ((DateTime.Now - m_LastSave).TotalSeconds > Configuration.Instance.DelaySaveBuffs)
            {
                Manager.SaveBuffsToXML();
                Logger.LogWarning("\tConfig has been saved!");
                m_LastSave = DateTime.Now;
            }
        }

        public void TriggerSend(SteamPlayer player, string name, ESteamCall mode, ESteamPacket type, params object[] arguments)
        {
            if (arguments.Length == 1 && name == "askConsume" && mode == ESteamCall.NOT_OWNER && Convert.ToInt32(arguments[0]) == 0)
            {
                foreach (Elexir elix in m_Elexirs)
                    elix.CheckItem(UnturnedPlayer.FromSteamPlayer(player));
                foreach (HealElixirXML elix in Configuration.Instance.HealElexirs)
                    elix.CheckItem(UnturnedPlayer.FromSteamPlayer(player));
            }
        }

        public void LoadItems(IAsset<Configuration> config)
        {
            m_Elexirs.Clear();
            foreach (ElixirXML el in config.Instance.Elixirs)
                m_Elexirs.Add(new Elexir(el.ItemID, el.Time, el.Skills, el.BoostLevel));
        }
        public void LoadBuffs(IAsset<Configuration> config)
        {
            Manager.ClearBuffs();
            foreach (BuffSkillXML bu in config.Instance.BuffSkills)
                Manager.AddBuff(bu.Time, bu.SteamID, bu.BoostLevel.Select(x => { return (byte)x; }).ToArray(), Elexir.ConvertSkill(bu.Skills));
            Manager.AddBuff(config.Instance.BuffHeal);
        }
        protected override void Load()
        {
            BuffSystem.Instance = this;
            BuffSystem.Manager = new BuffManager();
            m_Players = new List<SteamPlayer>();
            m_Elexirs = new List<Elexir>();
            U.Events.OnPlayerConnected += player => m_Players.Add(player.SteamPlayer());
            U.Events.OnPlayerDisconnected += player => m_Players.Remove(player.SteamPlayer());
            SteamChannel.onTriggerSend += TriggerSend;
            Logger.LogWarning("\tBuffSystem loaded!");
            Configuration.Load(LoadItems);
            Configuration.Load(LoadBuffs);
        }
        protected override void Unload()
        {
            Manager.SaveBuffsToXML();
            Instance = null;
            Manager.ClearBuffs();
            Manager = null;
            m_Players = null;
            m_Elexirs = null;
            SteamChannel.onTriggerSend -= TriggerSend;
            Logger.LogWarning("\tBuffSystem unloaded!");
        }
    }
}
