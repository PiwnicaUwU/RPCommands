using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using RPCommands.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace RPCommands.Commands;

internal sealed class PunchCommand : InternalRPCommand
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

            //Vector3 pushDirection = (target.Position - player.Position).normalized + Vector3.up * 0.5f;
            Timing.RunCoroutine(PushPlayer(target, player));
            target.SendHint(string.Format(Main.Instance.Config.Translation.PunchHintTarget, player.Nickname), 5f);
            response = string.Format(Main.Instance.Config.Translation.PunchSuccess, target.Nickname);
            return true;
        }

        response = Main.Instance.Config.Translation.NoTargetInRange;
        return false;
    }


    private IEnumerator<float> PushPlayer(Player target, Player pusher)
    {
        Vector3 pushed = pusher.Camera.forward * Main.Instance.Config.PunchPushForce;
        Vector3 endPosition = target.Position + new Vector3(pushed.x, 0, pushed.z);

        for (int i = 1; i < Main.Instance.Config.Iterations; i++)
        {
            Vector3 newPos = Vector3.MoveTowards(target.Position, endPosition, Main.Instance.Config.PunchPushForce / Main.Instance.Config.Iterations);

            if (Physics.Linecast(target.Position, newPos))
            {
                yield break;
            }
            target.Position = newPos;
            yield return Timing.WaitForOneFrame;
        }
    }
}