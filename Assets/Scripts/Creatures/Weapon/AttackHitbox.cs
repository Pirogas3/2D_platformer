using Assets.Scripts.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Creatures.Weapon
{
    public enum HitShape
    {
        Circle,      // круг (радиус)
        Rectangle,   // прямоугольник (ширина, высота)
        Cone         // конус (угол, радиус, направление)
    }

    public class AttackHitbox : MonoBehaviour
    {
        [Header("Hit Shape")]
        [SerializeField] private HitShape _shape = HitShape.Circle;

        [Header("Common")]
        [SerializeField] private Vector2 _offset = Vector2.zero; // смещение от позиции объекта
        [SerializeField] private LayerMask _targetLayers = 0;    // какие слои можно атаковать (если не заданы, то ищем по тегу)
        [SerializeField] private bool _useTagInsteadOfLayer = true;
        [SerializeField] private string _targetTag = "Enemy";

        // Параметры для круга
        [Header("Circle parameters")]
        [SerializeField] private float _radius = 1f;

        // Параметры для прямоугольника
        [Header("Rectangle parameters")]
        [SerializeField] private Vector2 _rectangleSize = new Vector2(1f, 1f);

        // Параметры для конуса
        [Header("Cone parameters")]
        [SerializeField] private float _coneRadius = 2f;
        [SerializeField][Range(0, 360)] private float _coneAngle = 90f;

        private DamageComponent _damageComponent;

        // Визуализация в редакторе
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 worldPos = (Vector2)transform.position + _offset;

            switch (_shape)
            {
                case HitShape.Circle:
                    Gizmos.DrawWireSphere(worldPos, _radius);
                    break;
                case HitShape.Rectangle:
                    Gizmos.DrawWireCube(worldPos, _rectangleSize);
                    break;
                case HitShape.Cone:
                    Vector2 coneDirection = transform.right;
                    Vector2 leftDir = Quaternion.Euler(0, 0, _coneAngle / 2) * coneDirection;
                    Vector2 rightLimit = Quaternion.Euler(0, 0, -_coneAngle / 2) * coneDirection;
                    Vector2 arcPoint1 = worldPos + leftDir * _coneRadius;
                    Vector2 arcPoint2 = worldPos + rightLimit * _coneRadius;
                    Gizmos.DrawLine(worldPos, arcPoint1);
                    Gizmos.DrawLine(worldPos, arcPoint2);
                    // Рисуем дугу приближённо
                    int segments = 20;
                    float angleStep = _coneAngle / segments;
                    Vector2 prevPoint = worldPos + rightLimit * _coneRadius;
                    for (int i = 1; i <= segments; i++)
                    {
                        float angle = -_coneAngle / 2 + angleStep * i;
                        Vector2 dir = Quaternion.Euler(0, 0, angle) * coneDirection;
                        Vector2 point = worldPos + dir * _coneRadius;
                        Gizmos.DrawLine(prevPoint, point);
                        prevPoint = point;
                    }
                    break;
            }
        }

        private void Awake()
        {
            // Ищем DamageComponent на этом объекте или на родителе (герое/юните)
            _damageComponent = GetComponent<DamageComponent>();
            if (_damageComponent == null)
                _damageComponent = GetComponentInParent<DamageComponent>();
        }

        public List<GameObject> GetTargets()
        {
            List<GameObject> results = new List<GameObject>();
            Vector2 worldPos = (Vector2)transform.position + _offset;

            Collider2D[] hits = null;

            switch (_shape)
            {
                case HitShape.Circle:
                    hits = Physics2D.OverlapCircleAll(worldPos, _radius);
                    break;
                case HitShape.Rectangle:
                    hits = Physics2D.OverlapBoxAll(worldPos, _rectangleSize, 0);
                    break;
                case HitShape.Cone:
                    hits = Physics2D.OverlapCircleAll(worldPos, _coneRadius);
                    break;
            }

            if (hits == null) return results;

            foreach (var hit in hits)
            {
                GameObject go = hit.gameObject;
                if (_useTagInsteadOfLayer)
                {
                    if (!go.CompareTag(_targetTag)) continue;
                }
                else
                {
                    if (((1 << go.layer) & _targetLayers) == 0) continue;
                }

                // Для конуса дополнительная проверка угла
                if (_shape == HitShape.Cone)
                {
                    float distance = Vector2.Distance(worldPos, hit.transform.position);
                    if (distance < 0.3f)
                    {
                        results.Add(go);
                        continue;
                    }

                    Vector2 dirToTarget = ((Vector2)hit.transform.position - worldPos).normalized;
                    float angle = Vector2.Angle(transform.right, dirToTarget);
                    if (angle <= _coneAngle / 2f)
                    {
                        results.Add(go);
                    }
                }
                else
                {
                    //Debug.Log($"Добавлена цель: {go.name}, тег: {go.tag}");
                    results.Add(go);
                }
            }
            return results;
        }

        /// <summary>
        /// Обычный метод нанесения урона через хитбокс, необязательно, но можно передать конкретный урон который надо нанести
        /// </summary>
        /// <param name="damage">Конкретный переданный урон (необязателен)</param>
        public void Attack(int damage = 0)
        {

            var targets = GetTargets();
            foreach (var target in targets)
            {
                if (_damageComponent != null && damage == 0)
                {
                    _damageComponent.ApplyDamage(target);
                }
                else
                {
                    // если DamageComponent не найден или передан конкретный урон, напрямую бьём по HealthComponent
                    var health = target.GetComponent<HealthComponent>();
                    if (health != null) health.TakeDamage(damage);
                    Debug.Log($"Юнитом: {name} - нанесён урон равный = {_damageComponent}");
                }
            }
        }

        /// <summary>
        /// Метод нанесения урона конкретной переданной цели, также можно передать конкретный урон
        /// </summary>
        /// <param name="target">Конкретная цель для нанесения урона</param>
        /// <param name="damage">Конкретный переданный урон (необязателен)</param>
        public void Attack(GameObject target, int damage = 0)
        {
            if (_damageComponent != null && damage == 0)
            {
                _damageComponent.ApplyDamage(target);
            }
            else
            {
                // если DamageComponent не найден или передан конкретный урон, напрямую бьём по HealthComponent
                var health = target.GetComponent<HealthComponent>();
                if (health != null) health.TakeDamage(damage);
                Debug.Log($"Юнитом: {name} - нанесён урон равный = {_damageComponent}");
            }
        }
    }
}
