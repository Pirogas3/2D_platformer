using UnityEngine;

namespace Scripts.Components
{
    public class TeleportComponent : MonoBehaviour
    {
        [SerializeField] private Transform _destTranform;

        public void Teleport(GameObject target)
        {
            target.transform.position = _destTranform.position;
        }
    }
}
