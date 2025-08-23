using Exiled.API.Features;
using PlayerRoles;
using RpCommands.Enum;
using RpCommands.Extensions;
using RPCommands;

namespace RpCommands.Commands
{

    public class UncuffCommand : RPCommand
    {
        public override string OriginalCommand => "uncuff";
        public override string Description => Main.Instance.Translation.Commands["uncuff"];
        public override bool AllowNoArguments => true;

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

            Player target = player.GetRaycastPlayer(5f);

            if (target != null && target != player)
            {
                if (target.Cuffer == null)
                {
                    response = string.Format(Main.Instance.Translation.NotCuffed, target.Nickname);
                    return false;
                }

                target.Cuffer = null;
                target.RemoveHandcuffs();

                switch (Main.Instance.Config.CuffBehavior)
                {
                    case CuffMode.SaveAndRestore:
                        if (CuffCommand.SavedInventories.TryGetValue(target.Id, out var savedItems))
                        {
                            foreach (var item in savedItems)
                            {
                                target.AddItem(item.Type);
                            }
                            CuffCommand.SavedInventories.Remove(target.Id);
                        }
                        break;

                    case CuffMode.DropOnGround:
                    default:
                        break;
                }

                target.ShowHint(string.Format(Main.Instance.Translation.DecuffHintTarget, player.Nickname), 5f);
                response = string.Format(Main.Instance.Translation.DecuffSuccess, target.Nickname);
                return true;
            }

            response = Main.Instance.Translation.NoTargetInRange;
            return false;
        }
    }
}