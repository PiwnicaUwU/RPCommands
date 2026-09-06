using HintServiceMeow.Core.Models.Hints;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using RPCommands.Components;
using RPCommands.Enum;
using RPCommands.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextToy = AdminToys.TextToy;

namespace RPCommands.Commands;

public class Zone
{
    private CoroutineHandle _zoneTicker;
    private readonly Dictionary<Player, DynamicHint> _activeHints = [];
    public ActiveZone activeZone;

    public void StartZone(ActiveZone zoneData)
    {
        activeZone = zoneData;
        ZoneCommand.ActiveZones.Add(this);
        _zoneTicker = Timing.RunCoroutine(ZoneTickCoroutine());
    }

    private IEnumerator<float> ZoneTickCoroutine()
    {
        float elapsedTime = 0f;
        float tickRate = Main.Instance.Config.ZoneHintTickRate;

        bool isHintMode = Main.Instance.Config.ZoneActivationMode == ZoneActivationMode.Hint;

        float sqrRadius = activeZone.Radius * activeZone.Radius;

        while (elapsedTime < activeZone.Duration)
        {
            if (isHintMode)
            {
                foreach (Player observer in Player.List)
                {
                    if (observer == null || observer.IsDestroyed) continue;

                    float sqrDistance = (activeZone.Position - observer.Position).sqrMagnitude;
                    bool inRange = sqrDistance <= sqrRadius && observer.IsAlive;

                    if (inRange)
                    {
                        if (!_activeHints.ContainsKey(observer))
                        {
                            DynamicHint zoneHint = observer.AddPersistentHint(activeZone.Message);
                            if (zoneHint != null)
                            {
                                _activeHints[observer] = zoneHint;
                            }
                        }
                    }
                    else
                    {
                        if (_activeHints.TryGetValue(observer, out DynamicHint activeHint))
                        {
                            observer.RemovePersistentHint(activeHint);
                            _activeHints.Remove(observer);
                        }
                    }
                }
            }

            elapsedTime += tickRate;
            yield return Timing.WaitForSeconds(tickRate);
        }

        DestroyZone();
    }

    public void DestroyZone()
    {
        if (activeZone.ZoneToy != null && activeZone.ZoneToy.gameObject != null)
        {
            NetworkServer.Destroy(activeZone.ZoneToy.gameObject);
        }

        foreach (var kvp in _activeHints)
        {
            DisplayHandler.RemovePersistentHint(kvp.Key, kvp.Value);
        }
        _activeHints.Clear();

        ZoneCommand.ActiveZones.Remove(this);
    }
}

internal sealed class ZoneCommand : InternalRPCommand
{
    public override string OriginalCommand => "zone";
    public override string Description => Main.Instance.Config.Translation.Commands["zone"];
    public static List<Zone> ActiveZones { get; } = [];

    protected override bool ExecuteAction(Player player, string message, out string response)
    {
        string formattedMessage = Main.Instance.Config.FormatMessage("zone", player.Nickname, message);

        var newZoneData = new ActiveZone
        {
            Position = player.Position,
            Radius = Main.Instance.Config.GetRange("zone"),
            Duration = Main.Instance.Config.GetDuration("zone"),
            CreatorName = player.Nickname,
            Message = formattedMessage
        };

        if (Main.Instance.Config.ZoneActivationMode == ZoneActivationMode.TextToy)
        {
            try
            {
                TextToy prefab = NetworkClient.prefabs.Values
                    .Select(p => p.GetComponent<TextToy>())
                    .FirstOrDefault(t => t != null);

                if (prefab == null)
                {
                    response = "Something went wrong.";
                    Logger.Error("TextToy prefab not found");
                    return false;
                }

                TextToy textToy = UnityEngine.Object.Instantiate(prefab);
                textToy.transform.position = newZoneData.Position + (Vector3.up * 0.5f);
                textToy.transform.rotation = Quaternion.identity;
                textToy.TextFormat = $"<size={Main.Instance.Config.ZoneTextToySize}>{newZoneData.Message}</size>";

                NetworkServer.Spawn(textToy.gameObject);

                var controller = textToy.gameObject.AddComponent<Components.TextToy>();
                controller.InitializeStatic(textToy, textToy.transform.position);

                float sqrRadius = newZoneData.Radius * newZoneData.Radius;
                foreach (Player p in Player.List.Where(p => p != null && !p.IsDestroyed))
                {
                    if ((p.Position - newZoneData.Position).sqrMagnitude <= sqrRadius)
                    {
                        p.SendConsoleMessage(newZoneData.Message, "yellow");
                    }
                }

                newZoneData.ZoneToy = textToy;
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to spawn Zone TextToy: {e}");
                response = "Something went wrong.";
                return false;
            }
        }

        var zoneController = new Zone();
        zoneController.StartZone(newZoneData);

        response = string.Format(Main.Instance.Config.Translation.ZoneSuccess, Mathf.RoundToInt(newZoneData.Duration));
        return true;
    }
}