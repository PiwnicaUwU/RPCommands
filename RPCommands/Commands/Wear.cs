using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using LabApiExtensions.Managers;
using MEC;
using PlayerRoles;
using PlayerRoles.Spectating;
using UnityEngine;

namespace RPCommands.Commands
{
    public class WearCommand : RPCommand
    {
        public override string OriginalCommand => "wear";
        public override string Description => Main.Instance.Config.Translation.Commands["wear"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.IsSCP)
            {
                response = Main.Instance.Config.Translation.ScpCantwear;
                return false;
            }

            Ragdoll nearestRagdoll = FindNearestRagdoll(player, out string failReason);
            if (nearestRagdoll == null)
            {
                response = failReason;
                return false;
            }

            if (WearDeadPlayer(player, nearestRagdoll))
            {
                response = Main.Instance.Config.Translation.Wore;
                return true;
            }

            response = Main.Instance.Config.Translation.WoreFailure;
            return false;
        }

        private Ragdoll FindNearestRagdoll(Player player, out string reason)
        {
            reason = Main.Instance.Config.Translation.NoDeadBodyFound;
            Ragdoll foundRagdoll = null;
            float closestDistance = float.MaxValue;

            foreach (var ragdoll in Ragdoll.List)
            {
                float distance = Vector3.Distance(player.Position, ragdoll.Position);

                if (distance < 3f && (distance < 1.5f || IsPlayerLookingAt(player, ragdoll.Position)))
                {
                    if (ragdoll.Role.IsScp())
                    {
                        reason = Main.Instance.Config.Translation.ScpClothesNotAllowed;
                        continue;
                    }

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        foundRagdoll = ragdoll;
                    }
                }
            }
            return foundRagdoll;
        }

        private bool IsPlayerLookingAt(Player player, Vector3 targetPosition)
        {
            Vector3 directionToTarget = (targetPosition - player.Camera.position).normalized;
            return Vector3.Angle(player.Camera.forward, directionToTarget) <= 45f;
        }

        private bool WearDeadPlayer(Player player, Ragdoll ragdollToWear)
        {
            try
            {
                var ragdollData = ragdollToWear.Base.NetworkInfo;
                if (ragdollData.OwnerHub == null) return false;

                var originalPosition = player.Position;
                var originalRole = player.Role;
                var originalNickname = player.Nickname;
                var disguiseRole = ragdollData.RoleType;

                UnwearCommand.SaveOriginalPlayerData(player, originalRole, originalNickname, disguiseRole);

                string newNickname = string.Format(
                    Main.Instance.Config.WearNicknameFormat,
                    ragdollData.OwnerHub.nicknameSync.MyNick,
                    originalNickname
                    );

                switch (Main.Instance.Config.WearMode)
                {
                    case Enum.WearMode.RoleChange:
                        player.SetRole(ragdollData.RoleType);
                        Timing.CallDelayed(0.2f, () =>
                        {
                            player.Position = originalPosition;
                            player.DisplayName = newNickname;
                        });
                        break;

                    case Enum.WearMode.ModelChange:
                        player.AddFakeRole(ragdollData.RoleType);
                        player.DisplayName = newNickname;
                        break;
                }

                ragdollToWear.Destroy();

                float disguiseDuration = Main.Instance.Config.WearDuration;
                if (disguiseDuration >= 0f)
                {
                    Timing.CallDelayed(disguiseDuration, () =>
                    {
                        if (player == null || player.IsDestroyed)
                        {
                            UnwearCommand.ClearPlayerData(player);
                            return;
                        }

                        var revertPosition = player.Position;

                        switch (Main.Instance.Config.WearMode)
                        {
                            case Enum.WearMode.RoleChange:
                                if (player.Role == ragdollData.RoleType)
                                {
                                    player.SetRole(originalRole);
                                    Timing.CallDelayed(0.2f, () =>
                                    {
                                        player.Position = revertPosition;
                                        player.DisplayName = originalNickname;
                                    });
                                }
                                break;

                            case Enum.WearMode.ModelChange:
                                player.AddFakeRole(originalRole);
                                player.DisplayName = originalNickname;
                                break;
                        }

                        UnwearCommand.ClearPlayerData(player);
                        player.SendBroadcast(Main.Instance.Config.Translation.DisguiseWornOff, 7);
                    });
                }
                return true;
            }
            catch (System.Exception)
            {
                UnwearCommand.ClearPlayerData(player);
                return false;
            }
        }
    }
}