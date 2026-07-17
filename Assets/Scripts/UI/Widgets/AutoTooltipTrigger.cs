using Assets.Scripts.UI.Hud.Inventory;
using SheetXExample;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Widgets
{
    public class AutoTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField] private float _delay = 0.3f;
        [SerializeField] private bool _showCost = true;

        private float _hoverStartTime;
        private bool _isHovering = false;

        private InventoryItemCell _itemCell;
        private PerkSlot _perkSlot;
        private string _id;

        private void Awake()
        {
            _itemCell = GetComponent<InventoryItemCell>();
            _perkSlot = GetComponent<PerkSlot>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!TryGetId()) return; // если не удалось получить ID, не показываем

            _isHovering = true;
            _hoverStartTime = Time.unscaledTime;
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

        private bool TryGetId()
        {
            if (!string.IsNullOrEmpty(_id)) return true;

            // Пытаемся получить ID из ячейки инвентаря
            if (_itemCell != null)
            {
                var itemData = _itemCell.GetItemData();
                if (itemData != null)
                {
                    _id = itemData.Id;
                    return true;
                }
            }

            // Пытаемся получить ID из слота перка
            if (_perkSlot != null)
            {
                _id = _perkSlot.GetPerkId();
                if (!string.IsNullOrEmpty(_id))
                    return true;
            }

            return false;
        }

        private void ShowTooltip()
        {
            if (!_isHovering || string.IsNullOrEmpty(_id)) return;

            string headerKey = $"{_id}_Header";
            string descriptionKey = $"{_id}_Description";
            string costKey = $"{_id}_Cost";

            string header = LocalizationUI.Get(headerKey).ToString();
            string description = LocalizationUI.Get(descriptionKey).ToString();
            string cost = _showCost ? LocalizationUI.Get(costKey).ToString() : "";

            if (string.IsNullOrEmpty(header))
                header = _id; // fallback, показываем название как id предмета, если не заполнена локализация

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
