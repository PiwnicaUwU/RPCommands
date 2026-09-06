using LabApi.Features.Wrappers;
using PlayerRoles;
using RPCommands.Extensions;

namespace RPCommands.Commands;

internal sealed class HealCommand : InternalRPCommand
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

        Item currentItem = player.CurrentItem;

        if (currentItem == null)
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

        ItemType itemType = currentItem.Type;

        switch (itemType)
        {
            case ItemType.Medkit:
                if (currentItem is UsableItem medkit)
                {
                    medkit.Use();
                    player.RemoveItem(currentItem);
                    target.SendHint(string.Format(Main.Instance.Config.Translation.HealHintTarget, player.Nickname), 5f);
                    response = string.Format(Main.Instance.Config.Translation.HealSuccess, target.Nickname);
                    return true;
                }
                break;

            case ItemType.Adrenaline:
                if (currentItem is UsableItem adrenaline)
                {
                    adrenaline.Use();
                    player.RemoveItem(currentItem);
                    target.SendHint(Main.Instance.Config.Translation.AdrenalineHeal, 5f);
                    response = string.Format(Main.Instance.Config.Translation.HealSuccess, target.Nickname);
                    return true;
                }
                break;

            case ItemType.Painkillers:
                if (currentItem is UsableItem painkillers)
                {
                    painkillers.Use();
                    player.RemoveItem(currentItem);
                    target.SendHint(Main.Instance.Config.Translation.PainkillersHeal, 5f);
                    response = string.Format(Main.Instance.Config.Translation.HealSuccess, target.Nickname);
                    return true;
                }
                break;
        }

        response = Main.Instance.Config.Translation.HealItemRequired;
        return false;
    }
}
