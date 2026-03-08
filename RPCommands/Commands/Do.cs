namespace RPCommands.Commands
{

    public class DoCommand : RPCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => Main.Instance.Config.Translation.Commands["do"];
    }
}
