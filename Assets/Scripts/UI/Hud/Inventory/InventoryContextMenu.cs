using Assets.Scripts.Creatures;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using Assets.Scripts.UI.Hud.QucikInventory;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hud.Inventory
{
    public class InventoryContextMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _dropButton;
        [SerializeField] private Button _clearSlotButton;
        [SerializeField] public RectTransform PanelRect;

        private InventoryItemData _itemData;
        private ItemDef _itemDef;
        private object _source; // InventoryItemCell или QuickInventorySlot
        private int _slotIndex;
        private InventoryWindowController _inventoryController;
        private QuickInventoryController _quickController;

        public void Setup(InventoryItemData itemData, object source, int slotIndex,
                          InventoryWindowController inventoryController,
                          QuickInventoryController quickController)
        {
            _itemData = itemData;
            _source = source;
            _slotIndex = slotIndex;
            _inventoryController = inventoryController;
            _quickController = quickController;

            if (itemData == null)
            {
                CloseMenu();
                return;
            }

            _itemDef = DefsFacade.Instance.Items.Get(itemData.Id);
            if (_itemDef.IsVoid)
            {
                CloseMenu();
                return;
            }

            // Настройка видимости кнопок
            bool canUse = _itemDef.Category == ItemCategory.Food ||
                          _itemDef.Category == ItemCategory.Potion ||
                          _itemDef.IsContainer;
            _useButton.gameObject.SetActive(canUse);

            _dropButton.gameObject.SetActive(true);

            bool isQuickSlot = source is QuickInventorySlot;
            _clearSlotButton.gameObject.SetActive(isQuickSlot);

            // Подписка на кнопки
            _useButton.onClick.AddListener(OnUse);
            _dropButton.onClick.AddListener(OnDrop);
            _clearSlotButton.onClick.AddListener(OnClearSlot);
        }

        private void OnUse()
        {
            var hero = FindObjectOfType<Hero>();
            if (hero != null)
                hero.UseItem(_itemData.Id);
            CloseMenu();
        }

        private void OnDrop()
        {
            var session = FindObjectOfType<GameSession>();
            if (session != null)
            {
                session.PlayerData.Inventory.Remove(_itemData.Id, _itemData.Value);
            }
            if (_source is QuickInventorySlot quickSlot && _quickController != null)
            {
                _quickController.ClearSlot(quickSlot.GetSlotIndex());
            }
            CloseMenu();
        }

        private void OnClearSlot()
        {
            if (_source is QuickInventorySlot quickSlot && _quickController != null)
            {
                _quickController.ClearSlot(quickSlot.GetSlotIndex());
            }
            CloseMenu();
        }

        public void CloseMenu()
        {
            Destroy(gameObject);
        }
    }
}
