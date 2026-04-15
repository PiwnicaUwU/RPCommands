using LabApi.Features.Wrappers;
using RPCommands.Handlers;

namespace RPCommands.Commands
{
    internal class LookCommand : InternalRPCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Main.Instance.Config.Translation.Commands["look"];
        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            string formattedMessage = FormatMessage(player, message);
            DisplayHandler.DisplayMessage(player, formattedMessage, CommandRange, CommandDuration, DisplayMode);

            response = MsgSent;
            return true;
        }
    }
}
