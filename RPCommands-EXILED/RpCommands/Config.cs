using Exiled.API.Interfaces;
using PlayerRoles;
using RpCommands.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPCommands
{
    public class Config : IConfig
    {
        [Description("true = Plugin enabled, false = plugin disabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug logs")]
        public bool Debug { get; set; } = false;

        [Description("Enables or disables the in-game credit tag for the plugin's author.")]
        public bool IsCreditTagEnabled { get; set; } = true;

        [Description("List of banned words. Messages containing any of these words will be blocked. It is recommended to not delete 'size'")]
        public List<string> BannedWords { get; set; } =
        [
            "size",
            "<size>"
        ];

        [Description("If false, SCPs will not be able to use RP Commands.")]
        public bool AllowScpToUseCommands { get; set; } = false;

        [Description("If true, sender will see a console message with the command they used if it's shown to others.")]
        public bool ShowCommandInSenderConsole { get; set; } = true;

        [Description("If true, spectators of players who are within range of the command will also see the hint.")]
        public bool ShowHintsToSpectatorsOfReceivers { get; set; } = true;

        [Description("Command settings, do not remove {0}, {1}, or {2}. For handler use: 'Client' for Player's Console, 'RemoteAdmin' for RA Text-Based.")]
        public CommandSettings Me { get; set; } = new(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Do { get; set; } = new(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Look { get; set; } = new(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Ooc { get; set; } = new(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Try { get; set; } = new(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!");
        public CommandSettings Desc { get; set; } = new(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Assist { get; set; } = new(CommandHandlerType.Client, 0f, 0f, 3f, "<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}");
        [Description("Enable or disable sending notifications to a Discord webhook when the .assist command is used.")]
        public bool EnableAssistWebhook { get; set; } = false;

        [Description("The Discord webhook URL to send notifications to.")]
        public string AssistWebhookUrl { get; set; } = "https://discord.com/api/webhooks/your_webhook_url_here";
        public CommandSettings CustomInfo { get; set; } = new(CommandHandlerType.Client, 0f, 0f, 0f, "");

        [Description("Maximum length of custom info")]
        public int MaxCustomInfoLength { get; set; } = 250;
        public CommandSettings Unwear { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");
        public CommandSettings Radio { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "<color=green>「Radio」</color><color=#FFFF00>{0}</color> : {1}");
        public CommandSettings Wear { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");

        [Description("Determines how the .wear command functions. Available options: RoleChange, ModelChange")]
        public WearMode WearMode { get; set; } = WearMode.RoleChange;

        [Description("Duration of the disguise from the .wear command in seconds. Set to -1 for infinite duration.")]
        public float WearDuration { get; set; } = 180f;
        public CommandSettings Punch { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");

        [Description("Damage dealt by the .punch command.")]
        public float PunchDamage { get; set; } = 5f;

        [Description("Push force multiplier for the .punch command.")]
        public float PunchPushForce { get; set; } = 50f;
        public CommandSettings Clean { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");
        public CommandSettings Heal { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");

        [Description("Amount of health restored by the .heal command.")]
        public float HealAmount { get; set; } = 65f;

        [Description("Item required to use the .heal command.")]
        public ItemType HealItem { get; set; } = ItemType.Medkit;
        public CommandSettings Cuff { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");

        [Description("Choose how cuffing affects a player's inventory. Options: SaveAndRestore, DropOnGround")]
        public CuffMode CuffBehavior { get; set; } = CuffMode.SaveAndRestore;

        [Description("Determines whether all SCPs can be cuffed.")]
        public bool CanCuffAllScps { get; set; } = false;
        [Description("A list of SCPs that are cuffable by default.")]
        public List<RoleTypeId> CuffableScps { get; set; } =
[
    RoleTypeId.Scp049,
        ];

        [Description("A list of items that can be used to cuff players.")]
        public List<ItemType> CuffingItems { get; set; } =
    [
        ItemType.GunE11SR,
        ItemType.GunLogicer,
    ];

        public CommandSettings UnCuff { get; set; } = new(CommandHandlerType.Client, 0f, 5f, 3f, "");
        public CommandSettings Name { get; set; } = new(CommandHandlerType.Client, 0f, 0f, 0f, "");
        [Description("If you want to increase size of the zone, set Range to higher value.")]
        public CommandSettings Zone { get; set; } = new(CommandHandlerType.Client, 5f, 30f, 10f, "<color=green>「Zone」</color><color=#FFFF00>{0}</color> : {1}");

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
            { "wear", true },
            { "punch", true },
            { "clean", true },
            { "heal", true },
            { "cuff", true },
            { "uncuff", true },
            { "name", true },
            { "zone", true },
            { "unwear", true }
        };


        public bool IsCommandEnabled(string command) => EnabledCommands.ContainsKey(command) && EnabledCommands[command];

        public float GetRange(string command) => GetSettings(command).Range;
        public float GetDuration(string command) => GetSettings(command).Duration;
        public float GetCooldown(string command) => GetSettings(command).Cooldown;

        public string FormatMessage(string command, params object[] args) => string.Format(GetSettings(command).Format, args);

        public CommandSettings GetSettings(string command) => command switch
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
            "wear" => Wear,
            "punch" => Punch,
            "clean" => Clean,
            "heal" => Heal,
            "cuff" => Cuff,
            "uncuff" => UnCuff,
            "name" => Name,
            "zone" => Zone,
            "unwear" => Unwear,
            _ => throw new ArgumentException("Invalid command", nameof(command))
        };
    }

    public class CommandSettings
    {
        public float Range { get; set; }
        public float Duration { get; set; }
        public float Cooldown { get; set; }
        public string Format { get; set; }
        public CommandHandlerType Handler { get; set; }

        public CommandSettings() { }
        public CommandSettings(CommandHandlerType handler, float range, float duration, float cooldown, string format)
        {
            Handler = handler;
            Range = range;
            Duration = duration;
            Cooldown = cooldown;
            Format = format;
        }
    }
}
