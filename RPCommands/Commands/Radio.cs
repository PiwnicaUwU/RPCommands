using LabApi.Features.Wrappers;
using RPCommands.Handlers;

namespace RPCommands.Commands
{
    internal class RadioCommand : InternalRPCommand
    {
        public override string OriginalCommand => "radio";
        public override string Description => Main.Instance.Config.Translation.Commands["radio"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (!HasRadio(player))
            {
                response = Main.Instance.Config.Translation.RadioRequired;
                return false;
            }

            float duration = Main.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            foreach (Player receiver in Player.List)
            {
                if (!receiver.IsAlive || !HasRadio(receiver))
                    continue;

                DisplayHandler.SendHint(receiver, formattedMessage, duration);
                receiver.SendConsoleMessage(formattedMessage, "yellow");
            }
            player.SendConsoleMessage(formattedMessage, "yellow");
            response = Main.Instance.Config.Translation.MessageSent;
            return true;
        }

        private bool HasRadio(Player player)
        {
            if (player.CurrentItem != null && player.CurrentItem.Type == ItemType.Radio)
                return true;

            foreach (var item in player.Items)
            {
                if (item.Type == ItemType.Radio)
                    return true;
            }
            return false;
        }
    }
}
