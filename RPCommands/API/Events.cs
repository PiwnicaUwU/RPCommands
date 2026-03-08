using RPCommands.API.PlayerEvents;
using System;

namespace RPCommands.API
{
    public static class Events
    {
        public static event EventHandler<PlayerSendingRpCommandEventArgs> SendingRpCommand;

        public static event EventHandler<PlayerReceivingRpCommandEventArgs> ReceivingRpCommand;

        internal static void OnSendingRpCommand(PlayerSendingRpCommandEventArgs ev)
        {
            SendingRpCommand?.Invoke(null, ev);
        }

        internal static void OnReceivingRpCommand(PlayerReceivingRpCommandEventArgs ev)
        {
            ReceivingRpCommand?.Invoke(null, ev);
        }
    }
}
