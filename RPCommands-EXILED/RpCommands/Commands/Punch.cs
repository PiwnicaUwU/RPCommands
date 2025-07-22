using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using PlayerStatsSystem;
using RemoteAdmin;
using RPCommands;
using System;
using UnityEngine;

namespace RpCommands.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class PunchCommand : RPCommand
    {
        public override string OriginalCommand => "punch";
        public override string Description => Main.Instance.Translation.Commands["punch"];
        public override bool AllowNoArguments => true;
        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.Team == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Translation.OnlyHumans;
                return false;
            }

            if (Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, 5f))
            {
                if (Player.Get(hit.collider.GetComponentInParent<ReferenceHub>()) is Player target && target != player)
                {
                    target.Hurt(new UniversalDamageHandler(Main.Instance.Config.PunchDamage, DeathTranslations.Unknown));

                    Vector3 pushDirection = (target.Position - player.Position).normalized + Vector3.up * 0.5f;
                    target.Position += pushDirection * Main.Instance.Config.PunchPushForce;
                    target.ShowHint(string.Format(Main.Instance.Translation.PunchHintTarget, player.Nickname), 5f);
                    response = string.Format(Main.Instance.Translation.PunchSuccess, target.Nickname);
                    return true;
                }
            }

            response = Main.Instance.Translation.NoTargetInRange;
            return false;
        }
    }
}
