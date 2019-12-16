using Rocket.API;
using System;
using System.Xml.Serialization;

namespace BuffSystem
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int DelayCheck = 1;
        public int DelaySaveBuffs = 300;
        public ElixirXML[] Elixirs;
        public BuffXML[] Buffs;

        public void LoadDefaults()
        {
            Elixirs = new ElixirXML[] { new ElixirXML() };
            Buffs = new BuffXML[] { new BuffXML() };
        }
    }

    [Serializable]
    public class ElixirXML
    {
        public ushort ItemID;
        public int Time;
        [XmlArrayItem(ElementName = "Skill")]
        public string[] Skills;
        [XmlArrayItem(ElementName = "Level")]
        public ushort[] BoostLevel;
        
        public ElixirXML()
        {
            ItemID = 999;
            Time = 30;
            Skills = new string[] {"Overkill", "Fishing"};
            BoostLevel = new ushort[] {2, 1};
        }
    }

    [Serializable]
    public class BuffXML
    {
        [XmlArrayItem(ElementName = "Level")]
        public ushort[] BoostLevel;
        public int Time;
        public ulong SteamID;
        [XmlArrayItem(ElementName = "Skill")]
        public string[] Skills;

        public BuffXML()
        {
            BoostLevel = new ushort[] {1, 1};
            Time = 30;
            SteamID = 123467890;
            Skills = new string[] {"Cooking", "Crafting"};
        }

        public BuffXML(ushort[] BoostLevel, int Time, ulong SteamID, string[] Skill)
        {
            this.BoostLevel = BoostLevel;
            this.Time = Time;
            this.SteamID = SteamID;
            this.Skills = Skill;
        }
    }
}
