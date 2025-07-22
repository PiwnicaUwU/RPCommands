using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace RpCommands
{
    internal class EventHandlers
    {
        public void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath;
        }

        public void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDeath;
        }

        public void OnPlayerDeath(DyingEventArgs e)
        {
            if (e.Player == null || !e.Player.IsConnected)
            {
                return;
            }
            else
            {
                if (e.Player.CustomInfo != null)
                {
                    e.Player.CustomInfo = null;
                }
            }
        }
        public class RagdollInfo
        {
            public Vector3 Position { get; set; }
            public RoleTypeId RoleType { get; set; }
            public Player Owner { get; set; }
            public string OwnerNickname { get; set; }
            public float CreationTime { get; set; }
        }

        public class RagdollTracker
        {
            private static readonly Dictionary<uint, RagdollInfo> ragdollInfos = [];

            public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
            {
                var info = new RagdollInfo
                {
                    Position = ev.Position,
                    RoleType = ev.Player.Role.Type,
                    Owner = ev.Player,
                    OwnerNickname = ev.Player.Nickname,
                    CreationTime = Time.time
                };

                uint key = (uint)(ev.Position.GetHashCode());
                ragdollInfos[key] = info;
            }

            public static RagdollInfo GetRagdollInfo(Vector3 position)
            {
                uint key = (uint)(position.GetHashCode());
                return ragdollInfos.ContainsKey(key) ? ragdollInfos[key] : null;
            }
        }
    }
}
