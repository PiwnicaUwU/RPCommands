using CommandSystem;
using Exiled.API.Features;
using MEC;
using System;
using System.Linq;
using UnityEngine;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using RemoteAdmin;

namespace RPCommands.Core.Commands
{
    public abstract class NarrativeCommand : ICommand
    {
        public abstract string Command { get; }
        public virtual string[] Aliases => Array.Empty<string>();
        public abstract string Description { get; }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.IsStarted)
            {
                response = "You cannot use this command because the round has not started yet.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = $"Usage: .{Command} <message>";
                return false;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = "Only players can use this command.";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            string message = string.Join(" ", arguments);
            float range = Plugin.Instance.Config.Me.Range;
            float duration = Plugin.Instance.Config.Me.Duration;
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = "Message has been sent.";
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return Plugin.Instance.Config.FormatMessage(Command, player.Nickname, message);
        }

        private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
        {
            foreach (Player player in Player.List.Where(p => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
            {
                SendHint(player, message, duration);
            }

            SendHint(sender, message, duration);
        }

        private void SendHint(Player player, string message, float duration)
        {
            DynamicHint hint = new()
            {
                Text = message,
                TargetY = 800,
                TargetX = -950,
                FontSize = 25,
            };

            PlayerDisplay playerDisplay = PlayerDisplay.Get(player);
            playerDisplay?.AddHint(hint);

            Timing.CallDelayed(duration, () => playerDisplay?.RemoveHint(hint));
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : NarrativeCommand
    {
        public override string Command => "me";
        public override string Description => "Narrative command 'Me'.";
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : NarrativeCommand
    {
        public override string Command => "do";
        public override string Description => "Narrative command 'Do'.";
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : NarrativeCommand
    {
        public override string Command => "look";
        public override string Description => "Narrative command 'Look'.";
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : NarrativeCommand
    {
        public override string Command => "ooc";
        public override string Description => "Narrative command 'Ooc'.";
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : NarrativeCommand
    {
        public override string Command => "try";
        public override string Description => "Narrative command 'Try'.";

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string result = isSuccess ? "successfully" : "unsuccessfully";
            return Plugin.Instance.Config.FormatMessage(Command, player.Nickname, message, result);
        }
    }
}
