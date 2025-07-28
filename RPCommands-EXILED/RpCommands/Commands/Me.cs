using RPCommands;

namespace RpCommands.Commands
{

    public class MeCommand : RPCommand
    {
        public override string OriginalCommand => "me";
        public override string Description => Main.Instance.Translation.Commands["me"];
    }
}
