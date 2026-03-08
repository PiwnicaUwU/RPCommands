using LabApi.Features.Wrappers;
using System;

namespace RPCommands.API.PlayerEvents
{
    public class PlayerSendingRpCommandEventArgs(Player player, string command, string message) : EventArgs
    {
        public Player Player { get; } = player;
        public string Command { get; set; } = command;
        public string Message { get; set; } = message;
        public bool IsAllowed { get; set; } = true;
    }
}
