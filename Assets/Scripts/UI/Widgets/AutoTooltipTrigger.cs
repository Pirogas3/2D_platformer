using Assets.Scripts.Model.Definitions;
using Assets.Scripts.UI.Hud.Inventory;
using SheetXExample;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Widgets
{
    public class AutoTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField]
        private float _delay = 0.3f;
        [SerializeField]
        private bool _showCost = true;

        private string _id;
        private float _hoverStartTime;
        private bool _isHovering = false;

        private void Awake()
        {
            // Пытаемся определить, что за объект
            var itemCell = GetComponent<InventoryItemCell>();
            if (itemCell != null)
            {
                // Предмет инвентаря
                var itemData = itemCell.GetItemData();
                if (itemData != null)
                {
                    _id = itemData.Id;
                    var itemDef = DefsFacade.Instance.Items.Get(_id);
                    if (itemDef != null && !itemDef.IsVoid)
                    {
                        //
                    }
                }
                return;
            }

            var perkSlot = GetComponent<PerkSlot>();
            if (perkSlot != null)
            {
                // Перк
                _id = perkSlot.GetPerkId(); // нужно добавить публичный метод GetPerkId в PerkSlot
                var perkDef = DefsFacade.Instance.Perks.Get(_id);
                if (perkDef != null && !perkDef.IsVoid)
                {
                    //
                }
                return;
            }

            // Если ничего не найдено, отключаем себя
            Debug.LogWarning($"AutoTooltipTrigger on {gameObject.name} couldn't find any supported component.");
            enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(_id)) return;

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

        private void ShowTooltip()
        {
            if (!_isHovering || string.IsNullOrEmpty(_id)) return;

            string headerKey = $"{_id}_Header";
            string descriptionKey = $"{_id}_Description";
            string costKey = $"{_id}_Cost";

            string header = LocalizationUI.Get(headerKey).ToString();
            string description = LocalizationUI.Get(descriptionKey).ToString();
            string cost = _showCost ? LocalizationUI.Get(costKey).ToString() : "";

            // Если какой-то ключ не найден, можно использовать fallback
            if (string.IsNullOrEmpty(header))
                header = _id; // или название из Defs

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
