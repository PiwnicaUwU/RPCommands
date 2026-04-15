using LabApi.Features.Wrappers;
using RPCommands.Handlers;

namespace RPCommands.Commands
{
    internal class TryCommand : InternalRPCommand
    {
        public override string OriginalCommand => "try";
        public override string Description => Main.Instance.Config.Translation.Commands["try"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            string formattedMessage = FormatMessage(player, message);
            DisplayHandler.DisplayMessage(player, formattedMessage, CommandRange, CommandDuration, DisplayMode);

            response = MsgSent;
            return true;
        }
        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(1, 101) <= Main.Instance.Config.TryCommandSuccessChance;
            string resultKey = isSuccess ? "success" : "fail";

            string result = Main.Instance.Config.Translation.TryResult[resultKey];

            return Main.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }
}
