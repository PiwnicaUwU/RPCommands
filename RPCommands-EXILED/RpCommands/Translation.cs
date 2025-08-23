using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RPCommands
{
    public class Translation : ITranslation
    {
        [Description("Message shown when .name command is used.")]
        public string NameResponse { get; set; } = "Your name is {0} and your custom info is {1}";

        [Description("Message shown to the player after successfully creating a zone.")]
        public string ZoneSuccess { get; set; } = "Zone successfully created. It will disappear in {0} seconds.";

        [Description("Message shown when you try to cuff player without holding a weapon.")]
        public string WeaponRequiredMessage { get; set; } = "You must be holding a weapon to use this command!";
        [Description("Message shown when player/scp is cuffed")]

        public string AlreadyCuffed = "{0} is already cuffed!";

        [Description("Message shown if you cannot cuff scp")]

        public string CannotCuffScp = "you cannot cuff this scp!";

        [Description("Hint shown if you get cuffed")]

        public string CuffHintTarget = "you got cuffed by {0}";

        [Description("Message shown when you cuff someone")]

        public string CuffSuccess = "you cuffed {0}";

        [Description("Message shown when target is not cuffed")]

        public string NotCuffed = "{0} is not cuffed";

        [Description("Message shown when target is uncuffed")]

        public string DecuffHintTarget = "{0} uncuffed you";

        [Description("Message shown when player uncuff target")]

        public string DecuffSuccess = "you uncuffed {0}";

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
        [Description("The format of the message sent to the webhook for the .assist command. {0} = Player Nickname, {1} = Player UserId, {2} = Message")]
        public string AssistWebhookMessageFormat { get; set; } = "🚨 **Assist Request** 🚨\\n\\n**Player:** {0} ({1})\\n**Message:**\\n```\\n{2}\\n```";

        [Description("Message shown when the player tries to use the .wear command with no ragdolls in range.")]
        public string NoDeadBodyFound { get; set; } = "No dead body found.";

        [Description("Message shown when the player fails to put on the clothes for an unspecified reason.")]
        public string WoreFailure { get; set; } = "You cannot put on these clothes.";

        [Description("Success message displayed to the player after successfully using the .wear command.")]
        public string Wore { get; set; } = "You put on the clothes of the deceased.";

        [Description("Message shown when the player attempts to wear the ragdoll of an SCP.")]
        public string ScpClothesNotAllowed { get; set; } = "You can't wear SCP ragdolls.";

        [Description("Message displayed when an SCP player tries to use the .wear command.")]
        public string ScpCantwear { get; set; } = "SCPs cannot wear clothes.";

        [Description("Message shown to the player when their disguise from the .wear command expires.")]
        public string DisguiseWornOff { get; set; } = "Your disguise has worn off.";

        [Description("Message shown when there is no target in range for a command.")]
        public string NoTargetInRange { get; set; } = "No target in range.";

        [Description("Message for punch command cooldown.")]
        public string PunchCooldown { get; set; } = "You can use this command again in {0} seconds.";

        [Description("Message for the player who successfully used the punch command.")]
        public string PunchSuccess { get; set; } = "You successfully punched <color=green>{0}</color>!";

        [Description("Hint message for the player who got punched.")]
        public string PunchHintTarget { get; set; } = "<color=red>You were punched by {0}</color>!";

        [Description("Message when no ragdoll is found nearby for the clean command.")]
        public string NoRagdollNearby { get; set; } = "There is no body nearby to clean up.";

        [Description("Message for successfully cleaning a ragdoll.")]
        public string CleanSuccess { get; set; } = "The body has been cleaned up.";

        [Description("Message when a player tries to use the heal command without holding the required item.")]
        public string HealItemRequired { get; set; } = "You must be holding a Medkit to use this command.";

        [Description("Message for the player who successfully used the heal command.")]
        public string HealSuccess { get; set; } = "You have healed <color=green>{0}</color>.";

        [Description("Hint message for the player who got healed.")]
        public string HealHintTarget { get; set; } = "<color=green>You have been healed by {0}.</color>";

        [Description("Message shown when player is scp and try use unwear command.")]
        public string ScpCantUnwear { get; set; } = "You can't use this command as Scp.";

        [Description("Message shown when you are not disguised.")]
        public string NotDisguised { get; set; } = "You are not disguised.";

        [Description("Message shown in result when player use unwear command.")]
        public string Unwore { get; set; } = "Disguise was worn off.";

        [Description("Message shown in result of command usage failure.")]
        public string UnworeFailure { get; set; } = "Discguise cannot be worn off.";

        [Description("Hint message shown in result of command usage sucess.")]
        public string DisguiseRemoved { get; set; } = "Disguise was sucessfully worn off.";


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
            { "punch", "punch" },
            { "clean", "clean" },
            { "heal", "heal" },
            { "cuff", "cuff" },
            { "uncuff", "uncuff" },
            { "name", "name" },
            { "zone", "zone" },
            { "unwear", "unwear" }
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
            { "wear", "allows to wear a dead body." },
            { "punch", "Punches a player" },
            { "clean", "Cleans up the nearest ragdoll." },
            { "heal", "Use a Medkit to heal player." },
            { "cuff", "cuffs player." },
            { "uncuff", "uncuffs player." },
            { "name", "shows your name and custom info." },
            { "zone", "Creates a zone." },
            { "unwear", "allows player to wear off disguise" }
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
