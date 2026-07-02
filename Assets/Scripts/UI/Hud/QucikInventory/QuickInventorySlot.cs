using Assets.Scripts.Creatures;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Hud.QucikInventory
{
    public class QuickInventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _countText;
        [SerializeField] private Image _selectionImage; // рамка выделения (активный слот)
        [SerializeField] private Image _hoverOverlay;
        [SerializeField] private float _hoverAlpha = 0.5f;

        private InventoryItemData _itemData;
        private ItemDef _itemDef;
        private int _slotIndex;
        private QuickInventoryController _controller;

        public void Initialize(int slotIndex, QuickInventoryController controller)
        {
            _slotIndex = slotIndex;
            _controller = controller;
        }

        public void Setup(InventoryItemData itemData)
        {
            if (itemData == null)
            {
                Clear();
                return;
            }

            _itemData = itemData;
            _itemDef = DefsFacade.Instance.Items.Get(itemData.Id);
            if (_itemDef.IsVoid)
            {
                Clear();
                return;
            }

            _icon.sprite = _itemDef.Icon;
            _icon.enabled = true;

            if (_itemData.Value > 1)
            {
                _countText.text = _itemData.Value.ToString();
                _countText.gameObject.SetActive(true);
            }
            else
            {
                _countText.gameObject.SetActive(false);
            }

            // Сброс выделения и подсветки
            SetSelected(false);
            if (_hoverOverlay != null)
                _hoverOverlay.gameObject.SetActive(false);
        }

        public void Clear()
        {
            _icon.sprite = null;
            _icon.enabled = false;
            _countText.gameObject.SetActive(false);
            _itemData = null;
            _itemDef = null;
            SetSelected(false);
            if (_hoverOverlay != null)
                _hoverOverlay.gameObject.SetActive(false);
        }

        public void SetSelected(bool selected)
        {
            if (_selectionImage != null)
                _selectionImage.gameObject.SetActive(selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_itemData == null) return;
            var hero = FindObjectOfType<Hero>();
            if (hero != null)
                hero.UseItem(_itemData.Id);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_hoverOverlay == null) return;
            // Включаем затемнение при наведении
            Color c = _hoverOverlay.color;
            c.a = _hoverAlpha;
            _hoverOverlay.color = c;
            _hoverOverlay.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_hoverOverlay != null)
                _hoverOverlay.gameObject.SetActive(false);
        }

        public int GetSlotIndex() => _slotIndex;
        public InventoryItemData GetItemData() => _itemData;
        public bool IsEmpty => _itemData == null;
    }
}
