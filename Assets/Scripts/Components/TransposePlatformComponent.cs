using UnityEngine;

namespace Scripts.Components
{
    public class TransposePlatformComponent : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _speed; //модификатор скорости перемещения
        [SerializeField] private Vector2 _moveOffset; // насколько сместиться по X и Y

        [Header("Runtime")]
        [SerializeField] private bool _isMoving = false; //изначально двигается или стоит

        private Vector2 _startPosition;
        private Vector2 _targetPosition;
        private bool _returning = false;

        private void Start()
        {
            _startPosition = transform.position;
            _targetPosition = _startPosition + _moveOffset;
        }

        private void FixedUpdate()
        {
            if (!_isMoving) return;

            // Определяем текущую целевую позицию
            Vector2 currentTarget = _returning ? _startPosition : _targetPosition;

            // Плавно двигаемся к цели
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, _speed * Time.deltaTime);

            // Проверяем, достигли ли цели
            if (Vector2.Distance(transform.position, currentTarget) < 0.01f)
            {
                if (_returning)
                {
                    // Закончили полный цикл туда-обратно и полностью остановились
                    _isMoving = false;
                    _returning = false;
                }
                else
                {
                    // Достигли конечной точки, начинаем возврат
                    _returning = true;
                }
            }
        }

        // Публичный метод для запуска перемещения/остановки, если уже двигается
        public void StartOrStopMoving()
        {
            if (_isMoving)
            {
                StopMoving();
                return;
            }
            _isMoving = true;
            _returning = false;
        }

        // Остановить перемещение
        public void StopMoving()
        {
            _isMoving = false;
        }
    }
}
