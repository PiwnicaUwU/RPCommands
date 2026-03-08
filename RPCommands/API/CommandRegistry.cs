using RemoteAdmin;
using RPCommands.Enum;
using System;
using System.Collections.Generic;

namespace RPCommands.API
{
    public static class CommandRegistry
    {
        private static readonly List<RPCommand> RegisteredExternalCommands = [];

        /// <summary>
        /// Allows external plugins to register their own RPCommands.
        /// </summary>
        /// <param name="command">The instance of the new command.</param>
        /// <param name="handlerType">Where the command should be registered.</param>
        public static void RegisterExternalCommand(RPCommand command, CommandHandlerType handlerType)
        {
            if (command == null)
            {
                Logger.Error("External command cannot be null!");
                return;
            }

            try
            {
                switch (handlerType)
                {
                    case CommandHandlerType.Client:
                        QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        Logger.Info($"External API: Registered client command '{command.OriginalCommand}'.");
                        break;
                    case CommandHandlerType.RemoteAdmin:
                        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                        Logger.Info($"External API: Registered RA command '{command.OriginalCommand}'.");
                        break;
                }

                RegisteredExternalCommands.Add(command);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while registering external command '{command.OriginalCommand}': {ex}");
            }
        }

        /// <summary>
        /// Unregisters all external commands.
        /// </summary>
        public static void UnregisterAllExternalCommands()
        {
            foreach (var cmd in RegisteredExternalCommands)
            {
                QueryProcessor.DotCommandHandler.UnregisterCommand(cmd);
                CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(cmd);
            }
            RegisteredExternalCommands.Clear();
            Logger.Info("External API: Unregistered all external commands.");
        }
    }
}