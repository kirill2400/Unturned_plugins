using Rocket.API;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InfectedArrow
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int DelayAutoSave = 300;

        public string UITextInfect = "Ваша рана гноится!";
        public string UIColorInfect = "#ffffff";
        public string UISizeInfect = "75";

        public List<InfectedPlayer> InfectedPlayers;

        public void LoadDefaults()
        {
            InfectedPlayers = new List<InfectedPlayer>();
        }
    }

    [Serializable]
    public class InfectedPlayer
    {
        public ulong SteamId;
        public int Time;

        public InfectedPlayer() { }

        public InfectedPlayer(ulong steamid)
        {
            SteamId = steamid;
            Time = 0;
        }
    }
}
