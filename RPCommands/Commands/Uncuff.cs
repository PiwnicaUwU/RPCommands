using LabApi.Features.Wrappers;
using PlayerRoles;
using RPCommands.Enum;
using RPCommands.Extensions;
using UnityEngine;

namespace RPCommands.Commands
{
    internal class UncuffCommand : InternalRPCommand
    {
        public override string OriginalCommand => "uncuff";
        public override string Description => Main.Instance.Config.Translation.Commands["uncuff"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.GetTeam() == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Config.Translation.OnlyHumans;
                return false;
            }

            if (player.CurrentItem == null || !Main.Instance.Config.CuffingItems.Contains(player.CurrentItem.Type))
            {
                response = Main.Instance.Config.Translation.WeaponRequiredMessage;
                return false;
            }

            Player target = player.GetRaycastPlayer(5f);

            if (target != null && target != player)
            {

                if (Main.Instance.Config.OnlyCuffFromBehind)
                {
                    Vector3 targetToPlayer = (player.Position - target.Position).normalized;
                    float dot = Vector3.Dot(target.GameObject.transform.forward, targetToPlayer);

                    if (dot > 0.0f)
                    {
                        response = Main.Instance.Config.Translation.MustBeBehind;
                        return false;
                    }
                }

                if (target.DisarmedBy == null)
                {
                    response = string.Format(Main.Instance.Config.Translation.NotCuffed, target.Nickname);
                    return false;
                }

                target.DisarmedBy = null;
                target.IsDisarmed = false;

                switch (Main.Instance.Config.CuffBehavior)
                {
                    case CuffMode.SaveAndRestore:
                        if (CuffCommand.SavedInventories.TryGetValue(target.PlayerId, out var savedItems))
                        {
                            foreach (var item in savedItems)
                            {
                                target.AddItem(item.Type);
                            }
                            CuffCommand.SavedInventories.Remove(target.PlayerId);
                        }
                        break;

                    case CuffMode.DropOnGround:
                    default:
                        break;
                }

                target.SendHint(string.Format(Main.Instance.Config.Translation.DecuffHintTarget, player.Nickname), 5f);
                response = string.Format(Main.Instance.Config.Translation.DecuffSuccess, target.Nickname);
                return true;
            }

            response = Main.Instance.Config.Translation.NoTargetInRange;
            return false;
        }
    }
}