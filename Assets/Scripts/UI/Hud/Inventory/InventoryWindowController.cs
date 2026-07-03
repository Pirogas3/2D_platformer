using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hud.Inventory
{
    public class InventoryWindowController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _window; // само окно
        [SerializeField] private Transform _content; // объект с GridLayoutGroup
        [SerializeField] private GameObject _itemPrefab; // префаб ячейки
        [SerializeField] private GameObject _contextMenuPrefab; // префаб контекстного меню
        [SerializeField] private Canvas _dynamicCanvas;

        [Header("Input")]
        [SerializeField] private InputActionReference _toggleAction; // действие для открытия/закрытия

        private GameSession _session;
        private InventoryData _inventory;
        private bool _isOpen = false;
        private int _dragFromIndex = -1;

        public bool IsOpen => _isOpen;

        private void OnEnable()
        {
            if (_toggleAction != null)
                _toggleAction.action.performed += OnTogglePerformed;
        }

        private void OnDisable()
        {
            if (_toggleAction != null)
                _toggleAction.action.performed -= OnTogglePerformed;
        }

        private void Start()
        {
            TryInitialize();
            if (_inventory != null)
                _inventory.OnChanged += RefreshInventoryUI;
            _window.SetActive(false);
            _isOpen = false;

            ContextMenuManager.Initialize(_contextMenuPrefab, _dynamicCanvas);
        }

        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.OnChanged -= RefreshInventoryUI;
        }

        private void TryInitialize()
        {
            if (_session == null)
                _session = FindObjectOfType<GameSession>();
            if (_session != null && _inventory == null)
                _inventory = _session.PlayerData.Inventory;
        }

        private void OnTogglePerformed(InputAction.CallbackContext context)
        {
            ToggleWindow();
        }

        [ContextMenu("Toggle Window")]
        public void ToggleWindow()
        {
            TryInitialize();
            _isOpen = !_isOpen;
            _window.SetActive(_isOpen);
            if (_isOpen) RefreshInventoryUI();
        }

        private void RefreshInventoryUI()
        {
            if (_inventory == null)
            {
                Debug.LogWarning("Инвентарь ещё не инициализирован.");
                return;
            }

            // Очистка
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            int index = 0;
            foreach (var itemData in _inventory.Items)
            {
                var cellGO = Instantiate(_itemPrefab, _content);
                var cell = cellGO.GetComponent<InventoryItemCell>();
                if (cell != null)
                {
                    cell.Initialize(index, this);
                    cell.Setup(itemData);
                    index++;
                }
            }
        }

        public void OnBeginDrag(int fromIndex)
        {
            _dragFromIndex = fromIndex;
        }

        public void OnEndDrag()
        {
            _dragFromIndex = -1;
            // Снять подсветку со всех ячеек (можно реализовать)
        }

        public void OnDrop(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex) return;
            if (fromIndex < 0 || fromIndex >= _inventory.Items.Count) return;
            if (toIndex < 0 || toIndex >= _inventory.Items.Count) return;

            var fromItem = _inventory.Items[fromIndex];
            var toItem = _inventory.Items[toIndex];

            // Если оба null – ничего не делаем
            if (fromItem == null && toItem == null) return;

            // Если один из них null – просто перемещаем (через Swap)
            if (fromItem != null && toItem == null)
            {
                _inventory.Swap(fromIndex, toIndex);
                RefreshInventoryUI();
                return;
            }

            if (fromItem == null && toItem != null) return;

            // Оба не null
            if (fromItem.Id == toItem.Id && !fromItem.IsContainer && !toItem.IsContainer)
            {
                var def = DefsFacade.Instance.Items.Get(fromItem.Id);
                int maxStack = def.MaxStack;
                int spaceInTarget = maxStack - toItem.Value;

                if (spaceInTarget > 0)
                {
                    int amountToMove = Mathf.Min(fromItem.Value, spaceInTarget);
                    toItem.Value += amountToMove;
                    fromItem.Value -= amountToMove;

                    if (fromItem.Value <= 0)
                    {
                        _inventory.RemoveAt(fromIndex);
                    }
                    // UI обновится через OnChanged
                    return;
                }
            }

            // Иначе меняем местами
            _inventory.Swap(fromIndex, toIndex);
            RefreshInventoryUI();
        }
    }
}
