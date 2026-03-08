using CommandSystem;
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
        public virtual string Command => Main.Instance.Config.Translation.CommandNames.TryGetValue(OriginalCommand, out string translatedName) ? translatedName : OriginalCommand;
        public virtual bool IsCommandEnabled => Main.Instance.Config.IsCommandEnabled(OriginalCommand);
        public virtual bool AllowScp => Main.Instance.Config.AllowScpToUseCommands;
        public virtual float CommandCooldown => Main.Instance.Config.GetCooldown(OriginalCommand);
        public virtual float CommandRange => Main.Instance.Config.GetRange(OriginalCommand);
        public virtual float CommandDuration => Main.Instance.Config.GetDuration(OriginalCommand);
        public virtual RPCommandsMode DisplayMode => Main.Instance.Config.DisplayMode;
        public virtual string MsgOnlyPlayers => Main.Instance.Config.Translation.OnlyPlayers;
        public virtual string MsgCommandDisabled => Main.Instance.Config.Translation.CommandDisabled;
        public virtual string MsgRoundNotStarted => Main.Instance.Config.Translation.RoundNotStarted;
        public virtual string MsgOnlyHumans => Main.Instance.Config.Translation.OnlyHumans;
        public virtual string MsgOnlyAlive => Main.Instance.Config.Translation.OnlyAlive;
        public virtual string MsgUsage => Main.Instance.Config.Translation.Usage;
        public virtual string MsgCooldown => Main.Instance.Config.Translation.CommandCooldown;
        public virtual string MsgBannedWord => Main.Instance.Config.Translation.BannedWordDetected;
        public virtual string MsgSent => Main.Instance.Config.Translation.MessageSent;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = MsgOnlyPlayers;
                return false;
            }
            Player player = Player.Get(playerSender.ReferenceHub);

            if (!IsCommandEnabled)
            {
                response = MsgCommandDisabled;
                return false;
            }

            if (!Round.IsRoundStarted && OriginalCommand != "assist") // assist must work even if round is not started
            {
                response = MsgRoundNotStarted;
                return false;
            }

            if (player.IsSCP && !AllowScp)
            {
                response = MsgOnlyHumans;
                return false;
            }

            if (!player.IsAlive && OriginalCommand != "assist")
            {
                response = MsgOnlyAlive;
                return false;
            }

            if (arguments.Count < 1 && !AllowNoArguments)
            {
                response = string.Format(MsgUsage, Command);
                return false;
            }

            if (player.HasCooldown(OriginalCommand))
            {
                float remainingTime = player.GetRemainingCooldown(OriginalCommand);
                if (MsgCooldown.Contains("{0}"))
                    response = string.Format(MsgCooldown, Math.Ceiling(remainingTime));
                else
                    response = MsgCooldown;

                return false;
            }

            string rawMessage = string.Join(" ", arguments);

            var sendingArgs = new PlayerSendingRpCommandEventArgs(player, OriginalCommand, rawMessage);
            Events.OnSendingRpCommand(sendingArgs);

            if (!sendingArgs.IsAllowed)
            {
                response = "Command cancelled by plugin.";
                return false;
            }

            string message = sendingArgs.Message;

            if (Main.Instance.Config.BannedWords.Any(bannedWord => message.ToLower().Contains(bannedWord.ToLower())))
            {
                response = MsgBannedWord;
                return false;
            }

            if (!ExecuteAction(player, message, out response))
                return false;

            player.SetCooldown(OriginalCommand, CommandCooldown);
            player.SetLastMessage(OriginalCommand, message);
            return true;
        }

        protected virtual bool ExecuteAction(Player player, string message, out string response)
        {
            string formattedMessage = FormatMessage(player, message);
            DisplayMessage(player, formattedMessage, CommandRange, CommandDuration);

            response = MsgSent;
            return true;
        }

        private void DisplayMessage(Player sender, string message, float range, float duration)
        {
            switch (DisplayMode)
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

        protected virtual string FormatMessage(Player player, string message)
        {
            return Main.Instance.Config.FormatMessage(OriginalCommand, player.Nickname, message);
        }

        private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
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

        private void SendHintToSpectatorsOfNearbyPlayers(Player sender, string message, float range, float duration)
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