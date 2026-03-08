using LabApi.Features.Wrappers;
using RPCommands.Extensions;
using UnityEngine;

namespace RPCommands.Components
{
    public class StaticTextToy : MonoBehaviour
    {
        private AdminToys.TextToy _toy;
        private Transform _transform;

        private void Awake()
        {
            _toy = GetComponent<AdminToys.TextToy>();
            _transform = transform;
        }

        public void FixedUpdate()
        {
            if (_toy == null)
            {
                Destroy(this);
                return;
            }

            foreach (Player observer in Player.List)
            {
                if (observer == null || !observer.IsDestroyed)
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
    }
}