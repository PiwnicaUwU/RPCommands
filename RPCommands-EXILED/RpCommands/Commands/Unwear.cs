using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using RPCommands;
using System.Collections.Generic;

namespace RpCommands.Commands
{
    public class UnwearCommand : RPCommand
    {
        private static readonly Dictionary<Player, OriginalPlayerData> originalPlayerData = [];

        public override string OriginalCommand => "unwear";
        public override string Description => Main.Instance.Translation.Commands["unwear"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.IsScp)
            {
                response = Main.Instance.Translation.ScpCantUnwear;
                return false;
            }

            if (!IsPlayerDisguised(player))
            {
                response = Main.Instance.Translation.NotDisguised;
                return false;
            }

            if (RemoveDisguise(player))
            {
                response = Main.Instance.Translation.Unwore;
                return true;
            }
            else
            {
                response = Main.Instance.Translation.UnworeFailure;
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
                            if (player == null || !player.IsConnected)
                                return;

                            player.Role.Set(originalData.OriginalRole, RoleSpawnFlags.None);

                            Timing.CallDelayed(0.1f, () =>
                            {
                                if (player == null || !player.IsConnected)
                                    return;

                                player.Teleport(currentPosition);
                                player.DisplayNickname = originalData.OriginalNickname;
                            });
                        });
                        break;

                    case Enum.WearMode.ModelChange:
                        Timing.CallDelayed(0.1f, () =>
                        {
                            if (player == null || !player.IsConnected)
                                return;

                            player.ChangeAppearance(originalData.OriginalRole, true);
                            player.DisplayNickname = originalData.OriginalNickname;
                        });
                        break;

                    default:
                        Log.Warn($"Invalid WearMode {Main.Instance.Config.WearMode} in config. Please use rolechange or modelchange.");
                        player.SendConsoleMessage("An error occurred while trying to unwear the disguise. Contact server staff.", "red");
                        return false;
                }

                originalPlayerData.Remove(player);

                player.ShowHint(Main.Instance.Translation.DisguiseRemoved, 5f);

                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error($"Error in UnwearCommand.RemoveDisguise: {ex}");
                return false;
            }
        }


        public static void SaveOriginalPlayerData(Player player, RoleTypeId originalRole, string originalNickname)
        {
            originalPlayerData[player] = new OriginalPlayerData
            {
                OriginalRole = originalRole,
                OriginalNickname = originalNickname
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
    }
}