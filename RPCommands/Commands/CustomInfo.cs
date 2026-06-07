using LabApi.Features.Wrappers;
using System.Text;

namespace RPCommands.Commands
{
    internal class CustomInfoCommand : InternalRPCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => Main.Instance.Config.Translation.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            StringBuilder sb = new();

            if (!string.IsNullOrEmpty(player.CustomInfo))
            {
                foreach (string preserved in Main.Instance.Config.PreservedCustomInfoLines)
                {
                    if (player.CustomInfo.Contains(preserved))
                    {
                        sb.Append(preserved);
                    }
                }
            }

            sb.Append(message);

            string msg = sb.ToString();
            int maxLength = Main.Instance.Config.MaxCustomInfoLength;

            if (msg.Length > maxLength)
            {
                response = Main.Instance.Config.Translation.CustomInfoTooLong;
                return false;
            }

            if (!Player.ValidateCustomInfo(msg, out string errorMessage))
            {
                response = errorMessage;
                return false;
            }

            player.CustomInfo = msg;
            response = Main.Instance.Config.Translation.CustomInfoSet;
            return true;
        }
    }
}
