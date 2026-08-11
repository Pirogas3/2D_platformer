using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class ExpAdd : MonoBehaviour
    {
        [SerializeField] private int _amount = 10;
        private bool _used = false;

        public void Add(GameObject go)
        {
            var hero = go.GetInterface<IAddExp>();
            hero?.AddExperience(_amount);
        }

        public void FindAndAdd()
        {
            if (_used) return;

            _used = true;
            Hero hero = FindObjectOfType<Hero>();
            hero?.AddExperience(_amount);
        }
    }
}
