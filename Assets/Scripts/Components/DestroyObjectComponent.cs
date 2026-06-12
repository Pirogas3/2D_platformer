using UnityEngine;

namespace Scripts.Components
{
    public class DestroyObjectComponent : MonoBehaviour
    {
        [SerializeField] public GameObject _objectToDestroy;

        public void DestroyObject()
        {
            Destroy(_objectToDestroy);
        }
    }
}
