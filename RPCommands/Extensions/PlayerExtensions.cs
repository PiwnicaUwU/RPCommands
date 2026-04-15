using LabApi.Features.Wrappers;
using UnityEngine;

namespace RPCommands.Extensions
{
    public static class PlayerExtensions
    {
        public static Player GetRaycastPlayer(this Player player, float maxDistance)
        {
            if (!Physics.Raycast(player.Camera.position, player.Camera.forward, out var raycastHit,
                maxDistance, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
            {
                return null;
            }

            return Player.Get(raycastHit.collider.gameObject);
        }

        public static bool SendStaffMessage(this Player player, string message, EncryptedChannelManager.EncryptedChannel channel = EncryptedChannelManager.EncryptedChannel.AdminChat)
        {
            return player.ReferenceHub.encryptedChannelManager.TrySendMessageToClient(player.NetworkId + "!" + message, channel);
        }
    }
}
