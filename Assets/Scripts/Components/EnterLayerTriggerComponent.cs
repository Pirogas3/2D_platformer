using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class EnterLayerTriggerComponent : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        [SerializeField] private UnityEvent<GameObject> _action;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.IsInLayer(_layer))
            {
                _action?.Invoke(other.gameObject);
            }
        }
    }
}
