namespace RPCommands.Commands
{


    public class OocCommand : RPCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => Main.Instance.Config.Translation.Commands["ooc"];
    }
}
