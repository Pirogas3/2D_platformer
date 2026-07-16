using SheetXExample;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Widgets
{
    /// <summary>
    /// Базовый триггер для тултипа с ручным указанием ключей локализации.
    /// </summary>
    public class BaseTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Localization Keys")]
        [SerializeField] private string _headerKey;
        [SerializeField] private string _descriptionKey;
        [SerializeField] private string _costKey;

        [Header("Settings")]
        [SerializeField] private float _delay = 0.3f;

        private float _hoverStartTime;
        private bool _isHovering = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _hoverStartTime = Time.unscaledTime;
            // Отменяем предыдущий вызов, если он был
            CancelInvoke(nameof(ShowTooltip));
            Invoke(nameof(ShowTooltip), _delay);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            CancelInvoke(nameof(ShowTooltip));
            if (TooltipManager.Instance != null)
                TooltipManager.Instance.HideTooltip();
        }

        private void ShowTooltip()
        {
            if (!_isHovering) return;

            string header = LocalizationUI.Get(_headerKey).ToString();
            string description = LocalizationUI.Get(_descriptionKey).ToString();
            string cost = LocalizationUI.Get(_costKey).ToString();

            if (TooltipManager.Instance != null)
            {
                TooltipManager.Instance.ShowTooltip(
                    Input.mousePosition,
                    header,
                    description,
                    cost
                );
            }
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(ShowTooltip));
            if (TooltipManager.Instance != null)
                TooltipManager.Instance.HideTooltip();
        }
    }
}
