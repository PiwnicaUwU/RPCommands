namespace RPCommands.Commands
{

    public class MeCommand : RPCommand
    {
        public override string OriginalCommand => "me";
        public override string Description => Main.Instance.Config.Translation.Commands["me"];
    }
}
