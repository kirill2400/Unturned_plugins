using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace BuffSystem
{
    class CommandTest : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "Test";

        public string Help => "Test";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "BuffSystem.Test" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var b = ((UnturnedPlayer)caller).Player.equipment;
            UnturnedChat.Say(b.itemID.ToString());
        }
    }
}
