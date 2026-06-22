using UnityEngine;
using UnityEngine.Events;
using WSWhitehouse.TagSelector;

namespace Assets.Scripts.Components
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        [TagSelector][SerializeField] private string _tag;
        [SerializeField] private UnityEvent<GameObject> _action;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_tag))
            {
                _action?.Invoke(other.gameObject);
            }
        }
    }
}
