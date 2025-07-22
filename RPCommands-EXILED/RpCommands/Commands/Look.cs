using CommandSystem;
using RPCommands;

namespace RpCommands.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : RPCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Main.Instance.Translation.Commands["look"];
    }
}
