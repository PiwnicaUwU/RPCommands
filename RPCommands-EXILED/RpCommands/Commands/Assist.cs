using Exiled.API.Features;
using RpCommands.Services;
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
            if (Main.Instance.Config.EnableAssistWebhook)
            {
                string webhookContent = string.Format(
                    Main.Instance.Translation.AssistWebhookMessageFormat,
                    player.Nickname,
                    player.UserId,
                    message
                );

                WebhookService.SendWebhookAsync(Main.Instance.Config.AssistWebhookUrl, webhookContent);
            }

            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendStaffMessage(FormatMessage(player, message));
            }

            response = Main.Instance.Translation.HelpRequestSent;
            return true;
        }
    }
}
