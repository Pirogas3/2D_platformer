using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public abstract class Patrol : MonoBehaviour
    {
        public abstract IEnumerator DoPatrol(Creature creature);
    }
}
