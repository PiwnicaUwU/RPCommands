using Exiled.API.Features;
using MEC;
using RPCommands;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RpCommands.Commands
{
    public class ActiveZone
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public float Duration { get; set; }
        public string CreatorName { get; set; }
        public string HintMessage { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class ZoneCommand : RPCommand
    {
        public override string OriginalCommand => "zone";
        public override string Description => Main.Instance.Translation.Commands["zone"];
        public override bool AllowNoArguments => true;
        public static List<ActiveZone> ActiveZones { get; } = [];
        protected override bool ExecuteAction(Player player, string message, out string response)
        {
            var newZone = new ActiveZone
            {
                Position = player.Position,
                Radius = Main.Instance.Config.GetRange("zone"),
                Duration = Main.Instance.Config.GetDuration("zone"),
                CreatorName = player.Nickname,
                HintMessage = message,
                CreationTime = DateTime.Now
            };

            ActiveZones.Add(newZone);

            response = Main.Instance.Translation.ZoneSuccess;
            return true;
        }
        public static IEnumerator<float> ZoneCoroutine()
        {
            string hintFormat = Main.Instance.Config.GetSettings("zone").Format;

            while (true)
            {
                yield return Timing.WaitForSeconds(0.5f);

                ActiveZones.RemoveAll(zone => (DateTime.Now - zone.CreationTime).TotalSeconds >= zone.Duration);

                foreach (Player p in Player.List)
                {
                    if (!p.IsConnected || p.IsDead)
                        continue;

                    foreach (ActiveZone zone in ActiveZones)
                    {
                        if (Vector3.Distance(p.Position, zone.Position) <= zone.Radius)
                        {
                            string formattedHint = string.Format(hintFormat, zone.CreatorName, zone.HintMessage);
                            p.ShowHint(formattedHint, 1);
                            break;
                        }
                    }
                }
            }
        }
    }
}