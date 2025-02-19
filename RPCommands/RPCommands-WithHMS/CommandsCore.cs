using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;
using MEC;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;

namespace RPCommands.Core
{
    public abstract class NarrativeCommand : ICommand
    {
        public abstract string Command { get; }
        public virtual string[] Aliases => Array.Empty<string>();
        public abstract string Description { get; }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Round.IsStarted)
            {
                response = "You cannot use this command because the round has not started yet.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = $"Usage: .{Command} <message>";
                return false;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = "Only players can use this command.";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            string message = string.Join(" ", arguments);
            string formattedMessage = FormatMessage(player, message);
            response = "Message has been sent.";
            return true;
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return $"[{Command}] {player.Nickname}: {message}";
        }
    }
}
