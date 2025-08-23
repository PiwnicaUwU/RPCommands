using Exiled.API.Features;
using Mirror;
using RpCommands.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RpCommands.Components
{
    public class TextToy : MonoBehaviour
    {
        private Player _owner;
        private AdminToys.TextToy _toy;
        private Transform _transform;
        private float _heightOffset;

        public void Initialize(Player owner, AdminToys.TextToy toy, float heightOffset)
        {
            _owner = owner;
            _toy = toy;
            _transform = toy.transform;
            _heightOffset = heightOffset;
        }

        public void FixedUpdate()
        {
            if (_toy == null || _owner == null || !_owner.IsConnected || !_owner.IsAlive)
            {
                DestroyToy();
                return;
            }

            _transform.position = _owner.Position + (Vector3.up * _heightOffset);

            foreach (Player observer in Player.List)
            {
                if (observer == null || !observer.IsConnected || observer == _owner)
                    continue;

                if (Vector3.Distance(_transform.position, observer.Position) > 60f)
                {
                    observer.SendFakeSyncVar(_toy, 4, Vector3.zero);
                    continue;
                }

                observer.SendFakeSyncVar(_toy, 4, Vector3.one);

                Vector3 direction = observer.Position - _transform.position;
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(-direction);

                observer.SendFakeSyncVar(_toy, 2, rotation);
            }
        }

        public void DestroyToy()
        {
            if (_toy != null)
            {
                NetworkServer.Destroy(_toy.gameObject);
            }
            Destroy(this);
        }
    }
}
