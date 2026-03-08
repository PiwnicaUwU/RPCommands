using AdminToys;
using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using RPCommands.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextToy = AdminToys.TextToy;

namespace RPCommands.Commands
{
    public class ActiveZone
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public float Duration { get; set; }
        public string CreatorName { get; set; }
        public string Message { get; set; }
        public DateTime CreationTime { get; set; }
        public TextToy ZoneToy { get; set; }
    }

    public class ZoneCommand : RPCommand
    {
        public override string OriginalCommand => "zone";
        public override string Description => Main.Instance.Config.Translation.Commands["zone"];
        public static List<ActiveZone> ActiveZones { get; } = [];

        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            var newZone = new ActiveZone
            {
                Position = player.Position,
                Radius = Main.Instance.Config.GetRange("zone"),
                Duration = Main.Instance.Config.GetDuration("zone"),
                CreatorName = player.Nickname,
                Message = message,
                CreationTime = DateTime.Now
            };

            if (Main.Instance.Config.ZoneActivationMode == ZoneActivationMode.TextToy)
            {
                try
                {
                    AdminToys.TextToy prefab = NetworkClient.prefabs.Values
                        .Select(p => p.GetComponent<AdminToys.TextToy>())
                        .FirstOrDefault(t => t != null);

                    TextToy textToy = UnityEngine.Object.Instantiate(prefab);

                    textToy.transform.position = newZone.Position + (Vector3.up * 0.5f);
                    textToy.transform.rotation = Quaternion.identity;

                    string consoleMessage = Main.Instance.Config.FormatMessage("zone", newZone.CreatorName, newZone.Message);

                    textToy.TextFormat = $"<size={Main.Instance.Config.ZoneTextToySize}>{consoleMessage}</size>";

                    NetworkServer.Spawn(textToy.gameObject);

                    foreach (Player p in Player.List.Where(p => p != null && !p.IsDestroyed))
                    {
                        if (Vector3.Distance(p.Position, newZone.Position) <= newZone.Radius)
                        {
                            p.SendConsoleMessage(consoleMessage, "yellow");
                        }
                    }

                    textToy.gameObject.AddComponent<Components.StaticTextToy>();

                    newZone.ZoneToy = textToy;
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to spawn Zone TextToy: {e}");
                    response = "Error spawning TextToy for zone.";
                    return false;
                }
            }

            ActiveZones.Add(newZone);

            response = string.Format(Main.Instance.Config.Translation.ZoneSuccess, Mathf.RoundToInt(newZone.Duration));
            return true;
        }

        public static IEnumerator<float> ZoneCoroutine()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(0.5f);

                for (int i = ActiveZones.Count - 1; i >= 0; i--)
                {
                    var zone = ActiveZones[i];
                    if ((DateTime.Now - zone.CreationTime).TotalSeconds >= zone.Duration)
                    {
                        if (zone.ZoneToy != null)
                        {
                            NetworkServer.Destroy(zone.ZoneToy.gameObject);
                        }
                        ActiveZones.RemoveAt(i);
                    }
                }

                if (Main.Instance.Config.ZoneActivationMode == ZoneActivationMode.Hint)
                {
                    string hintFormat = Main.Instance.Config.GetSettings("zone").Format;

                    foreach (Player p in Player.List)
                    {
                        if (p.IsDestroyed || p.Role.IsDead())
                            continue;

                        foreach (ActiveZone zone in ActiveZones)
                        {
                            if (Vector3.Distance(p.Position, zone.Position) <= zone.Radius)
                            {
                                string formattedHint = string.Format(hintFormat, zone.CreatorName, zone.Message);
                                p.SendHint("<size=25>" + "<align=left>" + formattedHint + "</align>" + "</size>", 1); // too lazy to do with HSM
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}