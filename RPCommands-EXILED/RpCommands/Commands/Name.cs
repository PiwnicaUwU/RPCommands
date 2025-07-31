using Exiled.API.Features;
using RPCommands;

namespace RpCommands.Commands
{
    public class NameCommand : RPCommand
    {
        public override string OriginalCommand => "name";
        public override string Description => Main.Instance.Translation.Commands["name"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            response = string.Format(Main.Instance.Translation.NameResponse, player.DisplayNickname, message);
            return true;
        }
    }
}