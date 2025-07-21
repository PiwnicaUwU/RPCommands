using CommandSystem;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Features.Wrappers;
using MEC;
using RemoteAdmin;
using RPCommands.API;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPCommands
{
    public abstract class NarrativeCommand : ICommand
    {
        public abstract string OriginalCommand { get; }
        public virtual string[] Aliases // well this shit is broken in labapi version ://
        {
            get
            {
                if (Main.Instance?.Config?.CommandNames is { } cm &&
                    cm.TryGetValue(OriginalCommand, out string translate) &&
                    !string.Equals(translate, OriginalCommand, StringComparison.OrdinalIgnoreCase))
                {
                    return new[] { translate };
                }

                return Array.Empty<string>();
            }
        }

        public abstract string Description { get; }
        public string Command
        {
            get
            {
                if (Main.Instance?.Config?.CommandNames is { } cm && cm.TryGetValue(OriginalCommand, out var rezultat))
                    return rezultat;

                return OriginalCommand;
            }
        }

        private static readonly Dictionary<Player, Dictionary<string, float>> PlayerCooldowns = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = Main.Instance.Config.OnlyPlayers;

                return false;
            }
            Player player = Player.Get(playerSender.ReferenceHub);

            if (!Main.Instance.Config.IsCommandEnabled(OriginalCommand))
            {
                response = Main.Instance.Config.CommandDisabled;
                return false;
            }

            if (!Round.IsRoundStarted && OriginalCommand != "assist") // assist must work even if round is not started
            {
                response = Main.Instance.Config.RoundNotStarted;
                return false;
            }

            if (player.IsSCP && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Config.OnlyHumans;
                return false;
            }

            if (!player.IsAlive && OriginalCommand != "assist") // assist ALL always MUST work
            {
                response = Main.Instance.Config.OnlyAlive;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = string.Format(Main.Instance.Config.Usage, Command);
                return false;
            }

            if (HasCooldown(player, out float remainingTime))
            {
                if (Main.Instance.Config.CommandCooldown.Contains("{0}"))
                    response = string.Format(Main.Instance.Config.CommandCooldown, Math.Ceiling(remainingTime));
                else
                    response = Main.Instance.Config.CommandCooldown;

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
            float range = Main.Instance.Config.GetRange(OriginalCommand);
            float duration = Main.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = Main.Instance.Config.MessageSent;
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return Main.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
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

            float cooldownDuration = Main.Instance.Config.GetCooldown(OriginalCommand);
            PlayerCooldowns[player][OriginalCommand] = Time.time + cooldownDuration;
        }

        private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
        {
            bool showInConsole = Main.Instance.Config.ShowCommandInSenderConsole;
            foreach (Player player in Player.List.Where(p => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
            {
                SendHint(player, message, duration);
                if (showInConsole)
                {
                    player.SendConsoleMessage($"{message}", "yellow");
                }
            }

            SendHint(sender, message, duration);

            if (showInConsole)
            {
                sender.SendConsoleMessage($"{message}", "yellow");
            }
        }

        public void SendHint(Player player, string message, float duration)
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


            if (Main.Instance.Config.ShowHintsToSpectatorsOfReceivers)
            {
                foreach (Player observer in player.CurrentSpectators)
                {
                    if (!observer.IsOnline) continue;

                    PlayerDisplay observerDisplay = PlayerDisplay.Get(observer);
                    observerDisplay?.AddHint(hint);
                    Timing.CallDelayed(duration, () => observerDisplay?.RemoveHint(hint));
                }
            }
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : NarrativeCommand
    {
        public override string OriginalCommand => "me";

        private string _description;
        public override string Description => _description ??= Main.Instance?.Config?.Commands["me"] ?? "Narrative command 'Me'";

    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => Main.Instance.Config.Commands["do"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : NarrativeCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Main.Instance.Config.Commands["look"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : NarrativeCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => Main.Instance.Config.Commands["ooc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DescCommand : NarrativeCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => Main.Instance.Config.Commands["desc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CustomInfoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => Main.Instance.Config.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = Main.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = Main.Instance.Config.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = Main.Instance.Config.CustomInfoSet;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HelpCommand : NarrativeCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => Main.Instance.Config.Commands["assist"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendBroadcast(FormatMessage(player, message), 15);
            }

            response = Main.Instance.Config.HelpRequestSent;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : NarrativeCommand
    {
        public override string OriginalCommand => "try";
        public override string Description => Main.Instance.Config.Commands["try"];

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string resultKey = isSuccess ? "success" : "fail";
            string result = Main.Instance.Config.TryResult[resultKey];

            return Main.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class RadioCommand : NarrativeCommand
    {
        public override string OriginalCommand => "radio";
        public override string Description => Main.Instance.Config.Commands["radio"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.CurrentItem == null || player.CurrentItem.Type != ItemType.Radio)
            {
                response = Main.Instance.Config.RadioRequired;
                return false;
            }

            float duration = Main.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            foreach (Player receiver in Player.List)
            {
                if (!receiver.IsAlive || receiver.CurrentItem?.Type != ItemType.Radio)
                    continue;

                SendHint(receiver, formattedMessage, duration);
            }

            response = Main.Instance.Config.MessageSent;
            return true;
        }
    }
}