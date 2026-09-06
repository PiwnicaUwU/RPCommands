using LabApi.Events;
using RPCommands.API.PlayerEvents;
using System;

namespace RPCommands.API;

public static class Events
{
    public static event LabEventHandler<PlayerSendingRpCommandEventArgs> SendingRpCommand;

    public static event LabEventHandler<PlayerReceivingRpCommandEventArgs> ReceivingRpCommand;

    internal static void OnSendingRpCommand(PlayerSendingRpCommandEventArgs ev)
    {
        SendingRpCommand?.Invoke(ev);
    }

    internal static void OnReceivingRpCommand(PlayerReceivingRpCommandEventArgs ev)
    {
        ReceivingRpCommand?.Invoke(ev);
    }
}
