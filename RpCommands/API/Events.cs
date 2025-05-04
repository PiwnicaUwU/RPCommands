using System;
using Exiled.API.Features;

namespace RPCommands.API
{
    public static class Events
    {
        /// <summary>
        /// Invoked when a roleplay message is set (.me, .desc, etc.)
        /// </summary>
        public static event Action<Player, string, string> MessageSet;

        /// <summary>
        /// Internal method to invoke MessageSet safely.
        /// </summary>
        internal static void InvokeMessageSet(Player player, string command, string message)
        {
            MessageSet?.Invoke(player, command, message);
        }
    }
}
