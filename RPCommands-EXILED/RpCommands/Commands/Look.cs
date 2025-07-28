using RPCommands;

namespace RpCommands.Commands
{

    public class LookCommand : RPCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Main.Instance.Translation.Commands["look"];
    }
}
