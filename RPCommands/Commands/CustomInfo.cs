using LabApi.Features.Wrappers;

namespace RPCommands.Commands
{
    internal class CustomInfoCommand : InternalRPCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => Main.Instance.Config.Translation.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = Main.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = Main.Instance.Config.Translation.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = Main.Instance.Config.Translation.CustomInfoSet;
            return true;
        }
    }
}
