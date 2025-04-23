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

namespace RPCommands
{
    public abstract class NarrativeCommand : ICommand
    {
        public abstract string OriginalCommand { get; }
        public virtual string[] Aliases => Array.Empty<string>();
        public abstract string Description { get; }

        public string Command => Plugin.Instance.Translation.CommandNames.TryGetValue(OriginalCommand, out string translatedName) ? translatedName : OriginalCommand;

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

            if (player.IsScp)
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
            string message = string.Join(" ", arguments);
            float range = Plugin.Instance.Config.GetRange(OriginalCommand);
            float duration = Plugin.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = Plugin.Instance.Translation.MessageSent;
            Request.SetLastMessage(player, OriginalCommand, string.Join(" ", arguments));
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return Plugin.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
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
    public class CustomInfoCommand : ICommand
    {
        public string Command => Plugin.Instance.Translation.CommandNames.TryGetValue("custom-info", out string translatedName) ? translatedName : "custom-info";
        public string[] Aliases => Array.Empty<string>();
        public string Description => Plugin.Instance.Translation.Commands["custom-info"];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Config.IsCommandEnabled("custom-info"))
            {
                response = Plugin.Instance.Translation.CommandDisabled;
                return false;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = Plugin.Instance.Translation.OnlyPlayers;
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);

            if (arguments.Count < 1)
            {
                response = string.Format(Plugin.Instance.Translation.Usage, Command);
                return false;
            }

            string customInfo = string.Join(" ", arguments);
            int maxLength = Plugin.Instance.Config.MaxCustomInfoLength;

            if (customInfo.Length > maxLength)
            {
                response = Plugin.Instance.Translation.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = customInfo;
            response = Plugin.Instance.Translation.CustomInfoSet;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HelpCommand : ICommand
    {
        public string Command => Plugin.Instance.Translation.CommandNames.TryGetValue("assist", out string translatedName) ? translatedName : "assist";
        public string[] Aliases => Array.Empty<string>();
        public string Description => Plugin.Instance.Translation.Commands["assist"];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Config.IsCommandEnabled("assist"))
            {
                response = Plugin.Instance.Translation.CommandDisabled;
                return false;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = Plugin.Instance.Translation.OnlyPlayers;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = string.Format(Plugin.Instance.Translation.Usage, Command);
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            string message = string.Join(" ", arguments);
            string formattedMessage = Plugin.Instance.Config.FormatMessage("assist", player.Nickname, message);

            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendStaffMessage(formattedMessage);
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
