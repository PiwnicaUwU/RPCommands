using CommandSystem;
using Exiled.API.Features;
using MEC;
using System;
using System.Linq;
using UnityEngine;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using RemoteAdmin;

namespace RPCommands
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
                response = Plugin.Instance.Translation.RoundNotStarted;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = string.Format(Plugin.Instance.Translation.Usage, Command);
                return false;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = Plugin.Instance.Translation.OnlyPlayers;
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            string message = string.Join(" ", arguments);
            float range = Plugin.Instance.Config.GetRange(Command);
            float duration = Plugin.Instance.Config.GetDuration(Command);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = Plugin.Instance.Translation.MessageSent;
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
        public override string Command => Plugin.Instance.Translation.CommandNames["me"];
        public override string Description => Plugin.Instance.Translation.Commands["me"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : NarrativeCommand
    {
        public override string Command => Plugin.Instance.Translation.CommandNames["do"];
        public override string Description => Plugin.Instance.Translation.Commands["do"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : NarrativeCommand
    {
        public override string Command => Plugin.Instance.Translation.CommandNames["look"];
        public override string Description => Plugin.Instance.Translation.Commands["look"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : NarrativeCommand
    {
        public override string Command => Plugin.Instance.Translation.CommandNames["ooc"];
        public override string Description => Plugin.Instance.Translation.Commands["ooc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : NarrativeCommand
    {
        public override string Command => Plugin.Instance.Translation.CommandNames["try"];
        public override string Description => Plugin.Instance.Translation.Commands["try"];

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string resultKey = isSuccess ? "success" : "fail";
            string result = Plugin.Instance.Translation.TryResult[resultKey];

            return Plugin.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }
}
