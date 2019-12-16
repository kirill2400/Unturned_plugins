using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;

namespace BuffSystem
{
    class CommandGetID : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "GetID";

        public string Help => "Get ItemId";

        public string Syntax => "";

        public List<string> Aliases => new List<string>() { "getid" };

        public List<string> Permissions => new List<string>() { "BuffSystem.GetID" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(((UnturnedPlayer)caller).Player.equipment.itemID.ToString());
        }
    }
}
