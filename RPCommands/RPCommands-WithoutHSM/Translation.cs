using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPCommands_WithoutHSM
{
    public class Translation : ITranslation
    {
        [Description("Message shown when the round has not started.")]
        public string RoundNotStarted { get; set; } = "You cannot use this command because the round has not started yet.";

        [Description("Usage message for commands.")]
        public string Usage { get; set; } = "Usage: .{0} <message>";

        [Description("Message shown when a non-player tries to use a command.")]
        public string OnlyPlayers { get; set; } = "Only players can use this command.";

        [Description("Message shown when a command is successfully sent.")]
        public string MessageSent { get; set; } = "Message has been sent.";

        [Description("Message shown when custom info is set.")]
        public string CustomInfoSet { get; set; } = "Your custom info has been set!";

        [Description("Message shown when the set custom info is too long.")]
        public string CustomInfoTooLong
        {
            get
            {
                return Plugin.Instance == null
                    ? "Custom info is too long!"
                    : $"Custom info is too long! The maximum length is {Plugin.Instance.Config.MaxCustomInfoLength} characters.";
            }
        }

        [Description("Message shown when a command is disabled.")]
        public string CommandDisabled { get; set; } = "This command is disabled.";

        [Description("Dictionary of command names used in the system.")]
        public Dictionary<string, string> CommandNames { get; set; } = new()
        {
            { "me", "me" },
            { "do", "do" },
            { "look", "look" },
            { "ooc", "ooc" },
            { "try", "try" },
            { "desc", "desc" },
            { "custom-info", "custom-info" }
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
            { "custom-info", "Sets your custom info." }
        };

        [Description("Dictionary of results for try command.")]
        public Dictionary<string, string> TryResult { get; set; } = new()
        {
            { "success", "successfully" },
            { "fail", "unsuccessfully" }
        };
    }
}
