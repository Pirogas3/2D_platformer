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

        private BoxCollider2D _collider;
        private Vector2 _initialColliderSize;
        private Vector2 _initialColliderOffset;
        private int _initialHeadCount;


        protected void Awake()
        {
            if (_heads.Count == 0)
            {
                var heads = GetComponentsInChildren<HealthComponent>();
                System.Array.Sort(heads, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
                _heads.AddRange(heads);
            }

            _collider = GetComponent<BoxCollider2D>();
            if (_collider != null)
            {
                _initialColliderSize = _collider.size;
                _initialColliderOffset = _collider.offset;
                _initialHeadCount = _heads.Count;
                UpdateColliderSize();
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
                    UpdateColliderSize();
                }
            }
        }

        private void UpdateColliderSize()
        {
            if (_collider == null || _initialHeadCount == 0) return;

            int currentCount = _heads.Count;
            if (currentCount == 0)
            {
                _collider.enabled = false;
                return;
            }

            float ratio = (float)currentCount / _initialHeadCount;

            // Новая высота
            float newHeight = _initialColliderSize.y * ratio;

            // Сохраняем нижнюю границу коллайдера неизменной
            float lowerBound = _initialColliderOffset.y - _initialColliderSize.y / 2f;

            // Новое смещение по Y
            float newOffsetY = lowerBound + newHeight / 2f;

            // Применяем изменения
            Vector2 newSize = _collider.size;
            newSize.y = newHeight;
            _collider.size = newSize;

            Vector2 newOffset = _collider.offset;
            newOffset.y = newOffsetY;
            _collider.offset = newOffset;
        }
    }
}
