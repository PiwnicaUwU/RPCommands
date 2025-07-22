using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using RemoteAdmin;
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

            if (!player.IsAlive && OriginalCommand != "assist") // assist ALL always MUST work
            {
                response = Main.Instance.Translation.OnlyAlive;
                return false;
            }

            if (arguments.Count < 1 && OriginalCommand != "wear")
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

            HintToNearbyPlayers(player, formattedMessage, range, duration);
            response = Main.Instance.Translation.MessageSent;
            return true;
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

        public void SendHint(Player player, string message, float duration)
        {
            DynamicHint hint = new()
            {
                Text = message,
                TargetY = 800,
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

    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : RPCommand
    {
        public override string OriginalCommand => "me";
        public override string Description => Main.Instance.Translation.Commands["me"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : RPCommand
    {
        public override string OriginalCommand => "do";
        public override string Description => Main.Instance.Translation.Commands["do"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class LookCommand : RPCommand
    {
        public override string OriginalCommand => "look";
        public override string Description => Main.Instance.Translation.Commands["look"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : RPCommand
    {
        public override string OriginalCommand => "ooc";
        public override string Description => Main.Instance.Translation.Commands["ooc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DescCommand : RPCommand
    {
        public override string OriginalCommand => "desc";
        public override string Description => Main.Instance.Translation.Commands["desc"];
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CustomInfoCommand : RPCommand
    {
        public override string OriginalCommand => "custom-info";
        public override string Description => Main.Instance.Translation.Commands["custom-info"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            int maxLength = Main.Instance.Config.MaxCustomInfoLength;

            if (message.Length > maxLength)
            {
                response = Main.Instance.Translation.CustomInfoTooLong;
                return false;
            }

            player.CustomInfo = message;
            response = Main.Instance.Translation.CustomInfoSet;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HelpCommand : RPCommand
    {
        public override string OriginalCommand => "assist";
        public override string Description => Main.Instance.Translation.Commands["assist"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            foreach (Player staff in Player.List.Where(p => p.ReferenceHub.serverRoles.RemoteAdmin))
            {
                staff.SendStaffMessage(FormatMessage(player, message));
            }

            response = Main.Instance.Translation.HelpRequestSent;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TryCommand : RPCommand
    {
        public override string OriginalCommand => "try";
        public override string Description => Main.Instance.Translation.Commands["try"];

        protected override string FormatMessage(Player player, string message)
        {
            bool isSuccess = UnityEngine.Random.Range(0, 2) == 0;
            string resultKey = isSuccess ? "success" : "fail";
            string result = Main.Instance.Translation.TryResult[resultKey];

            return Main.Instance.Config.FormatMessage("try", player.Nickname, message, result);
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class RadioCommand : RPCommand
    {
        public override string OriginalCommand => "radio";
        public override string Description => Main.Instance.Translation.Commands["radio"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            if (player.CurrentItem == null || player.CurrentItem.Type != ItemType.Radio)
            {
                response = Main.Instance.Translation.RadioRequired;
                return false;
            }

            float duration = Main.Instance.Config.GetDuration(OriginalCommand);
            string formattedMessage = FormatMessage(player, message);

            foreach (Player receiver in Player.List)
            {
                if (!receiver.IsAlive || receiver.CurrentItem?.Type != ItemType.Radio)
                    continue;

                SendHint(receiver, formattedMessage, duration);
            }

            response = Main.Instance.Translation.MessageSent;
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class WearCommand : RPCommand
    {
        public override string OriginalCommand => "wear";
        public override string Description => Main.Instance.Translation.Commands["wear"];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            var nearestRagdoll = FindNearestRagdoll(player);

            if (player.IsScp)
            {
                response = Main.Instance.Translation.ScpCantwear;
                return false;
            }

            if (!nearestRagdoll.HasValue)
            {
                response = Main.Instance.Translation.NoDeadBodyFound;
                return false;
            }

            if (WearDeadPlayer(player, nearestRagdoll.Value))
            {
                response = Main.Instance.Translation.Wore;
                return true;
            }
            else
            {
                response = Main.Instance.Translation.WoreFailure;
                return false;
            }
        }

        private RagdollData? FindNearestRagdoll(Player player)
        {
            Vector3 playerPosition = player.Position;
            RagdollData? nearestRagdoll = null;
            float nearestDistance = float.MaxValue;
            const float maxDistance = 3f;

            foreach (var ragdoll in Ragdoll.List)
            {
                float distance = Vector3.Distance(playerPosition, ragdoll.Position);
                if (distance <= maxDistance)
                {
                    if (IsPlayerLookingAt(player, ragdoll.Position) || distance <= 1.5f)
                    {
                        if (ragdoll.Role.IsScp())
                        {
                            player.SendConsoleMessage(Main.Instance.Translation.ScpClothesNotAllowed, "red");
                            break;
                        }

                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestRagdoll = ragdoll.NetworkInfo;
                        }
                    }
                }
            }
            return nearestRagdoll;
        }

        private bool IsPlayerLookingAt(Player player, Vector3 targetPosition)
        {
            Vector3 playerForward = player.CameraTransform.forward;
            Vector3 directionToTarget = (targetPosition - player.CameraTransform.position).normalized;
            float angle = Vector3.Angle(playerForward, directionToTarget);
            return angle <= 45f;
        }

        private bool WearDeadPlayer(Player player, RagdollData ragdollData)
        {
            try
            {
                if (ragdollData.OwnerHub != null)
                {
                    var ragdollToRemove = Ragdoll.List.FirstOrDefault(r => r.NetworkInfo.Equals(ragdollData));
                    var ragdollPosition = ragdollToRemove?.Position ?? player.Position;
                    var originalRole = player.Role.Type;
                    var originalNickname = player.Nickname;

                    switch (Main.Instance.Config.WearMode.ToLower())
                    {
                        case "rolechange":
                            player.Role.Set(ragdollData.RoleType, SpawnReason.ForceClass, RoleSpawnFlags.None);
                            Timing.CallDelayed(0.1f, () =>
                            {
                                player.DisplayNickname = ragdollData.OwnerHub.nicknameSync.MyNick;
                                player.Position = ragdollPosition;
                                ragdollToRemove?.Destroy();
                            });
                            break;

                        case "modelchange":
                            player.ChangeAppearance(ragdollData.RoleType, true);
                            player.DisplayNickname = ragdollData.OwnerHub.nicknameSync.MyNick;
                            ragdollToRemove?.Destroy();
                            break;

                        default:
                            Log.Warn($"Invalid WearMode '{Main.Instance.Config.WearMode}' in config. Please use 'rolechange' or 'modelchange'.");
                            player.SendConsoleMessage("An error occurred while trying to wear the dead player. Contact server staff.", "red");
                            return false;
                    }
                    float disguiseDuration = Main.Instance.Config.WearDuration;

                    if (disguiseDuration >= 0f)
                    {
                        Timing.CallDelayed(disguiseDuration, () =>
                        {
                            if (player == null || !player.IsConnected)
                                return;

                            switch (Main.Instance.Config.WearMode.ToLower())
                            {
                                case "rolechange":
                                    if (player.Role.Type == ragdollData.RoleType)
                                    {
                                        player.Role.Set(originalRole, RoleSpawnFlags.None);
                                        player.DisplayNickname = originalNickname;
                                    }
                                    break;

                                case "modelchange":
                                    player.ChangeAppearance(originalRole, true);
                                    player.DisplayNickname = originalNickname;
                                    break;
                            }

                            player.ShowHint(Main.Instance.Translation.DisguiseWornOff, 7f);
                        });
                    }
                    return true;
                }
                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class PunchCommand : RPCommand
        {
            private static readonly Dictionary<Player, float> cooldowns = new();

            public override string OriginalCommand => "punch";
            public override string Description => Main.Instance.Translation.Commands["punch"];

            public bool ExecuteAction(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is not PlayerCommandSender playerSender)
                {
                    response = "Only humans can use this command.";
                    return false;
                }

                Player attacker = Player.Get(playerSender.ReferenceHub);
                float time = Time.time;

                Player player = Player.Get(playerSender.ReferenceHub);

                if (player.Role.Team == Team.SCPs)
                {
                    response = "Only humans can use this command.";
                    return false;
                }

                if (cooldowns.TryGetValue(attacker, out float lastUse) && time - lastUse < 3f)
                {
                    response = $"to use the command, you have to wait {Math.Round(3f - (time - lastUse), 1)} seconds.";
                    return false;
                }

                if (Physics.Raycast(attacker.CameraTransform.position, attacker.CameraTransform.forward, out RaycastHit hit, 5f))
                {
                    ReferenceHub targetHub = hit.collider.GetComponentInParent<ReferenceHub>();
                    if (targetHub != null)
                    {
                        Player target = Player.Get(targetHub);
                        if (target != null && target != attacker)
                        {
                            target.Hurt(new UniversalDamageHandler(5, DeathTranslations.Unknown));

                            Vector3 pushDirection = (target.Position - attacker.Position).normalized + Vector3.up * 0.5f;
                            target.Position += pushDirection * 0.7f;

                            target.ShowHint($"<color=red>You got punched by {attacker.Nickname}</color>!", 5f);
                            cooldowns[attacker] = time;

                            response = $"You punched <color=green>{target.Nickname}</color>!";
                            return true;
                        }
                    }
                }

                response = "No target in range.";
                return false;
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class CzyscCommand : RPCommand
        {
            public override string OriginalCommand => "clean";
            public override string Description => Main.Instance.Translation.Commands["clean"];

            public bool ExecuteAction(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                PlayerCommandSender val = sender as PlayerCommandSender;
                if (val == null)
                {
                    response = "Only humans can use this command.";
                    return false;
                }

                Player player = Player.Get(val.ReferenceHub);


                if (player.Role.Team == Team.SCPs)
                {
                    response = "Only humans can use this command.";
                    return false;
                }

                Ragdoll val2 = FindRagdollUnderPlayer(player);
                if (val2 == null)
                {
                    response = "You are not looking on ragdoll.";
                    return false;
                }

                NetworkServer.Destroy(val2.GameObject);
                Player val3 = FindNearestPlayerInRoom(player);
                response = ((val3 != null) ? "Ragdoll was cleaned" : "Ragdoll was cleaned");
                return true;
            }

            private Ragdoll FindRagdollUnderPlayer(Player player)
            {
                Ragdoll result = null;
                float num = float.MaxValue;
                Vector3 position = player.Position;
                foreach (Ragdoll item in Ragdoll.List)
                {
                    float num2 = Vector3.Distance(position, item.Position);
                    if (num2 < 4f && num2 < num)
                    {
                        result = item;
                        num = num2;
                    }
                }
                return result;
            }

            public Player FindNearestPlayerInRoom(Player player)
            {
                Room currentRoom = player.CurrentRoom;
                Player result = null;
                float num = float.MaxValue;

                foreach (Player item in Player.List)
                {
                    if (item != player && item.CurrentRoom == currentRoom && item.IsAlive)
                    {
                        float num2 = Vector3.Distance(player.Position, item.Position);
                        if (num2 < num)
                        {
                            result = item;
                            num = num2;
                        }
                    }
                }
                return result;
            }
        }

        [CommandHandler(typeof(ClientCommandHandler))]
        public class HealCommand : RPCommand
        {
            public override string OriginalCommand => "heal";
            public override string Description => Main.Instance.Translation.Commands["heal"];

            public bool ExecuteAction(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                PlayerCommandSender playerSender = sender as PlayerCommandSender;
                if (playerSender == null)
                {
                    response = "Only humans can use this command.";
                    return false;
                }

                Player player = Player.Get(playerSender.ReferenceHub);

                if (player.Role.Team == Team.SCPs)
                {
                    response = "Only humans can use this command..";
                    return false;
                }

                Player healer = Player.Get(playerSender.ReferenceHub);
                if (healer.CurrentItem == null || healer.CurrentItem.Type != ItemType.Medkit)
                {
                    response = "You have to hold Medkit, to use this command";
                    return false;
                }

                if (Physics.Raycast(healer.CameraTransform.position, healer.CameraTransform.forward, out RaycastHit hit, 3.0f))
                {
                    ReferenceHub targetHub = hit.collider.GetComponentInParent<ReferenceHub>();
                    if (targetHub != null)
                    {
                        Player target = Player.Get(targetHub);
                        if (target != null && target != healer)
                        {
                            target.Health = Math.Min(target.Health + 65f, target.MaxHealth);
                            healer.RemoveItem(healer.CurrentItem, true);

                            response = $"You healed {target.Nickname}.";
                            target.ShowHint($"You got healed by {healer.Nickname}.", 5f);
                            return true;
                        }
                    }
                }

                response = "No target in range.";
                return false;
            }
        }
    }
}