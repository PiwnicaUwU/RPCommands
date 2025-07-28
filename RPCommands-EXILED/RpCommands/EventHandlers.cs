using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RemoteAdmin;
using RpCommands.Enum;
using RPCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RpCommands
{
    internal class EventHandlers
    {
        public void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Dying += OnPlayerDeath;
        }

        public void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDeath;
        }

        public void RegisterCommands()
        {
            Log.Debug("Starting RPCommands command registration...");

            var clientHandler = QueryProcessor.DotCommandHandler;
            var raHandler = CommandProcessor.RemoteAdminCommandHandler;
            var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(RPCommand)) && !t.IsAbstract);

            foreach (var type in commandTypes)
            {
                try
                {
                    if (Activator.CreateInstance(type) is not RPCommand command)
                        continue;

                    if (!Main.Instance.Config.IsCommandEnabled(command.OriginalCommand))
                    {
                        Log.Debug($"Command '{command.OriginalCommand}' is disabled in the config. Skipping.");
                        continue;
                    }

                    var handlerType = Main.Instance.Config.GetSettings(command.OriginalCommand).Handler;

                    switch (handlerType)
                    {
                        case CommandHandlerType.Client:
                            clientHandler.RegisterCommand(command);
                            Log.Debug($"Registered command '{command.Command}' to the ClientCommandHandler.");
                            break;
                        case CommandHandlerType.RemoteAdmin:
                            raHandler.RegisterCommand(command);
                            Log.Debug($"Registered command '{command.Command}' to the RemoteAdminCommandHandler.");
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    Log.Warn($"No config settings found for command type '{type.Name}'. The command will not be registered. Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to register command type '{type.Name}': {ex}");
                }
            }
            Log.Info("RPCommands command registration finished.");
        }

        public void OnPlayerDeath(DyingEventArgs e)
        {
            if (e.Player == null || !e.Player.IsConnected)
            {
                return;
            }
            else
            {
                if (e.Player.CustomInfo != null)
                {
                    e.Player.CustomInfo = null;
                }
            }
        }
        public class RagdollInfo
        {
            public Vector3 Position { get; set; }
            public RoleTypeId RoleType { get; set; }
            public Player Owner { get; set; }
            public string OwnerNickname { get; set; }
            public float CreationTime { get; set; }
        }

        public class RagdollTracker
        {
            private static readonly Dictionary<uint, RagdollInfo> ragdollInfos = [];

            public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
            {
                var info = new RagdollInfo
                {
                    Position = ev.Position,
                    RoleType = ev.Player.Role.Type,
                    Owner = ev.Player,
                    OwnerNickname = ev.Player.Nickname,
                    CreationTime = Time.time
                };

                uint key = (uint)(ev.Position.GetHashCode());
                ragdollInfos[key] = info;
            }

            public static RagdollInfo GetRagdollInfo(Vector3 position)
            {
                uint key = (uint)(position.GetHashCode());
                return ragdollInfos.ContainsKey(key) ? ragdollInfos[key] : null;
            }
        }
    }
}
