using LabApi.Features.Wrappers;
using LabApiExtensions.Managers;
using MEC;
using PlayerRoles;
using System.Collections.Generic;

namespace RPCommands.Commands
{
    public class UnwearCommand : RPCommand
    {
        public static readonly Dictionary<Player, OriginalPlayerData> originalPlayerData = [];

        public override string OriginalCommand => "unwear";
        public override string Description => Main.Instance.Config.Translation.Commands["unwear"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.IsSCP)
            {
                response = Main.Instance.Config.Translation.ScpCantUnwear;
                return false;
            }

            if (!IsPlayerDisguised(player))
            {
                response = Main.Instance.Config.Translation.NotDisguised;
                return false;
            }

            if (RemoveDisguise(player))
            {
                response = Main.Instance.Config.Translation.Unwore;
                return true;
            }
            else
            {
                response = Main.Instance.Config.Translation.UnworeFailure;
                return false;
            }
        }

        private bool IsPlayerDisguised(Player player)
        {
            return originalPlayerData.ContainsKey(player);
        }

        private bool RemoveDisguise(Player player)
        {
            try
            {
                if (!originalPlayerData.TryGetValue(player, out OriginalPlayerData originalData))
                {
                    return false;
                }

                var currentPosition = player.Position;

                switch (Main.Instance.Config.WearMode)
                {
                    case Enum.WearMode.RoleChange:
                        Timing.CallDelayed(0.1f, () =>
                        {
                            if (player == null || player.IsDestroyed)
                                return;

                            player.SetRole(originalData.OriginalRole);

                            Timing.CallDelayed(0.1f, () =>
                            {
                                if (player == null || player.IsDestroyed)
                                    return;

                                player.Position = currentPosition;
                                player.DisplayName = originalData.OriginalNickname;
                            });
                        });
                        break;

                    case Enum.WearMode.ModelChange:
                        Timing.CallDelayed(0.1f, () =>
                        {
                            if (player == null || player.IsDestroyed)
                                return;

                            player.AddFakeRole(originalData.OriginalRole);
                            player.DisplayName = originalData.OriginalNickname;
                        });
                        break;

                    default:
                        Logger.Warn($"Invalid WearMode {Main.Instance.Config.WearMode} in config. Please use rolechange or modelchange.");
                        player.SendConsoleMessage("An error occurred while trying to unwear the disguise. Contact server staff.", "red");
                        return false;
                }

                originalPlayerData.Remove(player);

                player.SendBroadcast(Main.Instance.Config.Translation.DisguiseRemoved, 5);

                return true;
            }
            catch (System.Exception ex)
            {
                Logger.Error($"Error in UnwearCommand.RemoveDisguise: {ex}");
                return false;
            }
        }


        public static void SaveOriginalPlayerData(Player player, RoleTypeId originalRole, string originalNickname, RoleTypeId disguiseRole)
        {
            originalPlayerData[player] = new OriginalPlayerData
            {
                OriginalRole = originalRole,
                OriginalNickname = originalNickname,
                DisguiseRole = disguiseRole
            };
        }


        public static void ClearPlayerData(Player player)
        {
            originalPlayerData.Remove(player);
        }


        public static bool IsPlayerWearing(Player player)
        {
            return originalPlayerData.ContainsKey(player);
        }
    }

    public class OriginalPlayerData
    {
        public RoleTypeId OriginalRole { get; set; }
        public string OriginalNickname { get; set; }
        public RoleTypeId DisguiseRole { get; set; }
    }
}