using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Components
{
    public class TeleportComponent : MonoBehaviour
    {
        [SerializeField] private Transform _destTranform;

        public void Teleport(GameObject target)
        {
            Debug.Log("Работает");
            target.transform.position = _destTranform.position;
        }
    }
}
