using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace RPCommands.API;

public static class Request
{
    private static readonly Dictionary<Player, Dictionary<string, string>> _lastMessages = [];

    /// <summary>
    /// Saves the last message sent by a player for a specific command.
    /// </summary>
    /// <param name="player">The player who sent the message.</param>
    /// <param name="command">The command used.</param>
    /// <param name="message">The message content.</param>
    public static void SetLastMessage(this Player player, BaseRPCommand command, string message)
    {
        if (!_lastMessages.ContainsKey(player))
            _lastMessages[player] = [];

        _lastMessages[player][command.Command] = message;
    }

    /// <summary>
    /// Retrieves the last message sent by a player for a specific command.
    /// </summary>
    /// <param name="player">The player to get the message from.</param>
    /// <param name="command">The command to check.</param>
    /// <returns>The last message if available, otherwise null.</returns>
    public static string GetLastMessage(this Player player, BaseRPCommand command)
    {
        return _lastMessages.TryGetValue(player, out var messages) && messages.TryGetValue(command.Command, out var msg)
            ? msg
            : null;
    }

    /// <summary>
    /// Retrieves all last messages sent by a player across all RPCommands.
    /// </summary>
    /// <param name="player">The player to get messages from.</param>
    /// <returns>A dictionary of command = message.</returns>
    public static Dictionary<string, string> GetAllMessages(Player player)
    {
        return _lastMessages.TryGetValue(player, out var messages)
            ? new Dictionary<string, string>(messages)
            : [];
    }

    /// <summary>
    /// Clears all messages associated with a specific player.
    /// </summary>
    /// <param name="player">The player whose messages should be cleared.</param>
    public static void ClearMessages(Player player)
    {
        _lastMessages.Remove(player);
    }

    /// <summary>
    /// Clears all stored messages.
    /// </summary>
    public static void ClearAllMessages()
    {
        _lastMessages.Clear();
    }
}