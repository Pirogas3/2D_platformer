using Assets.Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class HeroDamageComponent : DamageComponent
    {
        [SerializeField] private Hero _hero;

        public override int Attack
        {
            get
            {
                if (_hero != null)
                {
                    return _hero.attack;
                }
                else
                {
                    return base.Attack;
                }
            }
        }
    }
}
