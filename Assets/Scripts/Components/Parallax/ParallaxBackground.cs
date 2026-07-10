using UnityEngine;

namespace Assets.Scripts.Components.Parallax
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [SerializeField] private float _parallaxSpeed = 0.1f;      // Скорость относительно камеры (по модулю)
        [SerializeField] private float _parallaxDirection = 1f;    // Направление: 1 = вправо, -1 = влево
        [SerializeField] private Transform _cameraTransform;

        [Header("Auto Scroll Settings")]
        [SerializeField] private float _autoScrollSpeed = 0.02f;   // Собственная скорость движения (облака плывут сами)
        [SerializeField] private float _autoScrollDirection = 1f;  // Направление: 1 = вправо, -1 = влево

        private Material _material;
        private Vector2 _offset;

        private void Awake()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
                _material = renderer.material;
            else
                Debug.LogError("SpriteRenderer not found!");

            if (_cameraTransform == null)
                _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (_material == null || _cameraTransform == null) return;

            // Смещение от движения камеры (параллакс)
            float cameraOffsetX = -_cameraTransform.position.x * _parallaxSpeed * _parallaxDirection;

            // Собственное движение (автоскролл)
            float autoOffsetX = Time.time * _autoScrollSpeed * _autoScrollDirection;

            // Общее смещение
            float totalOffsetX = cameraOffsetX + autoOffsetX;

            _material.mainTextureOffset = new Vector2(totalOffsetX, 0);
        }

        /// <summary>
        /// Изменить направление автоскролла (1 = вправо, -1 = влево)
        /// </summary>
        public void SetAutoScrollDirection(float direction)
        {
            _autoScrollDirection = Mathf.Sign(direction);
        }

        /// <summary>
        /// Изменить скорость автоскролла
        /// </summary>
        public void SetAutoScrollSpeed(float speed)
        {
            _autoScrollSpeed = speed;
        }

        private void OnDestroy()
        {
            if (_material != null)
                DestroyImmediate(_material);
        }
    }
}
