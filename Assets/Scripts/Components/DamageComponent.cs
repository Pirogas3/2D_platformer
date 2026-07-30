using Assets.Scripts.Creatures.Totems;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DamageComponent : MonoBehaviour
    {
        [SerializeField] private int _damage;
        [SerializeField] private int _attack = 0;

        public virtual int Attack => _attack;


        public void SetDamage(int value) => _damage = value;
        public void SetAttack(int value) => _attack = value;

        public void ApplyDamage(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(_damage);
            }
        }

        public void NewApplyDamage(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                var calcDamage = DamageCalc.CalculateDamage(_damage, Attack, healthComponent.Defense);
                healthComponent.TakeDamage(calcDamage);
                Debug.Log($"Юнитом: {name} - нанесён урон равный = {calcDamage}");
            }

            var totemGroup = target.GetComponent<TotemGroupAI>();
            if (totemGroup != null)
            {
                totemGroup.DistributeDamage(_damage, Attack);
            }
        }

        public void NewApplyDamage(GameObject target, int baseDamage, int attack)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                var calcDamage = DamageCalc.CalculateDamage(baseDamage, attack, healthComponent.Defense);
                healthComponent.TakeDamage(calcDamage);
                Debug.Log($"Юнитом: {name} - нанесён урон равный = {calcDamage}");
            }

            var totemGroup = target.GetComponent<TotemGroupAI>();
            if (totemGroup != null)
            {
                totemGroup.DistributeDamage(baseDamage, attack);
            }
        }

        public void ApplyHeal(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.TakeHeal(_damage);
            }
        }
    }
}
