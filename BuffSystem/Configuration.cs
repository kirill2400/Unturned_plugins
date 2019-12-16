using Rocket.API;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BuffSystem
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int DelaySaveBuffs = 300;
        public HealElixirXML[] HealElexirs;
        public ElixirXML[] Elixirs;
        public List<BuffHeal> BuffHeal;
        public List<BuffSkillXML> BuffSkills;

        public void LoadDefaults()
        {
            HealElexirs = new HealElixirXML[] { new HealElixirXML() };
            Elixirs = new ElixirXML[] { new ElixirXML() };
            BuffHeal = new List<BuffHeal>() { new BuffHeal() };
            BuffSkills = new List<BuffSkillXML>() { new BuffSkillXML() };
        }
    }

    [Serializable]
    public class HealElixirXML
    {
        public ushort ItemID = 999;
        public int Power = 2;
        public int Time = 30;
        public int Rate = 2;

        public HealElixirXML() { }
        public void CheckItem(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            var equip = player.Player.equipment;
            if (equip.itemID != ItemID) return;
            BuffSystem.Manager.AddBuff(Time, player.CSteamID.m_SteamID, Power, Rate);
        }
    }

    [Serializable]
    public class ElixirXML
    {
        public ushort ItemID = 999;
        public int Time = 30;
        [XmlArrayItem(ElementName = "Skill")]
        public string[] Skills = new string[] { "Overkill", "Fishing" };
        [XmlArrayItem(ElementName = "Level")]
        public ushort[] BoostLevel = new ushort[] { 2, 1 };
    }

    [Serializable]
    public class BuffHeal: BuffManager.Buff,ICloneable
    {
        public int Power;
        public int Rate;
        public int ElapsedRate;

        public BuffHeal()
        {
            this.SteamId = 123;
            this.Time = 30;
            this.Power = 2;
            this.Rate = 2;
            this.Rate = 2;
        }
        public BuffHeal(ulong SteamId, int time, int power, int rate)
        {
            this.SteamId = SteamId;
            this.Time = time;
            this.Power = power;
            this.Rate = rate;
            this.ElapsedRate = rate;
        }

        public object Clone()
        {
            return new BuffHeal(SteamId, Time, Power, Rate);
        }
    }

    [Serializable]
    public class BuffSkillXML
    {
        [XmlArrayItem(ElementName = "Level")]
        public ushort[] BoostLevel;
        public int Time;
        public ulong SteamID;
        [XmlArrayItem(ElementName = "Skill")]
        public string[] Skills;

        public BuffSkillXML()
        {
            BoostLevel = new ushort[] {1, 1};
            Time = 30;
            SteamID = 123467890;
            Skills = new string[] {"Cooking", "Crafting"};
        }

        public BuffSkillXML(ushort[] BoostLevel, int Time, ulong SteamID, string[] Skill)
        {
            this.BoostLevel = BoostLevel;
            this.Time = Time;
            this.SteamID = SteamID;
            this.Skills = Skill;
        }
    }
}
