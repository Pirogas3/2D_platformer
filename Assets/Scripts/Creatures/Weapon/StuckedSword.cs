using UnityEngine;
using Scripts.Creatures;

namespace Assets.Scripts.Creatures.Weapon
{
    public class StuckedSword : MonoBehaviour
    {
        [SerializeField] private int _count = 1;

        private Hero _hero;

        private void Start()
        {
            GameObject heroGO = GameObject.FindWithTag("Player");
            if (heroGO != null)
                _hero = heroGO.GetComponent<Hero>();
        }

        public void СollectSword()
        {
            if (_hero != null)
            {
                _hero.CollectSword(_count);
            }
        }
    }
}
