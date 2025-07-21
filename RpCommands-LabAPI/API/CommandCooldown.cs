using LabApi.Features.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace RPCommands.API
{
    public static class CommandCooldown // very clear API
    {
        private static readonly Dictionary<string, Dictionary<string, float>> _cooldowns = new();

        /// <summary>
        /// Sets a cooldown for a player on a specific command.
        /// </summary>
        /// <param name="player">The player to set the cooldown for.</param>
        /// <param name="command">The command to set the cooldown on.</param>
        /// <param name="duration">The duration of the cooldown in seconds.</param>
        public static void SetCooldown(Player player, string command, float duration)
        {
            string userId = player.UserId;
            if (!_cooldowns.ContainsKey(userId))
                _cooldowns[userId] = new Dictionary<string, float>();

            _cooldowns[userId][command] = Time.realtimeSinceStartup + duration;
        }

        /// <summary>
        /// Checks if a player currently has an active cooldown on a specific command.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="command">The command to check for cooldown.</param>
        /// <returns>True if cooldown is active, false otherwise.</returns>
        public static bool HasCooldown(Player player, string command)
        {
            if (!_cooldowns.TryGetValue(player.UserId, out var commands))
                return false;

            return commands.TryGetValue(command, out var endTime) && endTime > Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Gets the remaining cooldown time for a player's command.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="command">The command to check for cooldown.</param>
        /// <returns>The remaining cooldown time in seconds. If no cooldown, returns 0.</returns>
        public static float GetRemainingCooldown(Player player, string command)
        {
            if (!_cooldowns.TryGetValue(player.UserId, out var commands))
                return 0f;

            if (!commands.TryGetValue(command, out var endTime))
                return 0f;

            float remaining = endTime - Time.realtimeSinceStartup;
            return remaining > 0 ? remaining : 0f;
        }

        /// <summary>
        /// Clears the cooldown for a player's specific command.
        /// </summary>
        /// <param name="player">The player to clear the cooldown for.</param>
        /// <param name="command">The command to clear the cooldown for.</param>
        public static void ClearCooldown(Player player, string command)
        {
            if (_cooldowns.TryGetValue(player.UserId, out var commands))
                commands.Remove(command);
        }

        /// <summary>
        /// Clears all cooldowns for a player.
        /// </summary>
        /// <param name="player">The player whose cooldowns should be cleared.</param>
        public static void ClearAllCooldowns(Player player)
        {
            _cooldowns.Remove(player.UserId);
        }

        /// <summary>
        /// Clears all cooldowns for all players.
        /// </summary>
        public static void ClearAllCooldowns()
        {
            _cooldowns.Clear();
        }
    }
}
