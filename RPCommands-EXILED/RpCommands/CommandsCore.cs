using CommandSystem;
using Exiled.API.Features;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using MEC;
using Mirror;
using RemoteAdmin;
using RpCommands.Components;
using RpCommands.Enum;
using RPCommands.API;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPCommands
{
    public abstract class RPCommand : ICommand
    {
        public abstract string OriginalCommand { get; }
        public virtual string[] Aliases
        {
            get
            {
                return [];
            }
        }

        public abstract string Description { get; }
        public virtual bool AllowNoArguments => false;
        public string Command => Main.Instance.Translation.CommandNames.TryGetValue(OriginalCommand, out string translatedName) ? translatedName : OriginalCommand;

        private static readonly Dictionary<Player, Dictionary<string, float>> PlayerCooldowns = new();
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = Main.Instance.Translation.OnlyPlayers;

                return false;
            }
            Player player = Player.Get(playerSender.ReferenceHub);

            if (!Main.Instance.Config.IsCommandEnabled(OriginalCommand))
            {
                response = Main.Instance.Translation.CommandDisabled;
                return false;
            }

            if (!Round.IsStarted && OriginalCommand != "assist") // assist must work even if round is not started
            {
                response = Main.Instance.Translation.RoundNotStarted;
                return false;
            }

            if (player.IsScp && !Main.Instance.Config.AllowScpToUseCommands)
            {
                response = Main.Instance.Translation.OnlyHumans;
                return false;
            }

            if (!player.IsAlive && OriginalCommand != "assist") 
            {
                response = Main.Instance.Translation.OnlyAlive;
                return false;
            }

            if (arguments.Count < 1 && !AllowNoArguments)
            {
                response = string.Format(Main.Instance.Translation.Usage, Command);
                return false;
            }

            if (HasCooldown(player, out float remainingTime))
            {
                if (Main.Instance.Translation.CommandCooldown.Contains("{0}"))
                    response = string.Format(Main.Instance.Translation.CommandCooldown, Math.Ceiling(remainingTime));
                else
                    response = Main.Instance.Translation.CommandCooldown;

                return false;
            }

            string message = string.Join(" ", arguments);

            if (Main.Instance.Config.BannedWords.Any(bannedWord => message.ToLower().Contains(bannedWord.ToLower())))
            {
                response = Main.Instance.Translation.BannedWordDetected;
                return false;
            }

            if (!ExecuteAction(player, message, out response))
                return false;

            SetCooldown(player);
            Request.SetLastMessage(player, OriginalCommand, message);
            return true;
        }

        protected virtual bool ExecuteAction(Player player, string message, out string response)
        {
            float range = Main.Instance.Config.GetRange(OriginalCommand);
            float duration = Main.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            DisplayMessage(player, formattedMessage, range, duration);

            response = Main.Instance.Translation.MessageSent;
            return true;
        }

        private void DisplayMessage(Player sender, string message, float range, float duration)
        {
            switch (Main.Instance.Config.DisplayMode)
            {
                case RPCommandsMode.Hints:
                    HintToNearbyPlayers(sender, message, range, duration);
                    break;

                case RPCommandsMode.TextToys:
                    SpawnTextToyForSender(sender, message, duration, true);

                    if (Main.Instance.Config.ShowHintsToSpectatorsOfReceivers)
                    {
                        SendHintToSpectatorsOfNearbyPlayers(sender, message, range, duration);
                    }
                    break;

                case RPCommandsMode.Both:
                    HintToNearbyPlayers(sender, message, range, duration);
                    SpawnTextToyForSender(sender, message, duration, false);
                    break;
            }
        }

        private void SpawnTextToyForSender(Player sender, string message, float duration, bool showInConsole)
        {
            try
            {
                if (showInConsole && Main.Instance.Config.ShowCommandInSenderConsole)
                {
                    foreach (Player player in Player.List.Where(p => Vector3.Distance(p.Position, sender.Position) <= Main.Instance.Config.GetRange(OriginalCommand)))
                    {
                        player.SendConsoleMessage($"{message}", "yellow");
                    }
                }
                AdminToys.TextToy prefab = Exiled.API.Features.Toys.Text.Prefab;
                if (prefab == null)
                {
                    Log.Error("TextToy prefab is null. Cannot spawn TextToy.");
                    return;
                }

                AdminToys.TextToy textToy = UnityEngine.Object.Instantiate(prefab);

                textToy.transform.position = sender.Position + (Vector3.up * Main.Instance.Config.TextToyHeightOffset);
                textToy.transform.rotation = sender.Transform.rotation;
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
                Log.Error($"Failed to spawn TextToy: {e}");
            }
        }

        protected virtual string FormatMessage(Player player, string message)
        {
            return Main.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
        }

        private bool HasCooldown(Player player, out float remainingTime)
        {
            if (PlayerCooldowns.TryGetValue(player, out var cooldowns) && cooldowns.TryGetValue(OriginalCommand, out var cooldownEndTime))
            {
                remainingTime = cooldownEndTime - Time.time;
                return remainingTime > 0;
            }

            remainingTime = 0;
            return false;
        }

        private void SetCooldown(Player player)
        {
            if (!PlayerCooldowns.ContainsKey(player))
                PlayerCooldowns[player] = new Dictionary<string, float>();

            float cooldownDuration = Main.Instance.Config.GetCooldown(OriginalCommand);
            PlayerCooldowns[player][OriginalCommand] = Time.time + cooldownDuration;
        }

        private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
        {
            bool showInConsole = Main.Instance.Config.ShowCommandInSenderConsole;
            foreach (Player player in Player.List.Where(p => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
            {
                SendHint(player, message, duration);
                if (showInConsole)
                {
                    player.SendConsoleMessage($"{message}", "yellow");
                }
            }

            SendHint(sender, message, duration);

            if (showInConsole)
            {
                sender.SendConsoleMessage($"{message}", "yellow");
            }
        }

        private void SendHintToSpectatorsOfNearbyPlayers(Player sender, string message, float range, float duration)
        {
            HashSet<Player> spectatorsToSend = [];

            foreach (Player playerInRange in Player.List.Where(p => Vector3.Distance(p.Position, sender.Position) <= range))
            {
                foreach (Player spectator in playerInRange.CurrentSpectatingPlayers)
                {
                    if (spectator.IsConnected)
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

        public void SendHint(Player player, string message, float duration)
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
                foreach (Player observer in player.CurrentSpectatingPlayers)
                {
                    if (!observer.IsConnected) continue;

                    PlayerDisplay observerDisplay = PlayerDisplay.Get(observer);
                    observerDisplay?.AddHint(hint);
                    Timing.CallDelayed(duration, () => observerDisplay?.RemoveHint(hint));
                }
            }
        }
    }
}