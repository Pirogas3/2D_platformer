using UnityEngine;
using UnityEngine.Events;
using WSWhitehouse.TagSelector;

namespace Scripts.Components
{
    public class EnterCollisionComponent : MonoBehaviour
    {
        [TagSelector][SerializeField] private string[] _tags;
        [SerializeField] private UnityEvent<GameObject> _action;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_tags == null || _tags.Length == 0) return;

            string colliderTag = collision.gameObject.tag;
            foreach (string tag in _tags)
            {
                if (tag == colliderTag)
                {
                    _action?.Invoke(collision.gameObject);
                    break;
                }
            }
        }
    }
}
