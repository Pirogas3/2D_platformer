using Assets.Scripts.Model.Data;
using Assets.Scripts.Utils;
using Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Props.Traps
{
    public class Spikes : MonoBehaviour
    {
        public void ApplyDamageFromSpikes(GameObject target)
        {
            var creature = target.GetInterface<IDamageFromSpikes>();
            creature?.TakeDamageFromSpikes();
        }
    }
}
