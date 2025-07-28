using Exiled.API.Features;
using RPCommands;

namespace RpCommands.Commands
{

    public class CustomInfoCommand : RPCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => Main.Instance.Translation.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = Main.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = Main.Instance.Translation.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = Main.Instance.Translation.CustomInfoSet;
            return true;
        }
    }
}
