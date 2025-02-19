using Exiled.API.Interfaces;
using System.ComponentModel;
using System;

namespace RPCommands_WithoutHSM
{
    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs - don't work")]
        public bool Debug { get; set; } = false;

        [Description("Command settings, do not remove {0}, {1} or {2}")]
        public CommandSettings Me { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Me]</color> <color=#FFFF00>{0}</color> : {1}</size></align>");
        public CommandSettings Do { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Do]</color> <color=#FFFF00>{0}</color> : {1}</size></align>");
        public CommandSettings Look { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Look]</color> <color=#FFFF00>{0}</color> : {1}</size></align>");
        public CommandSettings Ooc { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Ooc]</color> <color=#FFFF00>{0}</color> : {1}</size></align>");
        public CommandSettings Try { get; set; } = new CommandSettings(15f, 5f, "<size=25><align=left><color=green>[Try]</color> <color=#FFFF00>{0}</color> : tried to {1} and {2} did it!</size></align>");

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
