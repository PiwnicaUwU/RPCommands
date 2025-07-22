using CommandSystem;
using RPCommands;

namespace RpCommands.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : RPCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => Main.Instance.Translation.Commands["do"];
    }
}
