using Exiled.API.Features;
using UnityEngine;

namespace RpCommands.Extensions
{
    public static class PlayerExtensions
    {
        public static Player GetRaycastPlayer(this Player player, float maxDistance)
        {
            if (!Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out var raycastHit,
                maxDistance, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
            {
                return null;
            }

            return Player.Get(raycastHit.collider.gameObject);
        }
    }
}
