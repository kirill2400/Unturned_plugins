using Rocket.API;

namespace HealingStaff
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
