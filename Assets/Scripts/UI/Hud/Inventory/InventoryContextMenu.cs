using Assets.Scripts.Creatures;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using Assets.Scripts.UI.Hud.QucikInventory;
using SheetXExample;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hud.Inventory
{
    public class InventoryContextMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _equipUnquipButton;
        [SerializeField] private Text _equipUnquipText;
        [SerializeField] private Button _dropButton;
        [SerializeField] private Button _clearSlotButton;
        [SerializeField] public RectTransform PanelRect;

        private InventoryItemData _itemData;
        private ItemDef _itemDef;
        private object _source; // InventoryItemCell или QuickInventorySlot
        private int _slotIndex;
        private IInventoryController _inventoryController;
        private QuickInventoryController _quickController;

        private void OnDestroy()
        {
            _useButton.onClick.RemoveListener(OnUse);
            _equipUnquipButton.onClick.RemoveListener(OnUse);
            _dropButton.onClick.RemoveListener(OnDrop);
            _clearSlotButton.onClick.RemoveListener(OnClearSlot);
        }

        public void Setup(InventoryItemData itemData, object source, int slotIndex,
                  IInventoryController inventoryController,
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

            bool canUse = _itemDef.Category == ItemCategory.Food ||
                          _itemDef.Category == ItemCategory.Potion ||
                          _itemDef.IsContainer;
            _useButton.gameObject.SetActive(canUse);

            bool isQuickSlot = source is QuickInventorySlot;
            _dropButton.gameObject.SetActive(!isQuickSlot);
            _clearSlotButton.gameObject.SetActive(isQuickSlot);

            bool canEquip = (_itemDef.Category == ItemCategory.Weapon || _itemDef.Category == ItemCategory.Armor)
                && !(_inventoryController is ChestInventoryController);
            _equipUnquipButton.gameObject.SetActive(canEquip);
            if (canEquip)
            {
                var session = GameSession.Instance;
                bool isEquipped = (_itemData.Id == session.PlayerData.WeaponItemId);
                string key = isEquipped ? "HUD_Unequip" : "HUD_Equip";
                _equipUnquipText.text = LocalizationUI.Get(key).ToString();
            }

            _useButton.onClick.AddListener(OnUse);
            _equipUnquipButton.onClick.AddListener(OnUse);
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
            var session = GameSession.Instance;
            if (session != null)
            {
                session.PlayerData.Inventory.Remove(_itemData.Id, _itemData.Value);
            }
            if (_source is QuickInventorySlot quickSlot && _quickController != null)
            {
                _quickController.ClearSlot(quickSlot.GetSlotIndex());
            }
            _inventoryController?.RefreshUI();
            CloseMenu();
        }

        private void OnClearSlot()
        {
            if (_source is QuickInventorySlot quickSlot && _quickController != null)
            {
                _quickController.ClearSlot(quickSlot.GetSlotIndex());
            }
            _inventoryController?.RefreshUI();
            CloseMenu();
        }

        public void CloseMenu()
        {
            Destroy(gameObject);
        }
    }
}
