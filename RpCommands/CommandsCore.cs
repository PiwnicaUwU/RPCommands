using CommandSystem;
#if EXILED
using Exiled.API.Features;
#else
using LabApi.Features.Wrappers;
#endif
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
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
        public virtual string[] Aliases
        {
            get
            {
#if EXILED
        return Array.Empty<string>();
#else
                if (LabAPIMain.Instance?.Config?.CommandNames is { } names &&
                    names.TryGetValue(OriginalCommand, out string translated) &&
                    !string.Equals(translated, OriginalCommand, StringComparison.OrdinalIgnoreCase))
                {
                    return new[] { translated };
                }

                return Array.Empty<string>();
#endif
            }
        }

        public abstract string Description { get; }
#if EXILED
        public string Command => EXILEDMain.Instance.Translation.CommandNames.TryGetValue(OriginalCommand, out string translatedName) ? translatedName : OriginalCommand;

        private static readonly Dictionary<Player, Dictionary<string, float>> PlayerCooldowns = new();
#else
        public string Command
        {
            get
            {
                if (LabAPIMain.Instance?.Config?.CommandNames is { } names && names.TryGetValue(OriginalCommand, out var result))
                    return result;

                return OriginalCommand;
            }
        }

        private static readonly Dictionary<Player, Dictionary<string, float>> PlayerCooldowns = new();

#endif
#if EXILED
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = EXILEDMain.Instance.Translation.OnlyPlayers;

                return false;
            }
            Player player = Player.Get(playerSender.ReferenceHub);

            if (!EXILEDMain.Instance.Config.IsCommandEnabled(OriginalCommand))
            {
                response = EXILEDMain.Instance.Translation.CommandDisabled;
                return false;
            }

            if (!Round.IsStarted && OriginalCommand != "assist")
            {
                response = EXILEDMain.Instance.Translation.RoundNotStarted;
                return false;
            }

            if (player.IsScp && !EXILEDMain.Instance.Config.AllowScpToUseCommands)
            {
                response = EXILEDMain.Instance.Translation.OnlyHumans;
                return false;
            }

            if (!player.IsAlive && OriginalCommand != "assist")
            {
                response = EXILEDMain.Instance.Translation.OnlyAlive;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = string.Format(EXILEDMain.Instance.Translation.Usage, Command);
                return false;
            }

            if (HasCooldown(player, out float remainingTime))
            {
                if (EXILEDMain.Instance.Translation.CommandCooldown.Contains("{0}"))
                    response = string.Format(EXILEDMain.Instance.Translation.CommandCooldown, Math.Ceiling(remainingTime));
                else
                    response = EXILEDMain.Instance.Translation.CommandCooldown;

                return false;
            }

            string message = string.Join(" ", arguments);

            if (!ExecuteAction(player, message, out response))
                return false;

            SetCooldown(player);
            Request.SetLastMessage(player, OriginalCommand, message);
            return true;
        }
#else
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = LabAPIMain.Instance.Config.OnlyPlayers;

                return false;
            }
            Player player = Player.Get(playerSender.ReferenceHub);

            if (!LabAPIMain.Instance.Config.IsCommandEnabled(OriginalCommand))
            {
                response = LabAPIMain.Instance.Config.CommandDisabled;
                return false;
            }

            if (!Round.IsRoundStarted && OriginalCommand != "assist")
            {
                response = LabAPIMain.Instance.Config.RoundNotStarted;
                return false;
            }

            if (player.IsSCP && !LabAPIMain.Instance.Config.AllowScpToUseCommands)
            {
                response = LabAPIMain.Instance.Config.OnlyHumans;
                return false;
            }

            if (!player.IsAlive && OriginalCommand != "assist")
            {
                response = LabAPIMain.Instance.Config.OnlyAlive;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = string.Format(LabAPIMain.Instance.Config.Usage, Command);
                return false;
            }

            if (HasCooldown(player, out float remainingTime))
            {
                if (LabAPIMain.Instance.Config.CommandCooldown.Contains("{0}"))
                    response = string.Format(LabAPIMain.Instance.Config.CommandCooldown, Math.Ceiling(remainingTime));
                else
                    response = LabAPIMain.Instance.Config.CommandCooldown;

                return false;
            }

            string message = string.Join(" ", arguments);

            if (!ExecuteAction(player, message, out response))
                return false;

            SetCooldown(player);
            Request.SetLastMessage(player, OriginalCommand, message);
            return true;
        }
