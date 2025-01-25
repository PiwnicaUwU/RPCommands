using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using RemoteAdmin;
using System.Linq;
using UnityEngine;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace RPCommands_WithoutHSM
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name => "RPCommands-NOHSM";
        public override string Author => ".Piwnica";
        public override Version Version => new(1, 0, 1);

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
            return Plugin.Instance.Config.FormatMessage(Command, player.Nickname, message, result);
        }
    }

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
            float range = Plugin.Instance.Config.GetRange(Command);
            float duration = Plugin.Instance.Config.GetDuration(Command);
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
                player.ShowHint(message, duration);
            }

            sender.ShowHint(message, duration);
        }
    }

    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs")]
        public bool Debug { get; set; } = false;

        [Description("Command settings, do not remove {0}, {1} or {2}")]
        public CommandSettings Me { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Me]</color> <color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Do { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Do]</color> <color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Look { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Look]</color> <color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Ooc { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Ooc]</color> <color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Try { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Try]</color> <color=#FFFF00>{0}</color> : tried to {1} and {2} did it!");

        public float GetRange(string command) => GetSettings(command).Range;
        public float GetDuration(string command) => GetSettings(command).Duration;

        public string FormatMessage(string command, params object[] args) => string.Format(GetSettings(command).Format, args);

        private CommandSettings GetSettings(string command) => command switch
        {
            "me" => Me,
            "do" => Do,
            "look" => Look,
            "ooc" => Ooc,
            "try" => Try,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };
    }

    public class CommandSettings
    {
        public float Range { get; }
        public float Duration { get; }
        public string Format { get; }

        public CommandSettings(float range, float duration, string format)
        {
            Range = range;
            Duration = duration;
            Format = format;
        }
    }
}
