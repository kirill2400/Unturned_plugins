using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using SDG.Unturned;

namespace BuffSystem
{
    class CommandTest : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "Test";

        public string Help => "Test";

        public string Syntax => "";

        public List<string> Aliases => new List<string>() { "test" };

        public List<string> Permissions => new List<string>() { "BuffSystem.Test" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var b = ((UnturnedPlayer)caller).Player.equipment;
            UnturnedChat.Say(b.itemID.ToString());
        }
    }
}
