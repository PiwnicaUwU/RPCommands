﻿#if EXILED
using Exiled.API.Interfaces;
using System;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RPCommands
{
#if EXILED
    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs - don't work")]
        public bool Debug { get; set; } = false;

        [Description("If false, SCPs will not be able to use narrative commands.")]
        public bool AllowScpToUseCommands { get; set; } = false;

        [Description("If true, sender will see a console message with the command they used if it's shown to others.")]
        public bool ShowCommandInSenderConsole { get; set; } = true;

        [Description("If true, spectators of the player who used the command will also see the hint.")]
        public bool ShowHintsToSpectatorsOfSender { get; set; } = true;

        [Description("If true, spectators of players who are within range of the command will also see the hint.")]
        public bool ShowHintsToSpectatorsOfReceivers { get; set; } = true;

        [Description("Command settings, do not remove {0}, {1}, or {2}")]
        public CommandSettings Me { get; set; } = new(15f, 5f, 3f, "<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Do { get; set; } = new(15f, 5f, 3f, "<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Look { get; set; } = new(15f, 5f, 3f, "<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Ooc { get; set; } = new(15f, 5f, 3f, "<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Try { get; set; } = new(15f, 5f, 3f, "<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!");
        public CommandSettings Desc { get; set; } = new(15f, 5f, 3f, "<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Assist { get; set; } = new(0f, 0f, 3f, "<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}");
        public CommandSettings CustomInfo { get; set; } = new(0f, 0f, 0f, "");
        public CommandSettings Radio { get; set; } = new(0f, 5f, 3f, "<color=green>「Radio」</color><color=#FFFF00>{0}</color> : {1}");


        [Description("Maximum length of custom info")]
        public int MaxCustomInfoLength { get; set; } = 250;

        [Description("Enable or disable specific commands")]
        public Dictionary<string, bool> EnabledCommands { get; set; } = new()
        {
            { "me", true },
            { "do", true },
            { "look", true },
            { "ooc", true },
            { "try", true },
            { "desc", true },
            { "custom-info", true },
            { "assist", true },
            { "radio", true },
        };


        public bool IsCommandEnabled(string command) => EnabledCommands.ContainsKey(command) && EnabledCommands[command];

        public float GetRange(string command) => GetSettings(command).Range;
        public float GetDuration(string command) => GetSettings(command).Duration;
        public float GetCooldown(string command) => GetSettings(command).Cooldown;

        public string FormatMessage(string command, params object[] args) => string.Format(GetSettings(command).Format, args);

        private CommandSettings GetSettings(string command) => command switch
        {
            "me" => Me,
            "do" => Do,
            "look" => Look,
            "ooc" => Ooc,
            "try" => Try,
            "desc" => Desc,
            "assist" => Assist,
            "custom-info" => CustomInfo,
            "radio" => Radio,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };
    }

    public class CommandSettings
    {
        public float Range { get; set; }
        public float Duration { get; set; }
        public float Cooldown { get; set; }
        public string Format { get; set; }

        public CommandSettings() { }
        public CommandSettings(float range, float duration, float cooldown, string format)
        {
            Range = range;
            Duration = duration;
            Cooldown = cooldown;
            Format = format;
        }
    }
}
#else
    public class ConfigLabAPI
    {
        [Description("If false, SCPs will not be able to use narrative commands.")]
        public bool AllowScpToUseCommands { get; set; } = false;

        [Description("If true, sender will see a console message with the command they used if it's shown to others.")]
        public bool ShowCommandInSenderConsole { get; set; } = true;

        [Description("If true, spectators of the player who used the command will also see the hint.")]
        public bool ShowHintsToSpectatorsOfSender { get; set; } = true;

        [Description("If true, spectators of players who are within range of the command will also see the hint.")]
        public bool ShowHintsToSpectatorsOfReceivers { get; set; } = true;

        [Description("Command settings, do not remove {0}, {1}, or {2}")]
        public CommandSettings Me { get; set; } = new(15f, 5f, 3f, "<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Do { get; set; } = new(15f, 5f, 3f, "<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Look { get; set; } = new(15f, 5f, 3f, "<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Ooc { get; set; } = new(15f, 5f, 3f, "<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Try { get; set; } = new(15f, 5f, 3f, "<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!");
        public CommandSettings Desc { get; set; } = new(15f, 5f, 3f, "<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Assist { get; set; } = new(0f, 0f, 3f, "<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}");
        public CommandSettings CustomInfo { get; set; } = new(0f, 0f, 0f, "");
        public CommandSettings Radio { get; set; } = new(0f, 5f, 3f, "<color=green>「Radio」</color><color=#FFFF00>{0}</color> : {1}");


        [Description("Maximum length of custom info")]
        public int MaxCustomInfoLength { get; set; } = 250;

        [Description("Enable or disable specific commands")]
        public Dictionary<string, bool> EnabledCommands { get; set; } = new()
        {
            { "me", true },
            { "do", true },
            { "look", true },
            { "ooc", true },
            { "try", true },
            { "desc", true },
            { "custom-info", true },
            { "assist", true },
            { "radio", true },
        };

        public bool IsCommandEnabled(string command) => EnabledCommands.ContainsKey(command) && EnabledCommands[command];
        public float GetRange(string command) => GetSettings(command).Range;
        public float GetDuration(string command) => GetSettings(command).Duration;
        public float GetCooldown(string command) => GetSettings(command).Cooldown;

        public string FormatMessage(string command, params object[] args) => string.Format(GetSettings(command).Format, args);

        private CommandSettings GetSettings(string command) => command switch
        {
            "me" => Me,
            "do" => Do,
            "look" => Look,
            "ooc" => Ooc,
            "try" => Try,
            "desc" => Desc,
            "assist" => Assist,
            "custom-info" => CustomInfo,
            "radio" => Radio,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };

        [Description("Message shown when the round has not started.")]
        public string RoundNotStarted { get; set; } = "You cannot use this command because the round has not started yet.";

        [Description("Usage message for commands.")]
        public string Usage { get; set; } = "Usage: .{0} <message>";

        [Description("Message shown when a non-player tries to use a command.")]
        public string OnlyPlayers { get; set; } = "Only players can use this command.";

        [Description("Message shown when a non-human tries to use a command.")]
        public string OnlyHumans { get; set; } = "Only humans can use this command.";

        [Description("Message shown when Command Sender is not alive.")]
        public string OnlyAlive { get; set; } = "You must be alive to use this command.";

        [Description("Cooldown message when a player tries to use a command too quickly.")]
        public string CommandCooldown { get; set; } = "You must wait {0} seconds before using the command again.";

        [Description("Message shown when a command is successfully sent.")]
        public string MessageSent { get; set; } = "Message has been sent.";

        [Description("Message shown when custom info is set.")]
        public string CustomInfoSet { get; set; } = "Your custom info has been set!";

        [Description("Message shown when the set custom info is too long.")]
        public string CustomInfoTooLong { get; set; } = "Custom info is too long!";

        [Description("Message shown when a command is disabled.")]
        public string CommandDisabled { get; set; } = "This command is disabled.";

        [Description("Message shown when a player tries to use radio command without holding a radio.")]
        public string RadioRequired { get; set; } = "You must be holding a radio to use this command.";

        [Description("Message shown when a assist request is sent.")]
        public string HelpRequestSent { get; set; } = "Your assist request has been sent to the staff.";

        [Description("Broken on LabAPI version...")]
        public Dictionary<string, string> CommandNames { get; set; } = new()
        {
            { "me", "me" },
            { "do", "do" },
            { "look", "look" },
            { "ooc", "ooc" },
            { "try", "try" },
            { "desc", "desc" },
            { "custom-info", "custom-info" },
            { "assist", "assist" },
            { "radio", "radio" }
        };

        [Description("Dictionary of command descriptions.")]
        public Dictionary<string, string> Commands { get; set; } = new()
        {
            { "me", "Narrative command 'Me'." },
            { "do", "Narrative command 'Do'." },
            { "look", "Narrative command 'Look'." },
            { "ooc", "Narrative command 'Ooc'." },
            { "try", "Narrative command 'Try'." },
            { "desc", "Narrative command 'Desc'." },
            { "custom-info", "Sets your custom info." },
            { "assist", "Sends a assist request to the staff chat." },
            { "radio", "Sends a radio message to other players holding radios." }
        };

        [Description("Dictionary of results for try command.")]
        public Dictionary<string, string> TryResult { get; set; } = new()
        {
            { "success", "successfully" },
            { "fail", "unsuccessfully" }
        };

        public string GetOriginalCommandName(string translatedName)
        {
            return CommandNames
                .FirstOrDefault(x => x.Value == translatedName).Key ?? translatedName;
        }
    }
}
public class CommandSettings
{
    public float Range { get; set; }
    public float Duration { get; set; }
    public float Cooldown { get; set; }
    public string Format { get; set; }

    public CommandSettings() { }
    public CommandSettings(float range, float duration, float cooldown, string format)
    {
        Range = range;
        Duration = duration;
        Cooldown = cooldown;
        Format = format;
    }
}
#endif