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
            var nearestRagdoll = FindNearestRagdoll(player);

            if (player.IsScp)
            {
                response = Main.Instance.Translation.ScpCantwear;
                return false;
            }

            if (!nearestRagdoll.HasValue)
            {
                response = Main.Instance.Translation.NoDeadBodyFound;
                return false;
            }

            if (WearDeadPlayer(player, nearestRagdoll.Value))
            {
                response = Main.Instance.Translation.Wore;
                return true;
            }
            else
            {
                response = Main.Instance.Translation.WoreFailure;
                return false;
            }
        }

        private RagdollData? FindNearestRagdoll(Player player)
        {
            Vector3 playerPosition = player.Position;
            RagdollData? nearestRagdoll = null;
            float nearestDistance = float.MaxValue;
            const float maxDistance = 3f;

            foreach (var ragdoll in Ragdoll.List)
            {
                float distance = Vector3.Distance(playerPosition, ragdoll.Position);
                if (distance <= maxDistance)
                {
                    if (IsPlayerLookingAt(player, ragdoll.Position) || distance <= 1.5f)
                    {
                        if (ragdoll.Role.IsScp())
                        {
                            player.SendConsoleMessage(Main.Instance.Translation.ScpClothesNotAllowed, "red");
                            break;
                        }

                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestRagdoll = ragdoll.NetworkInfo;
                        }
                    }
                }
            }
            return nearestRagdoll;
        }

        private bool IsPlayerLookingAt(Player player, Vector3 targetPosition)
        {
            Vector3 playerForward = player.CameraTransform.forward;
            Vector3 directionToTarget = (targetPosition - player.CameraTransform.position).normalized;
            float angle = Vector3.Angle(playerForward, directionToTarget);
            return angle <= 45f;
        }

        private bool WearDeadPlayer(Player player, RagdollData ragdollData)
        {
            try
            {
                if (ragdollData.OwnerHub != null)
                {
                    var ragdollToRemove = Ragdoll.List.FirstOrDefault(r => r.NetworkInfo.Equals(ragdollData));
                    var currentPosition = player.Position;
                    var originalRole = player.Role.Type;
                    var originalNickname = player.Nickname;

                    UnwearCommand.SaveOriginalPlayerData(player, originalRole, originalNickname);

                    switch (Main.Instance.Config.WearMode)
                    {
                        case Enum.WearMode.RoleChange:
                            Timing.CallDelayed(0.1f, () =>
                            {
                                player.Role.Set(ragdollData.RoleType, SpawnReason.ForceClass, RoleSpawnFlags.None);

                                Timing.CallDelayed(0.1f, () =>
                                {
                                    player.Teleport(currentPosition);
                                    player.DisplayNickname = ragdollData.OwnerHub.nicknameSync.MyNick;
                                });

                                ragdollToRemove?.Destroy();
                            });
                            break;

                        case Enum.WearMode.ModelChange:
                            Timing.CallDelayed(0.1f, () =>
                            {
                                player.ChangeAppearance(ragdollData.RoleType, true);
                                player.DisplayNickname = ragdollData.OwnerHub.nicknameSync.MyNick;
                                ragdollToRemove?.Destroy();
                            });
                            break;

                        default:
                            Log.Warn($"Invalid WearMode {Main.Instance.Config.WearMode} in config. Please use rolechange or modelchange.");
                            player.SendConsoleMessage("An error occurred while trying to wear the dead player. Contact server staff.", "red");
                            return false;
                    }

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
                                    Timing.CallDelayed(0.1f, () =>
                                    {
                                        if (player.Role.Type == ragdollData.RoleType)
                                        {
                                            player.Role.Set(originalRole, RoleSpawnFlags.None);

                                            Timing.CallDelayed(0.1f, () =>
                                            {
                                                player.Teleport(revertPosition);
                                                player.DisplayNickname = originalNickname;
                                            });
                                        }
                                    });
                                    break;

                                case Enum.WearMode.ModelChange:
                                    Timing.CallDelayed(0.1f, () =>
                                    {
                                        player.ChangeAppearance(originalRole, true);
                                        player.DisplayNickname = originalNickname;
                                    });
                                    break;
                            }

                            UnwearCommand.ClearPlayerData(player);
                            player.ShowHint(Main.Instance.Translation.DisguiseWornOff, 7f);
                        });
                    }
                    return true;
                }
                return false;
            }
            catch (System.Exception)
            {
                UnwearCommand.ClearPlayerData(player);
                return false;
            }
        }
    }
}