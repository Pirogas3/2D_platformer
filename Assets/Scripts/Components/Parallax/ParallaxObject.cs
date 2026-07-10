using UnityEngine;

namespace Assets.Scripts.Components.Parallax
{
    public class ParallaxObject : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform _cameraTransform; // опционально

        private Vector3 _startPosition;
        private Vector3 _cameraStartPosition;

        private void Start()
        {
            if (_cameraTransform == null)
                _cameraTransform = Camera.main.transform;

            _startPosition = transform.position;
            _cameraStartPosition = _cameraTransform.position;
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            // Смещение камеры по X от начальной позиции
            float cameraDeltaX = _cameraTransform.position.x - _cameraStartPosition.x;

            // Двигаем объект только по X, сохраняя Y и Z
            transform.position = new Vector3(
                _startPosition.x + cameraDeltaX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}
