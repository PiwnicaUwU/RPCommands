using CommandSystem;
using Exiled.API.Features;
using Mirror;
using PlayerRoles;
using RPCommands;
using System.Linq;
using UnityEngine;

namespace RpCommands.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Clean : RPCommand
    {
        public override string OriginalCommand => "clean";
        public override string Description => Main.Instance.Translation.Commands["clean"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.Team == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Translation.OnlyHumans;
                return false;
            }

            Ragdoll nearbyRagdoll = FindNearestRagdoll(player, 5f);
            if (nearbyRagdoll == null)
            {
                response = Main.Instance.Translation.NoRagdollNearby;
                return false;
            }

            NetworkServer.Destroy(nearbyRagdoll.GameObject);
            response = Main.Instance.Translation.CleanSuccess;
            return true;
        }

        private Ragdoll FindNearestRagdoll(Player player, float range)
        {
            return Ragdoll.List
                .Where(ragdoll => Vector3.Distance(player.Position, ragdoll.Position) < range)
                .OrderBy(ragdoll => Vector3.Distance(player.Position, ragdoll.Position))
                .FirstOrDefault();
        }
    }
}
