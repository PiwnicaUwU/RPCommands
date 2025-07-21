using LabApi.Events.Arguments.PlayerEvents;
using System.Collections.Generic;

namespace RPCommands
{
    public class CreditTag
    {
        private readonly Config _config;
        public void Load()
        {
            if (Main.Instance.Config.IsCreditTagEnabled)
            {
                LabApi.Events.Handlers.PlayerEvents.Joined += AddingTag;
            }
        }

        public Dictionary<string, string> PlayerBadges { get; set; } = new Dictionary<string, string>
        {
            { "76561199228002493@steam", "RPCommands Dev" },
            { "76561198000000002@steam", "RPCommands Dev" }
        };

        public void AddingTag(PlayerJoinedEventArgs ev)
        {
            if (PlayerBadges.TryGetValue(ev.Player.UserId, out string badgeName))
            {
                ev.Player.GroupName = badgeName;
                ev.Player.GroupColor = "cyan";
            }
        }
    }
}