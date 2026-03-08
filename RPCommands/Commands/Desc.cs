namespace RPCommands.Commands
{

    public class DescCommand : RPCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => Main.Instance.Config.Translation.Commands["desc"];
    }
}
