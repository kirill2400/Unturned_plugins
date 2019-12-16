using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace BuffSystem
{
    class CommandReload : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "Reload";

        public string Help => "Call reload config";

        public string Syntax => "[items : buffs : save]";

        public List<string> Aliases => new List<string>() { "reload" };

        public List<string> Permissions => new List<string>() { "BuffSystem.Reload" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            switch (command[0])
            {
                case "items":
                    BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
                    break;
                case "buffs":
                    BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadBuffs);
                    break;
                case "save":
                    BuffSystem.Manager.SaveBuffsToXML();
                    break;
                default:
                    BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadItems);
                    BuffSystem.Instance.Configuration.Load(BuffSystem.Instance.LoadBuffs);
                    break;
            }
        }
    }
}