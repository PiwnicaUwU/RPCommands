using RPCommands;

namespace RpCommands.Commands
{

    public class DoCommand : RPCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => Main.Instance.Translation.Commands["do"];
    }
}
