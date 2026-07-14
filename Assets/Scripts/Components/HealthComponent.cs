using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent<int> OnHealthChanged;
        [SerializeField] private int _defense = 0;

        public virtual int Defense => _defense;

        public void SetDefense(int value) => _defense = value;

        public void TakeDamage(int damage)
        {
            _health -= damage;
            OnHealthChanged?.Invoke(_health);
            if (_health <= 0)
            {
                gameObject.tag = "Untagged";
                _onDie?.Invoke();
            }
            else
            {
                _onDamage?.Invoke();
            }
        }

        public void TakeHeal(int heal)
        {
            _health += heal;
            OnHealthChanged?.Invoke(_health);
            _onHeal?.Invoke();
        }

        public void SetHealth(int value)
        {
            _health = value;
        }
    }
}
