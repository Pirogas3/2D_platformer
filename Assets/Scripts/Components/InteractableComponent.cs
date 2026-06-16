using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<GameObject> _action;

        public void Interact(GameObject target)
        {
            _action?.Invoke(target);
        }
    }
}
