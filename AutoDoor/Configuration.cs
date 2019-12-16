using Rocket.API;
using System;
using System.Xml.Serialization;


namespace AutoDoor
{
    public class Configuration : IRocketPluginConfiguration
    {
        public ushort HealItemID = 999;
        public ushort HealAmount = 10;
        public float HealDistance = 3f;
        public bool HealBleeding = true;
        public bool HealBroken = true;

        public void LoadDefaults() { }
    }
}
