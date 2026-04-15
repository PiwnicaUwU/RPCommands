using LabApi.Features.Wrappers;

namespace RPCommands.Commands
{
    internal class NameCommand : InternalRPCommand
    {
        public override string OriginalCommand => "name";
        public override string Description => Main.Instance.Config.Translation.Commands["name"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            string customInfo = player.CustomInfo ?? "None";
            response = string.Format(Main.Instance.Config.Translation.NameResponse, player.DisplayName, customInfo);
            return true;
        }
    }
}