#endif
#if EXILED

        protected virtual bool ExecuteAction(Player player, string message, out string response)
        {
            float range = EXILEDMain.Instance.Config.GetRange(OriginalCommand);
            float duration = EXILEDMain.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = EXILEDMain.Instance.Translation.MessageSent;
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return EXILEDMain.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
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

            float cooldownDuration = EXILEDMain.Instance.Config.GetCooldown(OriginalCommand);
            PlayerCooldowns[player][OriginalCommand] = Time.time + cooldownDuration;
        }

        private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
        {
            bool sentToOthers = false;
            HashSet<Player> hintedPlayers = new();

            foreach (Player player in Player.List.Where(p => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
            {
                SendHint(player, message, duration);
                hintedPlayers.Add(player);
                sentToOthers = true;
            }

            if (EXILEDMain.Instance.Config.ShowHintsToSpectatorsOfSender)
            {
                foreach (Player spectator in sender.CurrentSpectatingPlayers)
                {
                    if (spectator != null && spectator.IsConnected && hintedPlayers.Add(spectator))
                    {
                        SendHint(spectator, message, duration);
                    }
                }
            }

            SendHint(sender, message, duration);

            if (sentToOthers && EXILEDMain.Instance.Config.ShowCommandInSenderConsole)
            {
                sender.SendConsoleMessage($"{sender.Nickname}: {message}", "yellow");
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


            if (EXILEDMain.Instance.Config.ShowHintsToSpectatorsOfReceivers)
            {
                foreach (Player observer in player.CurrentSpectatingPlayers)
                {
                    if (!observer.IsConnected) continue;

                    PlayerDisplay observerDisplay = PlayerDisplay.Get(observer);
                    observerDisplay?.AddHint(hint);
                    Timing.CallDelayed(duration, () => observerDisplay?.RemoveHint(hint));
                }
            }
        }
#else
        protected virtual bool ExecuteAction(Player player, string message, out string response)
        {
            float range = LabAPIMain.Instance.Config.GetRange(OriginalCommand);
            float duration = LabAPIMain.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = LabAPIMain.Instance.Config.MessageSent;
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return LabAPIMain.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
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

            float cooldownDuration = LabAPIMain.Instance.Config.GetCooldown(OriginalCommand);
            PlayerCooldowns[player][OriginalCommand] = Time.time + cooldownDuration;
        }

        private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
        {
            bool sentToOthers = false;
            HashSet<Player> hintedPlayers = new();

            foreach (Player player in Player.List.Where(p => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
            {
                SendHint(player, message, duration);
                hintedPlayers.Add(player);
                sentToOthers = true;
            }

            if (LabAPIMain.Instance.Config.ShowHintsToSpectatorsOfSender)
            {
                foreach (Player spectator in sender.CurrentSpectators)
                {
                    if (spectator != null && spectator.IsOnline && hintedPlayers.Add(spectator))
                    {
                        SendHint(spectator, message, duration);
                    }
                }
            }

            SendHint(sender, message, duration);

            if (sentToOthers && LabAPIMain.Instance.Config.ShowCommandInSenderConsole)
            {
                sender.SendConsoleMessage($"{sender.Nickname}: {message}", "yellow");
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


            if (LabAPIMain.Instance.Config.ShowHintsToSpectatorsOfReceivers)
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
#endif
    }
#if EXILED

    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : NarrativeCommand
    {
        public override string OriginalCommand => "me";
        public override string Description => EXILEDMain.Instance.Translation.Commands["me"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => EXILEDMain.Instance.Translation.Commands["do"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : NarrativeCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => EXILEDMain.Instance.Translation.Commands["look"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : NarrativeCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => EXILEDMain.Instance.Translation.Commands["ooc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DescCommand : NarrativeCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => EXILEDMain.Instance.Translation.Commands["desc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CustomInfoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => EXILEDMain.Instance.Translation.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = EXILEDMain.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = EXILEDMain.Instance.Translation.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = EXILEDMain.Instance.Translation.CustomInfoSet;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HelpCommand : NarrativeCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => EXILEDMain.Instance.Translation.Commands["assist"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendStaffMessage(FormatMessage(player, message));
            }

            response = EXILEDMain.Instance.Translation.HelpRequestSent;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : NarrativeCommand
    {
        public override string OriginalCommand => "try";
        public override string Description => EXILEDMain.Instance.Translation.Commands["try"];

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string resultKey = isSuccess ? "success" : "fail";
            string result = EXILEDMain.Instance.Translation.TryResult[resultKey];

            return EXILEDMain.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class RadioCommand : NarrativeCommand
    {
        public override string OriginalCommand => "radio";
        public override string Description => EXILEDMain.Instance.Translation.Commands["radio"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.CurrentItem == null || player.CurrentItem.Type != ItemType.Radio)
            {
                response = EXILEDMain.Instance.Translation.RadioRequired;
                return false;
            }

            float duration = EXILEDMain.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            foreach (Player receiver in Player.List)
            {
                if (!receiver.IsAlive || receiver.CurrentItem?.Type != ItemType.Radio)
                    continue;

                SendHint(receiver, formattedMessage, duration);
            }

            response = EXILEDMain.Instance.Translation.MessageSent;
            return true;
        }
    }
#else
    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : NarrativeCommand
    {
        public override string OriginalCommand => "me";

        private string _description;
        public override string Description => _description ??= LabAPIMain.Instance?.Config?.Commands["me"] ?? "Missing description";
        public override string[] Aliases => new[] { "me" };

    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => LabAPIMain.Instance.Config.Commands["do"];
        public override string[] Aliases => new[] { "do" };
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : NarrativeCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => LabAPIMain.Instance.Config.Commands["look"];
        public override string[] Aliases => new[] { "look" };
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : NarrativeCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => LabAPIMain.Instance.Config.Commands["ooc"];
        public override string[] Aliases => new[] { "ooc" };
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DescCommand : NarrativeCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => LabAPIMain.Instance.Config.Commands["desc"];
        public override string[] Aliases => new[] { "desc" };
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CustomInfoCommand : NarrativeCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => LabAPIMain.Instance.Config.Commands["custom-info"];
        public override string[] Aliases => new[] { "custom-info" };

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = LabAPIMain.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = LabAPIMain.Instance.Config.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = LabAPIMain.Instance.Config.CustomInfoSet;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HelpCommand : NarrativeCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => LabAPIMain.Instance.Config.Commands["assist"];
        public override string[] Aliases => new[] { "assist" };

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendBroadcast(FormatMessage(player, message), 15);
            }

            response = LabAPIMain.Instance.Config.HelpRequestSent;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : NarrativeCommand
    {
        public override string OriginalCommand => "try";
        public override string Description => LabAPIMain.Instance.Config.Commands["try"];
        public override string[] Aliases => new[] { "try" };

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string resultKey = isSuccess ? "success" : "fail";
            string result = LabAPIMain.Instance.Config.TryResult[resultKey];

            return LabAPIMain.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class RadioCommand : NarrativeCommand
    {
        public override string OriginalCommand => "radio";
        public override string Description => LabAPIMain.Instance.Config.Commands["radio"];
        public override string[] Aliases => new[] { "radio" };

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.CurrentItem == null || player.CurrentItem.Type != ItemType.Radio)
            {
                response = LabAPIMain.Instance.Config.RadioRequired;
                return false;
            }

            float duration = LabAPIMain.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            foreach (Player receiver in Player.List)
            {
                if (!receiver.IsAlive || receiver.CurrentItem?.Type != ItemType.Radio)
                    continue;

                SendHint(receiver, formattedMessage, duration);
            }

            response = LabAPIMain.Instance.Config.MessageSent;
            return true;
        }
    }
#endif
}