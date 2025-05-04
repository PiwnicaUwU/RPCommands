using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPCommands
{
    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs - don't work")]
        public bool Debug { get; set; } = false;

        [Description("If false, SCPs will not be able to use narrative commands.")]
        public bool AllowScpToUseCommands { get; set; } = false;

        [Description("Command settings, do not remove {0}, {1}, or {2}")]
        public CommandSettings Me { get; set; } = new(15f, 5f, 3f, "<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Do { get; set; } = new(15f, 5f, 3f, "<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Look { get; set; } = new(15f, 5f, 3f, "<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Ooc { get; set; } = new(15f, 5f, 3f, "<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Try { get; set; } = new(15f, 5f, 3f, "<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!");
        public CommandSettings Desc { get; set; } = new(15f, 5f, 3f, "<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Assist { get; set; } = new(0f, 0f, 3f, "<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}");

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
            { "assist", true }
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
