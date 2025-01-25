using System;
using CommandSystem;
using Exiled.API.Features;
using System.Linq;
using UnityEngine;
using Exiled.API.Interfaces;
using RemoteAdmin;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using System.ComponentModel;
using MEC;
namespace RPCommands
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "RPCommands";
        public override string Author => ".Piwnica";
        public override Version Version => new Version(1, 0, 0);

        public override void OnEnabled()
        {
            Log.Info("Plugin successfully enabled!");
            Instance = this;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
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
            string format = Plugin.Instance.Config.GetFormat("try");
            return string.Format(format, player.Nickname, message, result);
        }
    }

    public abstract class NarrativeCommand : ICommand
    {
        public abstract string Command { get; }
        public virtual string[] Aliases => new string[0];
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

            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Only players can use this command.";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            string message = string.Join(" ", arguments);
            float range = Plugin.Instance.Config.GetRange(Command);
            float duration = Plugin.Instance.Config.GetDuration(Command);
            string formattedMessage = FormatMessage(player, message);

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = "Message has been sent.";
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            string format = Plugin.Instance.Config.GetFormat(Command);
            return string.Format(format, player.Nickname, message);
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

    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Nothing important, additional logs (might not work)")]
        public bool Debug { get; set; } = false;

        [Description("The range of narrative commands (in meters). Players within this radius will receive messages related to the given command.")]
        public float MeRange { get; set; } = 15f;
        public float DoRange { get; set; } = 15f;
        public float LookRange { get; set; } = 15f;
        public float OocRange { get; set; } = 15f;
        public float TryRange { get; set; } = 15f;

        [Description("Duration of hints for narrative commands (in seconds).")]
        public float MeDuration { get; set; } = 5f;
        public float DoDuration { get; set; } = 5f;
        public float LookDuration { get; set; } = 5f;
        public float OocDuration { get; set; } = 5f;
        public float TryDuration { get; set; } = 5f;

        [Description("You can edit hint formatting and colors. Do not edit '{1}' - message or '{0}' - player, otherwise the plugin will break!")]
        public string MeFormat { get; set; } = "<b><color=green>[Me]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>";
        public string DoFormat { get; set; } = "<b><color=green>[Do]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>";
        public string LookFormat { get; set; } = "<b><color=green>[Look]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>";
        public string OocFormat { get; set; } = "<b><color=green>[Ooc]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>";
        public string TryFormat { get; set; } = "<b><color=green>[Try]</color> <color=#FFFF00>{0}</color> : tried to {1} and {2} did it!</b></size></align>";

        public string GetFormat(string command) => command switch
        {
            "me" => MeFormat,
            "do" => DoFormat,
            "look" => LookFormat,
            "ooc" => OocFormat,
            "try" => TryFormat,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };

        public float GetDuration(string command) => command switch
        {
            "me" => MeDuration,
            "do" => DoDuration,
            "look" => LookDuration,
            "ooc" => OocDuration,
            "try" => TryDuration,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };

        public float GetRange(string command) => command switch
        {
            "me" => MeRange,
            "do" => DoRange,
            "look" => LookRange,
            "ooc" => OocRange,
            "try" => TryRange,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };
    }
}
