using RPCommands;

namespace RpCommands.Commands
{


    public class OocCommand : RPCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => Main.Instance.Translation.Commands["ooc"];
    }
}
