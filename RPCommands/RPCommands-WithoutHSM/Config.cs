using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPCommands_WithoutHSM
{
    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs - don't work")]
        public bool Debug { get; set; } = false;

        [Description("Command settings, do not remove {0}, {1}, or {2}")]
        public CommandSettings Me { get; set; } = new(15f, 5f, "<size=25><align=left><color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}</align></size>");
        public CommandSettings Do { get; set; } = new(15f, 5f, "<size=25><align=left><color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}</align></size>");
        public CommandSettings Look { get; set; } = new(15f, 5f, "<size=25><align=left><color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}</align></size>");
        public CommandSettings Ooc { get; set; } = new(15f, 5f, "<size=25><align=left><color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}</align></size>");
        public CommandSettings Try { get; set; } = new(15f, 5f, "<size=25><align=left><color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!</align></size>");
        public CommandSettings Desc { get; set; } = new(15f, 5f, "<size=25><align=left><color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}</align></size>");

        [Description("Enable or disable specific commands")]
        public Dictionary<string, bool> EnabledCommands { get; set; } = new()
        {
            { "me", true },
            { "do", true },
            { "look", true },
            { "ooc", true },
            { "try", true },
            { "desc", true },
            { "custom-info", true }
        };

        public bool IsCommandEnabled(string command) => EnabledCommands.ContainsKey(command) && EnabledCommands[command];

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
            "desc" => Desc,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };
    }

    public class CommandSettings
    {
        public float Range { get; set; }
        public float Duration { get; set; }
        public string Format { get; set; }

        public CommandSettings() { }
        public CommandSettings(float range, float duration, string format)
        {
            Range = range;
            Duration = duration;
            Format = format;
        }
    }
}
