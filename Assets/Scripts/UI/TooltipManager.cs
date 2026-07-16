using Assets.Scripts.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private TooltipView _tooltipPrefab;

        private TooltipView _tooltipInstance;
        private bool _isVisible = false;

        public static TooltipManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Создаём экземпляр тултипа сразу и выключаем
            _tooltipInstance = Instantiate(_tooltipPrefab, transform);
            _tooltipInstance.gameObject.SetActive(false);
        }

        public void ShowTooltip(Vector2 position, string header, string description, string cost = "")
        {
            if (_tooltipInstance == null) return;

            // Заполняем данные
            _tooltipInstance.SetData(header, description, cost);

            // Принудительно пересчитываем Layout, чтобы получить актуальный размер
            RectTransform rect = _tooltipInstance.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            // Размер тултипа
            Vector2 size = rect.rect.size;

            // Базовое смещение (вправо-вниз)
            Vector2 offset = new Vector2(15, -15);
            Vector2 finalPos = position + offset;

            // Корректировка по границам экрана
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // По горизонтали: если тултип выходит за правый край – показать слева
            if (finalPos.x + size.x > screenWidth)
                finalPos.x = position.x - size.x - 15;

            // По горизонтали: если выходит за левый край – прижать к левому краю
            if (finalPos.x < 0)
                finalPos.x = 0;

            // По вертикали: если тултип выходит за нижний край – показать выше
            if (finalPos.y < 0)
                finalPos.y = position.y + 15;

            // По вертикали: если выходит за верхний край – прижать к верхнему краю
            if (finalPos.y + size.y > screenHeight)
                finalPos.y = screenHeight - size.y;

            _tooltipInstance.SetPosition(finalPos);
            _tooltipInstance.gameObject.SetActive(true);
            _isVisible = true;
        }

        public void HideTooltip()
        {
            if (_tooltipInstance == null || !_isVisible) return;
            _tooltipInstance.gameObject.SetActive(false);
            _isVisible = false;
        }

        public bool IsVisible => _isVisible;
    }
}
