using UnityEngine;

namespace Assets.Scripts.Components
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
