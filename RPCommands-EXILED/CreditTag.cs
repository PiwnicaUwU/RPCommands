using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;

namespace RPCommands
{
    public class CreditTag
    {
        public void Load()
        {
            if (Main.Instance.Config.IsCreditTagEnabled)
            {
                Exiled.Events.Handlers.Player.Verified += AddingTag;
            }
        }

        public Dictionary<string, string> PlayerBadges { get; set; } = new Dictionary<string, string>
        {
            { "76561199228002493@steam", "RPCommands Dev" },
            { "76561199227305141@steam", "RPCommands Dev" }
        };

        public void AddingTag(VerifiedEventArgs ev)
        {
            if (PlayerBadges.TryGetValue(ev.Player.UserId, out string badgeName))
            {
                ev.Player.RankName = badgeName;
                ev.Player.RankColor = "cyan";
            }
        }
    }
}