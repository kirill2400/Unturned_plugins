using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace BuffSystem
{
    class CommandGetID : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "GetID";

        public string Help => "Get ItemId";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "BuffSystem.GetID" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(((UnturnedPlayer)caller).Player.equipment.itemID.ToString());
        }
    }
}
