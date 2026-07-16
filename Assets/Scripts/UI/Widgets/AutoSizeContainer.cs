using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Widgets
{
    /// <summary>
    /// Автоматически подстраивает размер (высоту и/или ширину) контейнера
    /// под суммарный размер всех его дочерних элементов с учётом отступов.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class AutoSizeContainer : MonoBehaviour
    {
        [Header("Axes")]
        [SerializeField] private bool _matchWidth = false;
        [SerializeField] private bool _matchHeight = true;

        [Header("Padding")]
        [SerializeField] private float _paddingTop = 0f;
        [SerializeField] private float _paddingBottom = 0f;
        [SerializeField] private float _paddingLeft = 0f;
        [SerializeField] private float _paddingRight = 0f;

        [Header("Spacing")]
        [SerializeField] private float _spacing = 0f; // отступ между детьми

        [Header("Child Alignment")]
        [SerializeField] private bool _stretchChildrenWidth = true; // растягивать детей по ширине

        private RectTransform _rectTransform;
        private Vector2 _lastSize;
        private float _lastSpacing;
        private Vector4 _lastPadding;
        private bool _lastMatchWidth;
        private bool _lastMatchHeight;
        private bool _lastStretch;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            Rebuild();
        }

        private void OnEnable()
        {
            Rebuild();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckAndRebuild();
                return;
            }
#endif

            CheckAndRebuild();
        }

        private void CheckAndRebuild()
        {
            if (_rectTransform == null) return;

            bool needRebuild = false;

            // Проверяем изменения параметров
            if (_matchWidth != _lastMatchWidth) needRebuild = true;
            if (_matchHeight != _lastMatchHeight) needRebuild = true;
            if (_spacing != _lastSpacing) needRebuild = true;
            if (_paddingTop != _lastPadding.x || _paddingBottom != _lastPadding.y ||
                _paddingLeft != _lastPadding.z || _paddingRight != _lastPadding.w)
                needRebuild = true;
            if (_stretchChildrenWidth != _lastStretch) needRebuild = true;

            // Если дети изменили свои размеры, тоже нужно перестроить
            // Для этого мы можем сравнить суммарную высоту/ширину детей с предыдущими значениями,
            // но проще пересчитывать каждый раз при изменении параметров.
            // Однако для производительности лучше проверять по необходимости.

            // Можно также подписаться на изменение размеров детей, но это сложнее.
            // Будем считать, что Rebuild вызывается при изменении параметров.
            if (needRebuild)
            {
                Rebuild();
                _lastMatchWidth = _matchWidth;
                _lastMatchHeight = _matchHeight;
                _lastSpacing = _spacing;
                _lastPadding = new Vector4(_paddingTop, _paddingBottom, _paddingLeft, _paddingRight);
                _lastStretch = _stretchChildrenWidth;
            }
        }

        [ContextMenu("Rebuild")]
        public void Rebuild()
        {
            if (_rectTransform == null || transform.childCount == 0)
                return;

            float totalWidth = 0f;
            float totalHeight = 0f;
            int childCount = transform.childCount;

            // Сначала пересчитываем Layout детей, чтобы их размеры были актуальны
            foreach (Transform child in transform)
            {
                var childRect = child as RectTransform;
                if (childRect == null) continue;
                LayoutRebuilder.ForceRebuildLayoutImmediate(childRect);
            }

            // Суммируем размеры детей (с учетом их предпочтительных размеров)
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i) as RectTransform;
                if (child == null) continue;

                // Получаем предпочтительные размеры (если есть ContentSizeFitter или другие компоненты)
                float preferredWidth = LayoutUtility.GetPreferredWidth(child);
                float preferredHeight = LayoutUtility.GetPreferredHeight(child);

                // Если предпочтительные размеры равны 0, используем текущие размеры
                if (preferredWidth <= 0) preferredWidth = child.rect.width;
                if (preferredHeight <= 0) preferredHeight = child.rect.height;

                totalWidth = Mathf.Max(totalWidth, preferredWidth);
                totalHeight += preferredHeight;

                // Добавляем отступы между детьми (кроме последнего)
                if (i < childCount - 1)
                    totalHeight += _spacing;
            }

            // Добавляем padding
            totalWidth += _paddingLeft + _paddingRight;
            totalHeight += _paddingTop + _paddingBottom;

            // Применяем размеры
            float newWidth = _rectTransform.rect.width;
            float newHeight = _rectTransform.rect.height;

            if (_matchWidth)
                newWidth = totalWidth;
            if (_matchHeight)
                newHeight = totalHeight;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

            // Растягиваем детей по ширине, если нужно
            if (_stretchChildrenWidth)
            {
                float childWidth = newWidth - _paddingLeft - _paddingRight;
                foreach (Transform child in transform)
                {
                    var childRect = child as RectTransform;
                    if (childRect == null) continue;
                    childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childWidth);
                }
            }
        }

        private void OnValidate()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            Rebuild();
        }
    }
}
