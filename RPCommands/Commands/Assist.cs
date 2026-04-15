using LabApi.Features.Wrappers;
using RPCommands.Extensions;
using RPCommands.Services;
using System.Linq;

namespace RPCommands.Commands
{
    internal class AssistCommand : InternalRPCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => Main.Instance.Config.Translation.Commands["assist"];
        public override bool RequireAlive => false;
        public override bool RequireRoundStarted => false;
        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            string displayUserId = (Main.Instance.Config.HideUserIdForDnt && player.DoNotTrack)
                ? "DNT"
                : player.UserId;

            if (Main.Instance.Config.EnableAssistWebhook)
            {
                string webhookContent = string.Format(
                    Main.Instance.Config.Translation.AssistWebhookMessageFormat,
                    player.Nickname,
                    displayUserId,
                    player.PlayerId,
                    message
                );

                WebhookService.SendWebhookAsync(Main.Instance.Config.AssistWebhookUrl, webhookContent);
            }

            foreach (ReferenceHub hub in ReferenceHub.AllHubs.Where(h => h.serverRoles.RemoteAdmin))
            {
                player.SendStaffMessage(FormatMessage(player, message), EncryptedChannelManager.EncryptedChannel.AdminChat);
            }

            response = Main.Instance.Config.Translation.HelpRequestSent;
            return true;
        }
    }
}
