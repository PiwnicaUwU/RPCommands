using LabApi.Features.Wrappers;
using RPCommands.Services;
using System.Linq;

namespace RPCommands.Commands
{

    public class AssistCommand : RPCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => Main.Instance.Config.Translation.Commands["assist"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (Main.Instance.Config.EnableAssistWebhook)
            {
                string webhookContent = string.Format(
                    Main.Instance.Config.Translation.AssistWebhookMessageFormat,
                    player.Nickname,
                    player.UserId,
                    message
                );

                WebhookService.SendWebhookAsync(Main.Instance.Config.AssistWebhookUrl, webhookContent);
            }

            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.ReferenceHub.encryptedChannelManager.TrySendMessageToClient(FormatMessage(player, message), EncryptedChannelManager.EncryptedChannel.AdminChat);
            }

            response = Main.Instance.Config.Translation.HelpRequestSent;
            return true;
        }
    }
}
