using LabApi.Features.Wrappers;

namespace RPCommands.Commands;

internal sealed class SuicideCommand : InternalRPCommand
{
    public override string OriginalCommand => "suicide";

    public override string Description => Main.Instance.Config.Translation.Commands["suicide"];
    public override bool AllowScp => Main.Instance.Config.AllowScpSuicide;

    protected override bool ExecuteAction(Player player, string message, out string response)
    {
        player.Kill(Main.Instance.Config.Translation.DeathReason);
        response = Main.Instance.Config.Translation.SuicideSuccess;
        return true;
    }
}
