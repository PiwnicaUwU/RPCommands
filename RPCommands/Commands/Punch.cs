using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RPCommands.Extensions;
using UnityEngine;

namespace RPCommands.Commands
{

    public class PunchCommand : RPCommand
    {
        public override string OriginalCommand => "punch";
        public override string Description => Main.Instance.Config.Translation.Commands["punch"];
        public override bool AllowNoArguments => true;
        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.GetTeam() == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Config.Translation.OnlyHumans;
                return false;
            }

            Player target = player.GetRaycastPlayer(5f);
            if (target != null && target != player)
            {
                target.Damage(new UniversalDamageHandler(Main.Instance.Config.PunchDamage, DeathTranslations.Unknown));

                Vector3 pushDirection = (target.Position - player.Position).normalized + Vector3.up * 0.5f;
                if (target.RoleBase is IFpcRole fpcRole)
                {
                    fpcRole.FpcModule.CharController.Move(pushDirection * Main.Instance.Config.PunchPushForce * Time.deltaTime);
                }
                target.SendHint(string.Format(Main.Instance.Config.Translation.PunchHintTarget, player.Nickname), 5f);
                response = string.Format(Main.Instance.Config.Translation.PunchSuccess, target.Nickname);
                return true;
            }

            response = Main.Instance.Config.Translation.NoTargetInRange;
            return false;
        }
    }
}