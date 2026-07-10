using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField] protected UnityEvent<GameObject> _action;

        public virtual void Interact(GameObject target)
        {
            _action?.Invoke(target);
        }
    }
}
