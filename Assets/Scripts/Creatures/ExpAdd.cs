using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class ExpAdd : MonoBehaviour
    {
        [SerializeField] private int _amount = 10;

        public void Add(GameObject go)
        {
            var hero = go.GetInterface<IAddExp>();
            hero?.AddExperience(_amount);
        }

        public void FindAndAdd()
        {
            Hero hero = FindObjectOfType<Hero>();
            hero?.AddExperience(_amount);
        }
    }
}
