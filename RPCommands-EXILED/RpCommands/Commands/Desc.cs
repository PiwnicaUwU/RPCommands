using CommandSystem;
using RPCommands;

namespace RpCommands.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DescCommand : RPCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => Main.Instance.Translation.Commands["desc"];
    }
}
