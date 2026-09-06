using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using RPCommands.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace RPCommands.Components;

public class TextToy : MonoBehaviour
{
    private Player _owner;
    private AdminToys.TextToy _toy;
    private Transform _transform;
    private float _heightOffset;
    private CoroutineHandle _updateCoroutine;
    private bool _isStatic;
    private readonly float TickRate = Main.Instance.Config.TextToyTickRate;

    private readonly Dictionary<Player, Quaternion> _lastRotations = [];
    private readonly Dictionary<Player, Vector3> _lastScales = [];

    public void Initialize(Player owner, AdminToys.TextToy toy, float heightOffset)
    {
        _owner = owner;
        _toy = toy;
        _transform = toy.transform;
        _heightOffset = heightOffset;
        _isStatic = false;

        _updateCoroutine = Timing.RunCoroutine(UpdateToyCoroutine());
    }
    public void InitializeStatic(AdminToys.TextToy toy, Vector3 position)
    {
        _toy = toy;
        _transform = toy.transform;
        _transform.position = position;
        _isStatic = true;

        _updateCoroutine = Timing.RunCoroutine(UpdateToyCoroutine());
    }

    private void OnDestroy()
    {
        Timing.KillCoroutines(_updateCoroutine);
    }

    private IEnumerator<float> UpdateToyCoroutine()
    {
        float renderDistanceSqr = 60f * 60f;

        while (true)
        {
            if (!_isStatic && (_owner == null || _owner.IsDestroyed || !_owner.IsAlive))
            {
                DestroyToy();
                yield break;
            }

            if (_toy == null) yield break;

            if (!_isStatic)
            {
                _transform.position = _owner.Position + (Vector3.up * _heightOffset);
            }

            foreach (Player observer in Player.List)
            {
                if (observer == null || observer.IsDestroyed || (!_isStatic && observer == _owner))
                    continue;

                float sqrDistance = (_transform.position - observer.Position).sqrMagnitude;

                if (sqrDistance > renderDistanceSqr)
                {
                    if (!_lastScales.TryGetValue(observer, out Vector3 lastScale) || lastScale != Vector3.zero)
                    {
                        observer.SendFakeSyncVar(_toy, 4, Vector3.zero);
                        _lastScales[observer] = Vector3.zero;
                    }
                    continue;
                }

                if (!_lastScales.TryGetValue(observer, out Vector3 currentScale) || currentScale != Vector3.one)
                {
                    observer.SendFakeSyncVar(_toy, 4, Vector3.one);
                    _lastScales[observer] = Vector3.one;
                }

                Vector3 direction = observer.Position - _transform.position;
                direction.y = 0;

                Quaternion newRotation = direction != Vector3.zero
                    ? Quaternion.LookRotation(-direction)
                    : Quaternion.identity;

                if (!_lastRotations.TryGetValue(observer, out Quaternion lastRot) || Quaternion.Angle(lastRot, newRotation) > 3f)
                {
                    observer.SendFakeSyncVar(_toy, 2, newRotation);
                    _lastRotations[observer] = newRotation;
                }
            }

            yield return Timing.WaitForSeconds(TickRate);
        }
    }

    public void DestroyToy()
    {
        if (_toy != null && _toy.gameObject != null)
        {
            NetworkServer.Destroy(_toy.gameObject);
        }
        Destroy(this);
    }
}