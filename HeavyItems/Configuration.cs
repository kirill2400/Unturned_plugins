using Rocket.API;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HeavyItems
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int DelayAutoSave = 300;

        public string UITextBackpack = "Ваш рюкзак полон!";
        public string UIColorBackpack = "#ffffff";
        public string UISizeBackpack = "70";
        public string UITextArmor = "Ваша одежда очень тяжела!";
        public string UIColorArmor = "#ffffff";
        public string UISizeArmor = "50";

        [XmlArrayItem(ElementName = "SteamID")]
        public List<ulong> OverloadedByBackpackPlayers;
        [XmlArrayItem(ElementName = "SteamID")]
        public List<ulong> OverloadedByArmorPlayers;
        
        [XmlArrayItem(ElementName = "ItemID")]
        public ushort[] ShirtID;
        [XmlArrayItem(ElementName = "ItemID")]
        public ushort[] VestID;

        public void LoadDefaults()
        {
            OverloadedByBackpackPlayers = new List<ulong>();
            OverloadedByArmorPlayers = new List<ulong>();

            ShirtID = new ushort[] { 12332, 32123 };
            VestID = new ushort[] { 12332, 32123 };
        }
    }
}
