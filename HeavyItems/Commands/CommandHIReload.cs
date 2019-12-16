using Rocket.API;
using System.Collections.Generic;
using Logger = Rocket.Core.Logging.Logger;

namespace HeavyItems
{
    class CommandHIReload : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "hiReload";

        public string Help => "Reload configuration";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "HeavyBackpack.Reload" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 1 && command[0] == "save")
            {
                HeavyItems.Instance.Configuration.Save();
                Logger.LogWarning("\tConfig has been saved!");
            }
            else
            {
                HeavyItems.Instance.Configuration.Load();
                Logger.LogWarning("\tConfig has been reloaded!");
            }
        }
    }
}
