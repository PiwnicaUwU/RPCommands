using Exiled.API.Features;
using PlayerRoles;
using RpCommands.Extensions;
using RPCommands;
using System;

namespace RpCommands.Commands
{

    public class HealCommand : RPCommand
    {
        public override string OriginalCommand => "heal";
        public override string Description => Main.Instance.Translation.Commands["heal"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.Team == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Translation.OnlyHumans;
                return false;
            }

            if (player.CurrentItem == null || player.CurrentItem.Type != Main.Instance.Config.HealItem)
            {
                response = Main.Instance.Translation.HealItemRequired;
                return false;
            }

            Player target = player.GetRaycastPlayer(5f);

            if (target != null && target != player)
            {
                target.Health = Math.Min(target.MaxHealth, target.Health + Main.Instance.Config.HealAmount);
                player.RemoveItem(player.CurrentItem, true);
                target.ShowHint(string.Format(Main.Instance.Translation.HealHintTarget, player.Nickname), 5f);
                response = string.Format(Main.Instance.Translation.HealSuccess, target.Nickname);
                return true;
            }

            response = Main.Instance.Translation.NoTargetInRange;
            return false;
        }
    }
}
