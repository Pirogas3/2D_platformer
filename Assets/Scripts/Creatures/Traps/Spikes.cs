using Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Creatures.Traps
{
    public class Spikes : MonoBehaviour
    {
        public void TakeDamageFromSpikes(GameObject target)
        {
            Creature creature = target.GetComponent<Creature>();
            if (creature != null)
            {
                creature.TakeDamageFromSpikes();
            }
        }
    }
}
