using LabApi.Features.Wrappers;
using RPCommands.API;
using RPCommands.Enum;

namespace RPCommands
{
    /// <summary>
    /// Used ONLY for the built-in commands of the RPCommands.
    /// </summary>
    internal abstract class InternalRPCommand : BaseRPCommand
    {
        public abstract string OriginalCommand { get; }

        public override string Command => Main.Instance.Config.Translation.CommandNames.TryGetValue(OriginalCommand, out string translatedName) ? translatedName : OriginalCommand;
        public override bool IsCommandEnabled => Main.Instance.Config.IsCommandEnabled(OriginalCommand);
        public override bool AllowScp => Main.Instance.Config.AllowScpToUseCommands;
        public override float CommandCooldown => Main.Instance.Config.GetCooldown(OriginalCommand);
        public override float CommandRange => Main.Instance.Config.GetRange(OriginalCommand);
        public override float CommandDuration => Main.Instance.Config.GetDuration(OriginalCommand);
        public override RPCommandsMode DisplayMode => Main.Instance.Config.DisplayMode;

        public override string MsgOnlyPlayers => Main.Instance.Config.Translation.OnlyPlayers;
        public override string MsgCommandDisabled => Main.Instance.Config.Translation.CommandDisabled;
        public override string MsgRoundNotStarted => Main.Instance.Config.Translation.RoundNotStarted;
        public override string MsgOnlyHumans => Main.Instance.Config.Translation.OnlyHumans;
        public override string MsgOnlyAlive => Main.Instance.Config.Translation.OnlyAlive;
        public override string MsgUsage => Main.Instance.Config.Translation.Usage;
        public override string MsgCooldown => Main.Instance.Config.Translation.CommandCooldown;

        protected override string FormatMessage(Player player, string message)
        {
            return Main.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
        }
    }
}