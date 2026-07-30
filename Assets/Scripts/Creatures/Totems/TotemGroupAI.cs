using Assets.Scripts.Components;
using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Creatures.Totems
{
    public class TotemGroupAI : MonoBehaviour
    {
        [Header("Heads")]
        [SerializeField] protected List<HealthComponent> _heads = new List<HealthComponent>();
        [SerializeField] private UnityEvent _onDie;

        protected void Awake()
        {
            // Если список не заполнен в инспекторе, находим дочерние объекты с HealthComponent
            if (_heads.Count == 0)
            {
                var heads = GetComponentsInChildren<HealthComponent>();
                // Сортировка по Y (сверху вниз) — предполагаем, что верхняя голова имеет больший Y
                System.Array.Sort(heads, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
                _heads.AddRange(heads);
            }
        }

        protected void Update()
        {
            if (_heads.Count == 0)
                _onDie?.Invoke();
        }

        public void DistributeDamage(int baseDamage, int attack)
        {
            if (_heads.Count == 0) return;

            // Ищем первую живую голову
            HealthComponent target = null;
            for (int i = 0; i < _heads.Count; i++)
            {
                if (_heads[i] != null)
                {
                    target = _heads[i];
                    break;
                }
            }

            if (target != null)
            {
                var calcDamage = DamageCalc.CalculateDamage(baseDamage, attack, target.Defense);
                target.TakeDamage(calcDamage);
                if (target.Health <= 0)
                {
                    _heads.Remove(target);
                }
            }
        }
    }
}
