using CommandSystem;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using RPCommands.API.PlayerEvents;
using RPCommands.Enum;
using System;

namespace RPCommands.API
{
    /// <summary>
    /// Base class for all RPCommands. 
    /// Devs should inherit from this class to create their own commands.
    /// </summary>
    public abstract class BaseRPCommand : ICommand
    {
        public abstract string Command { get; }
        public virtual string[] Aliases => [];
        public abstract string Description { get; }

        // Command settings
        public virtual bool IsCommandEnabled => true;
        public virtual bool AllowScp => false;
        public virtual bool RequireAlive => true;
        public virtual bool RequireRoundStarted => true;
        public virtual bool AllowNoArguments => false;

        public virtual float CommandCooldown => 0f;
        public virtual float CommandRange => 10f;
        public virtual float CommandDuration => 5f;
        public virtual RPCommandsMode DisplayMode => RPCommandsMode.Both;

        // Default messages that you can override if you want to
        public virtual string MsgOnlyPlayers => "Only players can use this command.";
        public virtual string MsgCommandDisabled => "This command is disabled.";
        public virtual string MsgRoundNotStarted => "The round hasn't started yet.";
        public virtual string MsgOnlyHumans => "SCPs cannot use this command.";
        public virtual string MsgOnlyAlive => "You must be alive to use this command.";
        public virtual string MsgUsage => "Usage: .{0} [message]";
        public virtual string MsgCooldown => "You must wait {0} seconds before using this again.";
        public virtual string MsgSent => "Message sent successfully.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = MsgOnlyPlayers;
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);

            if (!IsCommandEnabled)
            {
                response = MsgCommandDisabled;
                return false;
            }

            if (RequireRoundStarted && !Round.IsRoundStarted)
            {
                response = MsgRoundNotStarted;
                return false;
            }

            if (!AllowScp && player.IsSCP)
            {
                response = MsgOnlyHumans;
                return false;
            }

            if (RequireAlive && !player.IsAlive)
            {
                response = MsgOnlyAlive;
                return false;
            }

            if (arguments.Count < 1 && !AllowNoArguments)
            {
                response = string.Format(MsgUsage, Command);
                return false;
            }

            if (player.HasCooldown(Command))
            {
                float remainingTime = player.GetRemainingCooldown(Command);
                response = MsgCooldown.Contains("{0}") ? string.Format(MsgCooldown, Math.Ceiling(remainingTime)) : MsgCooldown;
                return false;
            }

            string rawMessage = string.Join(" ", arguments);

            var sendingArgs = new PlayerSendingRpCommandEventArgs(player, Command, rawMessage);
            Events.OnSendingRpCommand(sendingArgs);

            if (!sendingArgs.IsAllowed)
            {
                response = "Command cancelled by an external plugin.";
                return false;
            }

            if (CheckForBannedWords(sendingArgs.Message))
            {
                response = "Banned word detected in your message.";
                return false;
            }

            if (!ExecuteAction(player, sendingArgs.Message, out response))
                return false;

            player.SetCooldown(Command, CommandCooldown);
            player.SetLastMessage(Command, sendingArgs.Message);
            return true;
        }

        /// <summary>
        /// Contains the core logic of the command.
        /// </summary>
        protected abstract bool ExecuteAction(Player player, string message, out string response);

        /// <summary>
        /// Formats the message before displaying it.
        /// </summary>
        protected virtual string FormatMessage(Player player, string message)
        {
            return $"[{player.Nickname}] {message}";
        }

        private bool CheckForBannedWords(string message)
        {
            if (Main.Instance == null || Main.Instance.Config == null) return false;

            foreach (var bannedWord in Main.Instance.Config.BannedWords)
            {
                if (message.ToLower().Contains(bannedWord.ToLower()))
                    return true;
            }
            return false;
        }
    }
}