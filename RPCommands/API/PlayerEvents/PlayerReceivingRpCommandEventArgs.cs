using LabApi.Features.Wrappers;
using System;

namespace RPCommands.API.PlayerEvents
{
    public class PlayerReceivingRpCommandEventArgs(Player sender, Player receiver, string message) : EventArgs
    {
        public Player Sender { get; } = sender;
        public Player Receiver { get; } = receiver;
        public string Message { get; set; } = message;
        public bool IsAllowed { get; set; } = true;
    }
}
