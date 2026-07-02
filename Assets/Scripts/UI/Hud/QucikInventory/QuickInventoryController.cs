using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI.Hud.QucikInventory
{
    public class QuickInventoryController : MonoBehaviour
    {
        [Header("Slots")]
        [SerializeField] private QuickInventorySlot[] _slots; // 6 слотов (в инспекторе назначить)

        [Header("Input")]
        [SerializeField] private InputActionReference[] _slotKeyActions; // 6 действий для клавиш 1-6

        private GameSession _session;
        private InventoryData _inventory;
        private InventoryItemData[] _quickItems = new InventoryItemData[6]; // данные быстрых слотов
        private int _selectedIndex = 0;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            if (_session == null)
            {
                Debug.LogError("GameSession не найден!");
                return;
            }
            _inventory = _session.PlayerData.Inventory;
            _inventory.OnChanged += OnInventoryChanged;

            // Инициализация слотов
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                    _slots[i].Initialize(i, this);
            }

            // Подписка на действия клавиш
            for (int i = 0; i < _slotKeyActions.Length && i < 6; i++)
            {
                if (_slotKeyActions[i] != null)
                    _slotKeyActions[i].action.performed += ctx => SelectSlot(i);
            }

            RefreshUI();
            SelectSlot(0); // по умолчанию первый слот активен
        }

        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.OnChanged -= OnInventoryChanged;

            for (int i = 0; i < _slotKeyActions.Length && i < 6; i++)
            {
                if (_slotKeyActions[i] != null)
                    _slotKeyActions[i].action.performed -= ctx => SelectSlot(i);
            }
        }

        private void Update()
        {
            // Обработка скролла мыши (только если окно инвентаря не активно)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0 && !IsInventoryOpen())
            {
                int delta = scroll > 0 ? -1 : 1;
                int newIndex = Mathf.Clamp(_selectedIndex + delta, 0, 5);
                if (newIndex != _selectedIndex)
                    SelectSlot(newIndex);
            }
        }

        private bool IsInventoryOpen()
        {
            // Можно проверить, активно ли окно инвентаря (например, через InventoryWindowController)
            // Для простоты оставим заглушку: всегда разрешаем скролл (пока не реализовано)
            // Позже можно добавить проверку.
            return false;
        }

        private void OnInventoryChanged()
        {
            // Проверяем, не изменилось ли количество предметов, на которые есть ссылки
            for (int i = 0; i < _quickItems.Length; i++)
            {
                if (_quickItems[i] != null)
                {
                    int currentCount = _inventory.Count(_quickItems[i].Id);
                    if (currentCount == 0)
                    {
                        // Предмет полностью удалён из инвентаря – очищаем слот
                        _quickItems[i] = null;
                    }
                    else if (currentCount != _quickItems[i].Value)
                    {
                        // Обновляем количество
                        _quickItems[i].Value = currentCount;
                    }
                }
            }
            RefreshUI();
        }

        public void RefreshUI()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null) continue;
                if (_quickItems[i] != null)
                    _slots[i].Setup(_quickItems[i]);
                else
                    _slots[i].Clear();
            }
            // После обновления восстанавливаем выделение
            SelectSlot(_selectedIndex);
        }

        public void SelectSlot(int index)
        {
            if (index < 0 || index >= _slots.Length) return;
            _selectedIndex = index;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                    _slots[i].SetSelected(i == index);
            }
        }

        // Метод для добавления предмета в слот (вызывается при перетаскивании из инвентаря)
        public bool TryAssignItem(int slotIndex, InventoryItemData itemData)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) return false;
            if (_quickItems[slotIndex] != null) return false; // слот занят

            // Копируем данные (создаём новый объект, чтобы не ссылаться на оригинал)
            _quickItems[slotIndex] = new InventoryItemData(itemData.Id, itemData.Value);
            RefreshUI();
            SelectSlot(_selectedIndex); // сохраняем выделение
            return true;
        }

        // Метод для очистки слота (можно вызвать по ПКМ, но пока не реализовано)
        public void ClearSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) return;
            _quickItems[slotIndex] = null;
            RefreshUI();
            SelectSlot(_selectedIndex);
        }

        // Метод для получения данных слота (для перетаскивания из быстрого слота обратно в инвентарь – пока не используется)
        public InventoryItemData GetSlotData(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) return null;
            return _quickItems[slotIndex];
        }
    }
}
