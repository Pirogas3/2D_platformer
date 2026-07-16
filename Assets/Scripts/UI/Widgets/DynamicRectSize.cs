using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Widgets
{
    /// <summary>
    /// Динамически подстраивает размер (Width/Height) данного RectTransform
    /// под размер целевого RectTransform с дополнительными отступами.
    /// Работает в редакторе (без Play Mode).
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class DynamicRectSize : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private RectTransform _target;

        [Header("Axes")]
        [SerializeField] private bool _matchWidth = false;
        [SerializeField] private bool _matchHeight = true;

        [Header("Padding")]
        [SerializeField] private float _paddingX = 0f;
        [SerializeField] private float _paddingY = 0f;

        private RectTransform _rectTransform;
        private Vector2 _lastTargetSize;
        private Vector2 _lastPadding;
        private bool _lastMatchWidth;
        private bool _lastMatchHeight;
        private bool _isInitialized = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            //// Принудительно пересчитываем Layout целевого объекта, чтобы он гарантированно имел финальный размер
            if (_target != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_target);

            UpdateSize();
            _isInitialized = true;
        }

        private void Update()
        {
            // В редакторе обновляем постоянно, чтобы видеть изменения в реальном времени
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckAndUpdate();
                return;
            }
#endif

            // В рантайме обновляем только при изменении параметров, но только после первой инициализации
            if (_isInitialized)
                CheckAndUpdate();
        }

        private void CheckAndUpdate()
        {
            if (_target == null) return;

            Vector2 targetSize = _target.rect.size;
            bool needUpdate = false;

            if (targetSize != _lastTargetSize)
                needUpdate = true;
            if (_paddingX != _lastPadding.x || _paddingY != _lastPadding.y)
                needUpdate = true;
            if (_matchWidth != _lastMatchWidth || _matchHeight != _lastMatchHeight)
                needUpdate = true;

            if (needUpdate)
            {
                UpdateSize();
                _lastTargetSize = targetSize;
                _lastPadding = new Vector2(_paddingX, _paddingY);
                _lastMatchWidth = _matchWidth;
                _lastMatchHeight = _matchHeight;
            }
        }

        private void OnValidate()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            UpdateSize();
        }

        [ContextMenu("Update Size")]
        public void UpdateSize()
        {
            if (_rectTransform == null || _target == null)
                return;

            float newWidth = _rectTransform.rect.width;
            float newHeight = _rectTransform.rect.height;

            if (_matchWidth)
                newWidth = _target.rect.width + _paddingX;

            if (_matchHeight)
                newHeight = _target.rect.height + _paddingY;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        public void SetTarget(RectTransform target)
        {
            _target = target;
            UpdateSize();
        }
    }
}
