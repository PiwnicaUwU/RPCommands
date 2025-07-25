﻿using Exiled.API.Features;
using System.Collections.Generic;

namespace RPCommands.API
{
    public static class Request
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _lastMessages = new();

        /// <summary>
        /// Saves the last message sent by a player for a specific command.
        /// </summary>
        /// <param name="player">The player who sent the message.</param>
        /// <param name="command">The command used.</param>
        /// <param name="message">The message content.</param>
        public static void SetLastMessage(Player player, string command, string message)
        {
            string userId = player.UserId;

            if (!_lastMessages.ContainsKey(userId))
                _lastMessages[userId] = new Dictionary<string, string>();

            _lastMessages[userId][command] = message;
            Events.InvokeMessageSet(player, command, message);
        }

        /// <summary>
        /// Retrieves the last message sent by a player for a specific command.
        /// </summary>
        /// <param name="player">The player to get the message from.</param>
        /// <param name="command">The command to check.</param>
        /// <returns>The last message if available, otherwise null.</returns>
        public static string GetLastMessage(Player player, string command)
        {
            return _lastMessages.TryGetValue(player.UserId, out var messages) && messages.TryGetValue(command, out var msg)
                ? msg
                : null;
        }

        /// <summary>
        /// Retrieves all last messages sent by a player across all RP Commands.
        /// </summary>
        /// <param name="player">The player to get messages from.</param>
        /// <returns>A dictionary of command = message.</returns>
        public static Dictionary<string, string> GetAllMessages(Player player)
        {
            return _lastMessages.TryGetValue(player.UserId, out var messages)
                ? new Dictionary<string, string>(messages)
                : new Dictionary<string, string>();
        }

        /// <summary>
        /// Clears all messages associated with a specific player.
        /// </summary>
        /// <param name="player">The player whose messages should be cleared.</param>
        public static void ClearMessages(Player player)
        {
            _lastMessages.Remove(player.UserId);
        }

        /// <summary>
        /// Clears all stored messages.
        /// </summary>
        public static void ClearAllMessages()
        {
            _lastMessages.Clear();
        }
    }
}