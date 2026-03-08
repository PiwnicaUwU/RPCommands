namespace RPCommands.Commands
{

    public class LookCommand : RPCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Main.Instance.Config.Translation.Commands["look"];
    }
}
