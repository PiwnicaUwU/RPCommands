using System;
#if EXILED
using Exiled.API.Features;
#else
using LabApi.Features.Wrappers;
#endif

namespace RPCommands.API
{
    public static class Events
    {
#if EXILED
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
#else
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
#endif