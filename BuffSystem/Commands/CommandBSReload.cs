using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using System.Collections.Generic;

namespace BuffSystem
{
    class CommandBSReload : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "bsReload";

        public string Help => "Call reload config";

        public string Syntax => "[items : buffs : save]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "BuffSystem.Reload" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 1)
                switch (command[0])
                {
                    case "items":
                        BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
                        Logger.LogWarning("\tItems reloaded!");
                        break;
                    case "buffs":
                        BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadBuffs);
                        Logger.LogWarning("\tBuffs reloaded!");
                        break;
                    case "save":
                        BuffSystem.Manager.SaveBuffsToXML();
                        Logger.LogWarning("\tBuffs saved!");
                        break;
                    default:
                        BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
                        BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadBuffs);
                        Logger.LogWarning("\tConfiguration reloaded!");
                        break;
                }
                else
                {
                    BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
                    BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadBuffs);
                    Logger.LogWarning("\tConfiguration reloaded!");
                }
        }
    }
}