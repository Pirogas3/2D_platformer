using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using UnityEngine;

namespace Assets.Scripts.UI.Hud.Inventory
{
    public class ChestInventoryController : MonoBehaviour, IInventoryController
    {
        [Header("UI References")]
        [SerializeField] private GameObject _window;          // всё окно
        [SerializeField] private Transform _content;          // GridLayoutGroup
        [SerializeField] private GameObject _itemPrefab;      // префаб ячейки (можно переиспользовать InventoryItemCell)

        private InventoryData _inventoryData;
        private GameSession _session;
        private bool _isOpen = false;
        private int _dragFromIndex = -1;

        public GameObject Window => _window;
        public bool IsOpen => _isOpen;
        public InventoryData GetInventoryData() => _inventoryData;

        private void Awake()
        {
            _session = GameSession.Instance;
        }

        public void Open(InventoryData inventoryData)
        {
            _inventoryData = inventoryData;
            _inventoryData.OnChanged += RefreshUI;
            RefreshUI();
            _window.SetActive(true);
            _isOpen = true;
        }

        public void Close()
        {
            if (_inventoryData != null)
                _inventoryData.OnChanged -= RefreshUI;
            _window.SetActive(false);
            _isOpen = false;
        }

        public void RefreshUI()
        {
            if (_inventoryData == null) return;

            // Очистка
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            int index = 0;
            foreach (var itemData in _inventoryData.Items)
            {
                var cellGO = Instantiate(_itemPrefab, _content);
                var cell = cellGO.GetComponent<InventoryItemCell>();
                if (cell != null)
                {
                    cell.Initialize(index, this); // передаём себя как контроллер
                    cell.Setup(itemData);
                    index++;
                }
            }
        }

        // --- Drag & Drop внутри сундука ---
        public void OnBeginDrag(int fromIndex)
        {
            _dragFromIndex = fromIndex;
        }

        public void OnEndDrag()
        {
            _dragFromIndex = -1;
        }

        public void OnDrop(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex) return;
            if (_inventoryData == null) return;
            if (fromIndex < 0 || fromIndex >= _inventoryData.Items.Count) return;
            if (toIndex < 0 || toIndex >= _inventoryData.Items.Count) return;

            // Используем Swap для обмена местами
            _inventoryData.Swap(fromIndex, toIndex);
        }

        // --- Перетаскивание извне (из игрока или другого сундука) ---
        public void MoveFromOutside(InventoryData sourceInventory, int fromIndex, int toIndex = -1)
        {
            if (_inventoryData == null || sourceInventory == null) return;
            sourceInventory.MoveTo(_inventoryData, fromIndex, toIndex);
        }

        // --- Взять все ---
        public void MoveAllTo(InventoryData targetInventory)
        {
            if (_inventoryData == null || targetInventory == null) return;
            _inventoryData.MoveAllTo(targetInventory);
        }

        public void TakeAll()
        {
            if (_session == null)
                return;

            MoveAllTo(_session.PlayerData.Inventory);
        }

        private void OnDestroy()
        {
            if (_inventoryData != null)
                _inventoryData.OnChanged -= RefreshUI;
        }
    }
}
