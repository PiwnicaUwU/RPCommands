using Exiled.API.Features;
using RPCommands;
using System.Linq;

namespace RpCommands.Commands
{

    public class AssistCommand : RPCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => Main.Instance.Translation.Commands["assist"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendStaffMessage(FormatMessage(player, message));
            }

            response = Main.Instance.Translation.HelpRequestSent;
            return true;
        }
    }
}
