using Assets.Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class HeroHealthComponent : HealthComponent
    {
        [SerializeField] private Hero _hero;

        public override int Defense
        {
            get
            {
                if (_hero != null)
                {
                    return _hero.defense;
                }
                else
                {
                    return base.Defense;
                }
            }
        }
    }
}
