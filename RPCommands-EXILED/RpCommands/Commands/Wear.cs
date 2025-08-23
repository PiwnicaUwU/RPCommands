using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using RPCommands;
using System.Linq;
using UnityEngine;

namespace RpCommands.Commands
{
    public class WearCommand : RPCommand
    {
        public override string OriginalCommand => "wear";
        public override string Description => Main.Instance.Translation.Commands["wear"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.IsScp)
            {
                response = Main.Instance.Translation.ScpCantwear;
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
                response = Main.Instance.Translation.Wore;
                return true;
            }

            response = Main.Instance.Translation.WoreFailure;
            return false;
        }

        private Ragdoll FindNearestRagdoll(Player player, out string reason)
        {
            reason = Main.Instance.Translation.NoDeadBodyFound;
            Ragdoll foundRagdoll = null;
            float closestDistance = float.MaxValue;

            foreach (var ragdoll in Ragdoll.List)
            {
                float distance = Vector3.Distance(player.Position, ragdoll.Position);

                if (distance < 3f && (distance < 1.5f || IsPlayerLookingAt(player, ragdoll.Position)))
                {
                    if (ragdoll.Role.IsScp())
                    {
                        reason = Main.Instance.Translation.ScpClothesNotAllowed;
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
            Vector3 directionToTarget = (targetPosition - player.CameraTransform.position).normalized;
            return Vector3.Angle(player.CameraTransform.forward, directionToTarget) <= 45f;
        }

        private bool WearDeadPlayer(Player player, Ragdoll ragdollToWear)
        {
            try
            {
                var ragdollData = ragdollToWear.NetworkInfo;
                if (ragdollData.OwnerHub == null) return false;

                var originalPosition = player.Position;
                var originalRole = player.Role.Type;
                var originalNickname = player.Nickname;

                UnwearCommand.SaveOriginalPlayerData(player, originalRole, originalNickname);

                switch (Main.Instance.Config.WearMode)
                {
                    case Enum.WearMode.RoleChange:
                        player.Role.Set(ragdollData.RoleType, SpawnReason.ForceClass, RoleSpawnFlags.None);
                        Timing.CallDelayed(0.2f, () =>
                        {
                            player.Teleport(originalPosition);
                            player.DisplayNickname = ragdollData.OwnerHub.nicknameSync.MyNick;
                        });
                        break;

                    case Enum.WearMode.ModelChange:
                        player.ChangeAppearance(ragdollData.RoleType, true);
                        player.DisplayNickname = ragdollData.OwnerHub.nicknameSync.MyNick;
                        break;
                }

                ragdollToWear.Destroy();

                float disguiseDuration = Main.Instance.Config.WearDuration;
                if (disguiseDuration >= 0f)
                {
                    Timing.CallDelayed(disguiseDuration, () =>
                    {
                        if (player == null || !player.IsConnected)
                        {
                            UnwearCommand.ClearPlayerData(player);
                            return;
                        }

                        var revertPosition = player.Position;

                        switch (Main.Instance.Config.WearMode)
                        {
                            case Enum.WearMode.RoleChange:
                                if (player.Role.Type == ragdollData.RoleType)
                                {
                                    player.Role.Set(originalRole, RoleSpawnFlags.None);
                                    Timing.CallDelayed(0.2f, () =>
                                    {
                                        player.Teleport(revertPosition);
                                        player.DisplayNickname = originalNickname;
                                    });
                                }
                                break;

                            case Enum.WearMode.ModelChange:
                                player.ChangeAppearance(originalRole, true);
                                player.DisplayNickname = originalNickname;
                                break;
                        }

                        UnwearCommand.ClearPlayerData(player);
                        player.ShowHint(Main.Instance.Translation.DisguiseWornOff, 7f);
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