using RemoteAdmin;
using RPCommands.Enum;
using System;
using System.Collections.Generic;

namespace RPCommands.API
{
    public static class CommandRegistry
    {
        private static readonly List<BaseRPCommand> RegisteredExternalCommands = [];

        /// <summary>
        /// Allows external plugins to register their own custom RPCommands.
        /// </summary>
        /// <param name="command">The instance of the new command.</param>
        /// <param name="handlerType">Where the command should be registered (Client or RemoteAdmin).</param>
        /// <returns>True if the command was successfully registered, false otherwise.</returns>
        public static bool RegisterExternalCommand(BaseRPCommand command, CommandHandlerType handlerType)
        {
            if (command == null)
            {
                Logger.Error("External API Error: Failed to register command because the provided instance is null.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Command))
            {
                Logger.Error("External API Error: Command name is empty or null. Check the 'Command' property in your class.");
                return false;
            }

            try
            {
                switch (handlerType)
                {
                    case CommandHandlerType.Client:
                        QueryProcessor.DotCommandHandler.RegisterCommand(command);
                        Logger.Info($"External API: Registered client command '.{command.Command}'.");
                        break;
                    case CommandHandlerType.RemoteAdmin:
                        CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
                        Logger.Info($"External API: Registered RA command '{command.Command}'.");
                        break;
                    default:
                        Logger.Warn($"External API: Unknown CommandHandlerType for command '{command.Command}'.");
                        return false;
                }

                RegisteredExternalCommands.Add(command);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"External API Error: Exception while registering '{command.Command}': {ex.Message}\n{ex.StackTrace}");
                return false;
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