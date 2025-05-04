using CommandSystem;
using Exiled.API.Features;
using MEC;
using System;
using System.Linq;
using UnityEngine;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using RemoteAdmin;
using RPCommands.API;
using System.Collections.Generic;

namespace RPCommands
{
    public abstract class NarrativeCommand : ICommand
    {
        public abstract string OriginalCommand { get; }
        public virtual string[] Aliases => Array.Empty<string>();
        public abstract string Description { get; }

        public string Command => Plugin.Instance.Translation.CommandNames.TryGetValue(OriginalCommand, out string translatedName) ? translatedName : OriginalCommand;

        private static readonly Dictionary<Player, Dictionary<string, float>> PlayerCooldowns = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = Plugin.Instance.Translation.OnlyPlayers;
                return false;
            }
            Player player = Player.Get(playerSender.ReferenceHub);

            if (!Plugin.Instance.Config.IsCommandEnabled(OriginalCommand))
            {
                response = Plugin.Instance.Translation.CommandDisabled;
                return false;
            }

            if (!Round.IsStarted)
            {
                response = Plugin.Instance.Translation.RoundNotStarted;
                return false;
            }

            if (player.IsScp && !Plugin.Instance.Config.AllowScpToUseCommands)
            {
                response = Plugin.Instance.Translation.OnlyHumans;
                return false;
            }

            if (!player.IsAlive)
            {
                response = Plugin.Instance.Translation.OnlyAlive;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = string.Format(Plugin.Instance.Translation.Usage, Command);
                return false;
            }

            if (HasCooldown(player, out float remainingTime))
            {
                if (Plugin.Instance.Translation.CommandCooldown.Contains("{0}"))
                    response = string.Format(Plugin.Instance.Translation.CommandCooldown, Math.Ceiling(remainingTime));
                else
                    response = Plugin.Instance.Translation.CommandCooldown;

                return false;
            }

            string message = string.Join(" ", arguments);

            if (!ExecuteAction(player, message, out response))
                return false;

            SetCooldown(player);
            Request.SetLastMessage(player, OriginalCommand, message);
            return true;
        }

        protected virtual bool ExecuteAction(Player player, string message, out string response)
        {
            float range = Plugin.Instance.Config.GetRange(OriginalCommand);
            float duration = Plugin.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = Plugin.Instance.Translation.MessageSent;
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return Plugin.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
        }

        private bool HasCooldown(Player player, out float remainingTime)
        {
            if (PlayerCooldowns.TryGetValue(player, out var cooldowns) && cooldowns.TryGetValue(OriginalCommand, out var cooldownEndTime))
            {
                remainingTime = cooldownEndTime - Time.time;
                return remainingTime > 0;
            }

            remainingTime = 0;
            return false;
        }

        private void SetCooldown(Player player)
        {
            if (!PlayerCooldowns.ContainsKey(player))
                PlayerCooldowns[player] = new Dictionary<string, float>();

            float cooldownDuration = Plugin.Instance.Config.GetCooldown(OriginalCommand);
            PlayerCooldowns[player][OriginalCommand] = Time.time + cooldownDuration;
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
        public override string OriginalCommand => "me";
        public override string Description => Plugin.Instance.Translation.Commands["me"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => Plugin.Instance.Translation.Commands["do"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : NarrativeCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Plugin.Instance.Translation.Commands["look"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : NarrativeCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => Plugin.Instance.Translation.Commands["ooc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DescCommand : NarrativeCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => Plugin.Instance.Translation.Commands["desc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CustomInfoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => Plugin.Instance.Translation.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = Plugin.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = Plugin.Instance.Translation.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = Plugin.Instance.Translation.CustomInfoSet;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HelpCommand : NarrativeCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => Plugin.Instance.Translation.Commands["assist"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendStaffMessage(FormatMessage(player, message));
            }

            response = Plugin.Instance.Translation.HelpRequestSent;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : NarrativeCommand
    {
        public override string OriginalCommand => "try";
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
