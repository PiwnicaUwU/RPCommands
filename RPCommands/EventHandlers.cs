using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using LabApiExtensions.Managers;
using PlayerRoles;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RPCommands
{
    internal class EventHandlers
    {
        public void LoadEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death += OnPlayerDeath;
            LabApi.Events.Handlers.PlayerEvents.Joined += OnPlayerVerified;
        }

        public void UnloadEvents()
        {
            LabApi.Events.Handlers.PlayerEvents.Death -= OnPlayerDeath;
            LabApi.Events.Handlers.PlayerEvents.Joined -= OnPlayerVerified;
        }

        public void RegisterCommands()
        {
            Logger.Info("Starting RPCommands internal command registration...");

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

                    if (!command.IsCommandEnabled)
                    {
                        Logger.Debug($"Command {command.OriginalCommand} is disabled, skipping.");
                        continue;
                    }

                    clientHandler.RegisterCommand(command);
                    Logger.Debug($"Registered internal command: {command.OriginalCommand}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to register internal command {type.Name}: {ex}");
                }
            }
        }

        public void UnregisterCommands()
        {
            Logger.Info("Unregistering RPCommands internal commands...");
            var clientHandler = QueryProcessor.DotCommandHandler;

            var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(RPCommand)) && !t.IsAbstract);

            foreach (var type in commandTypes)
            {
                try
                {
                    if (Activator.CreateInstance(type) is RPCommand command)
                    {
                        clientHandler.UnregisterCommand(command);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to unregister internal command {type.Name}: {ex}");
                }
            }

            API.CommandRegistry.UnregisterAllExternalCommands();
        }

        public void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            if (e.Player == null || !e.Player.IsDestroyed)
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

        public void OnPlayerVerified(PlayerJoinedEventArgs e)
        {
            foreach (var entry in Commands.UnwearCommand.originalPlayerData)
            {
                Player disguisedPlayer = entry.Key;
                RoleTypeId disguiseRole = entry.Value.DisguiseRole;
                disguisedPlayer.AddFakeRole(disguiseRole); // appearance won't change resync when player joins
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

            public static void OnSpawningRagdoll(PlayerSpawnedRagdollEventArgs ev)
            {
                var info = new RagdollInfo
                {
                    Position = ev.Ragdoll.Position,
                    RoleType = ev.Player.Role,
                    Owner = ev.Player,
                    OwnerNickname = ev.Player.Nickname,
                    CreationTime = Time.time
                };

                uint key = (uint)(ev.Ragdoll.Position.GetHashCode());
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
