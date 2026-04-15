using LabApi.Features.Wrappers;
using PlayerRoles;
using System.Linq;
using UnityEngine;

namespace RPCommands.Commands
{

    internal class Clean : InternalRPCommand
    {
        public override string OriginalCommand => "clean";
        public override string Description => Main.Instance.Config.Translation.Commands["clean"];
        public override bool AllowNoArguments => true;

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.Role.GetTeam() == Team.SCPs && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Config.Translation.OnlyHumans;
                return false;
            }

            Ragdoll nearbyRagdoll = FindNearestRagdoll(player, 5f);
            if (nearbyRagdoll == null)
            {
                response = Main.Instance.Config.Translation.NoRagdollNearby;
                return false;
            }

            nearbyRagdoll.Destroy();
            response = Main.Instance.Config.Translation.CleanSuccess;
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
