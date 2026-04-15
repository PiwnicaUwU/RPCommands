using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using RemoteAdmin;
using RPCommands.API;
using RPCommands.API.PlayerEvents;
using RPCommands.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextToy = RPCommands.Components.TextToy;

namespace RPCommands.Handlers
{
    public static class DisplayHandler
    {
        public static void DisplayMessage(Player sender, string message, float range, float duration, RPCommandsMode displayMode)
        {
            switch (displayMode)
            {
                case RPCommandsMode.None:
                    break;

                case RPCommandsMode.Hints:
                    HintToNearbyPlayers(sender, message, range, duration);
                    break;

                case RPCommandsMode.TextToys:
                    SpawnTextToyForSender(sender, message, range, duration, true);

                    if (Main.Instance.Config.ShowHintsToSpectatorsOfReceivers)
                        SendHintToSpectatorsOfNearbyPlayers(sender, message, range, duration);
                    break;

                case RPCommandsMode.Both:
                    HintToNearbyPlayers(sender, message, range, duration);
                    SpawnTextToyForSender(sender, message, range, duration, false);
                    break;
            }
        }

        private static void SpawnTextToyForSender(Player sender, string message, float range, float duration, bool showInConsole)
        {
            try
            {
                if (showInConsole && Main.Instance.Config.ShowCommandInSenderConsole)
                {
                    foreach (Player player in Player.List.Where(p => Vector3.Distance(p.Position, sender.Position) <= range))
                    {
                        var receivingArgs = new PlayerReceivingRpCommandEventArgs(sender, player, message);
                        Events.OnReceivingRpCommand(receivingArgs);

                        if (!receivingArgs.IsAllowed) continue;

                        player.SendConsoleMessage($"{message}", "yellow");
                    }
                }

                AdminToys.TextToy prefab = NetworkClient.prefabs.Values
                    .Select(p => p.GetComponent<AdminToys.TextToy>())
                    .FirstOrDefault(t => t != null);

                if (prefab == null)
                {
                    Logger.Error("TextToy prefab is null. Cannot spawn TextToy.");
                    return;
                }

                AdminToys.TextToy textToy = UnityEngine.Object.Instantiate(prefab);

                textToy.transform.position = sender.Position + (Vector3.up * Main.Instance.Config.TextToyHeightOffset);
                textToy.transform.rotation = sender.GameObject.transform.rotation;
                textToy.transform.localScale = Vector3.one;

                NetworkServer.Spawn(textToy.gameObject);

                textToy.TextFormat = $"<size={Main.Instance.Config.TextToySize}>{message}</size>";

                TextToy controller = textToy.gameObject.AddComponent<TextToy>();
                controller.Initialize(sender, textToy, Main.Instance.Config.TextToyHeightOffset);
                sender.Connection.Send(new ObjectDestroyMessage { netId = textToy.netId });

                Timing.CallDelayed(duration, () =>
                {
                    controller?.DestroyToy();
                });
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to spawn TextToy: {e}");
            }
        }

        private static void HintToNearbyPlayers(Player sender, string message, float range, float duration)
        {
            bool showInConsole = Main.Instance.Config.ShowCommandInSenderConsole;
            foreach (Player player in Player.List.Where(p => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
            {
                var receivingArgs = new PlayerReceivingRpCommandEventArgs(sender, player, message);
                Events.OnReceivingRpCommand(receivingArgs);

                if (!receivingArgs.IsAllowed)
                    continue;

                string targetSpecificMessage = receivingArgs.Message;

                SendHint(player, targetSpecificMessage, duration);
                if (showInConsole)
                {
                    player.SendConsoleMessage($"{targetSpecificMessage}", "yellow");
                }
            }

            SendHint(sender, message, duration);

            if (showInConsole)
            {
                sender.SendConsoleMessage($"{message}", "yellow");
            }
        }

        private static void SendHintToSpectatorsOfNearbyPlayers(Player sender, string message, float range, float duration)
        {
            HashSet<Player> spectatorsToSend = [];

            foreach (Player playerInRange in Player.List.Where(p => Vector3.Distance(p.Position, sender.Position) <= range))
            {
                foreach (Player spectator in playerInRange.CurrentSpectators)
                {
                    if (!spectator.IsDestroyed)
                    {
                        spectatorsToSend.Add(spectator);
                    }
                }
            }

            foreach (Player spectator in spectatorsToSend)
            {
                DynamicHint hint = new()
                {
                    Text = message,
                    TargetY = 800,
                    TargetX = -950,
                    FontSize = 25,
                };

                PlayerDisplay observerDisplay = PlayerDisplay.Get(spectator);
                observerDisplay?.AddHint(hint);
                Timing.CallDelayed(duration, () => observerDisplay?.RemoveHint(hint));
            }
        }

        public static void SendHint(Player player, string message, float duration)
        {
            DynamicHint hint = new()
            {
                Text = message,
                TargetY = 760,
                TargetX = -950,
                FontSize = 25,
            };

            PlayerDisplay playerDisplay = PlayerDisplay.Get(player);
            playerDisplay?.AddHint(hint);

            Timing.CallDelayed(duration, () => playerDisplay?.RemoveHint(hint));

            if (Main.Instance.Config.ShowHintsToSpectatorsOfReceivers)
            {
                foreach (Player observer in player.CurrentSpectators)
                {
                    if (observer.IsDestroyed) continue;

                    DynamicHint spectatorHint = new()
                    {
                        Text = message,
                        TargetY = 760,
                        TargetX = -950,
                        FontSize = 25,
                    };

                    PlayerDisplay observerDisplay = PlayerDisplay.Get(observer);
                    observerDisplay?.AddHint(spectatorHint);
                    Timing.CallDelayed(duration, () => observerDisplay?.RemoveHint(spectatorHint));
                }
            }
        }
    }
}