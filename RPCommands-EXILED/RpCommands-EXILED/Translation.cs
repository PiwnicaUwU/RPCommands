using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RPCommands
{
    public class Translation : ITranslation
    {
        [Description("Message shown when the round has not started.")]
        public string RoundNotStarted { get; set; } = "You cannot use this command because the round has not started yet.";

        [Description("Usage message for commands.")]
        public string Usage { get; set; } = "Usage: .{0} <message>";

        [Description("Message shown when banned word is detected.")]
        public string BannedWordDetected { get; set; } = "Your message contains a banned word and has been blocked.";

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

        [Description("Dictionary of command names used in the system.")]

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
            { "radio", "radio" },
            { "wear", "wear" },
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
            { "radio", "Sends a radio message to other players holding radios." },
            { "wear", "xxx" },
        };

        [Description("Dictionary of results for try command.")]
        public Dictionary<string, string> TryResult { get; set; } = new()
        {
            { "success", "successfully" },
            { "fail", "unsuccessfully" }
        };

        public string GetOriginalCommandName(string translatedName)
        {
            return Main.Instance.Translation.CommandNames
                .FirstOrDefault(x => x.Value == translatedName).Key ?? translatedName;
        }

    }
}
