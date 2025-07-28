using Exiled.API.Features;
using RPCommands;

namespace RpCommands.Commands
{

    public class TryCommand : RPCommand
    {
        public override string OriginalCommand => "try";
        public override string Description => Main.Instance.Translation.Commands["try"];

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string resultKey = isSuccess ? "success" : "fail";
            string result = Main.Instance.Translation.TryResult[resultKey];

            return Main.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }
}
