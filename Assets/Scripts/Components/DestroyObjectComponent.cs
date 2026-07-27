using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class DestroyObjectComponent : MonoBehaviour
    {
        [SerializeField] public GameObject _objectToDestroy;
        [SerializeField] protected UnityEvent _action;

        public virtual void DestroyObject()
        {
            _action?.Invoke();
            Destroy(_objectToDestroy);
        }
    }
}
