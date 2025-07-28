using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using RpCommands.Enum;
using RPCommands;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RpCommands.Commands
{

    public class CuffCommand : RPCommand
    {
        public override string OriginalCommand => "cuff";
        public override string Description => Main.Instance.Translation.Commands["cuff"];
        public override bool AllowNoArguments => true;

        public static readonly Dictionary<int, List<Item>> SavedInventories = new Dictionary<int, List<Item>>();
        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.Team == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Translation.OnlyHumans;
                return false;
            }

            if (player.CurrentItem == null || !Main.Instance.Config.CuffingItems.Contains(player.CurrentItem.Type))
            {
                response = Main.Instance.Translation.WeaponRequiredMessage;
                return false;
            }

            if (Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, 5f))
            {
                if (Player.Get(hit.collider.GetComponentInParent<ReferenceHub>()) is Player target && target != player)
                {
                    if (target.Cuffer != null)
                    {
                        response = string.Format(Main.Instance.Translation.AlreadyCuffed, target.Nickname);
                        return false;
                    }

                    if (target.Role.Team == Team.SCPs && !Main.Instance.Config.CanCuffAllScps && !Main.Instance.Config.CuffableScps.Contains(target.Role))
                    {
                        response = Main.Instance.Translation.CannotCuffScp;
                        return false;
                    }

                    switch (Main.Instance.Config.CuffBehavior)
                    {
                        case CuffMode.SaveAndRestore:
                            if (target.Items.Any())
                            {
                                SavedInventories[target.Id] = [.. target.Items];
                                target.ClearItems();
                            }
                            break;

                        case CuffMode.DropOnGround:
                            target.DropItems();
                            break;
                    }


                    target.Cuffer = player;
                    target.ShowHint(string.Format(Main.Instance.Translation.CuffHintTarget, player.Nickname), 5f);
                    response = string.Format(Main.Instance.Translation.CuffSuccess, target.Nickname);
                    return true;
                }
            }

            response = Main.Instance.Translation.NoTargetInRange;
            return false;
        }
    }
}