using LabApi.Features.Wrappers;
using PlayerRoles;
using RPCommands.Enum;
using RPCommands.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPCommands.Commands
{

    public class CuffCommand : RPCommand
    {
        public override string OriginalCommand => "cuff";
        public override string Description => Main.Instance.Config.Translation.Commands["cuff"];
        public override bool AllowNoArguments => true;

        public static readonly Dictionary<int, List<Item>> SavedInventories = [];
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

                if (target.DisarmedBy != null)
                {
                    response = string.Format(Main.Instance.Config.Translation.AlreadyCuffed, target.Nickname);
                    return false;
                }

                if (target.Role.GetTeam() == Team.SCPs && !Main.Instance.Config.CanCuffAllScps && !Main.Instance.Config.CuffableScps.Contains(target.Role))
                {
                    response = Main.Instance.Config.Translation.CannotCuffScp;
                    return false;
                }

                switch (Main.Instance.Config.CuffBehavior)
                {
                    case CuffMode.SaveAndRestore:
                        if (target.Items.Any())
                        {
                            SavedInventories[target.PlayerId] = [.. target.Items];
                            target.ClearItems();
                        }
                        break;

                    case CuffMode.DropOnGround:
                        target.DropAllItems();
                        break;
                }

                if (target.Role.GetTeam() == player.Role.GetTeam()) // Game doesn't allow handcuffing same side players, so we force it
                {
                    target.IsDisarmed = true;
                }
                else
                {
                    target.DisarmedBy = player;
                }

                target.SendHint(string.Format(Main.Instance.Config.Translation.CuffHintTarget, player.Nickname), 5f);
                response = string.Format(Main.Instance.Config.Translation.CuffSuccess, target.Nickname);
                return true;
            }

            response = Main.Instance.Config.Translation.NoTargetInRange;
            return false;
        }
    }
}