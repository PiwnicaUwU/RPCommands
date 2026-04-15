using LabApi.Features.Wrappers;
using PlayerRoles;
using RPCommands.Extensions;

namespace RPCommands.Commands
{
    internal class HealCommand : InternalRPCommand
    {
        public override string OriginalCommand => "heal";
        public override string Description => Main.Instance.Config.Translation.Commands["heal"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.GetTeam() == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Config.Translation.OnlyHumans;
                return false;
            }

            if (player.CurrentItem == null)
            {
                response = Main.Instance.Config.Translation.HealItemRequired;
                return false;
            }
            Player target = player.GetRaycastPlayer(5f);

            if (target == null || target == player)
            {
                response = Main.Instance.Config.Translation.NoTargetInRange;
                return false;
            }

            ItemType itemType = player.CurrentItem.Type;
            switch (itemType)
            {
                case ItemType.Medkit:
                    UsableItem medkit = player.CurrentItem as UsableItem;
                    medkit.Use();
                    player.RemoveItem(player.CurrentItem);
                    target.SendHint(string.Format(Main.Instance.Config.Translation.HealHintTarget, player.Nickname), 5f);
                    response = string.Format(Main.Instance.Config.Translation.HealSuccess, target.Nickname);
                    return true;

                case ItemType.Adrenaline:
                    UsableItem adrenaline = player.CurrentItem as UsableItem;
                    adrenaline.Use();
                    target.SendHint(Main.Instance.Config.Translation.AdrenalineHeal, 5f);
                    response = string.Format(Main.Instance.Config.Translation.HealSuccess, target.Nickname);
                    return true;

                case ItemType.Painkillers:
                    UsableItem painkillers = player.CurrentItem as UsableItem;
                    painkillers.Use();
                    target.SendHint(Main.Instance.Config.Translation.PainkillersHeal, 5f);
                    response = string.Format(Main.Instance.Config.Translation.HealSuccess, target.Nickname);
                    return true;

                default:
                    response = Main.Instance.Config.Translation.HealItemRequired;
                    return false;
            }
        }
    }
}