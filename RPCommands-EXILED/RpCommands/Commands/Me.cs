using CommandSystem;
using RPCommands;

namespace RpCommands.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : RPCommand
    {
        public override string OriginalCommand => "me";
        public override string Description => Main.Instance.Translation.Commands["me"];
    }
}
