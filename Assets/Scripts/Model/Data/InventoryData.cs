using Assets.Scripts.Model.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class InventoryData
    {
        [SerializeField] private List<InventoryItemData> _items = new List<InventoryItemData>();
        public IReadOnlyList<InventoryItemData> Items => _items;

        public event Action OnChanged;

        // --- Базовые операции (без контейнеров) ---

        public void Add(string id, int amount = 1)
        {
            if (amount <= 0) return;
            var def = DefsFacade.Instance.Items.Get(id);
            if (def.IsVoid) return;

            // Контейнеры добавляются через отдельный метод AddContainer
            if (def.IsContainer)
            {
                Debug.LogWarning("Используйте AddContainer для добавления контейнеров.");
                return;
            }

            // Стакинг
            var existing = _items.Find(i => i.Id == id && !i.IsContainer);
            if (existing != null)
            {
                int space = def.MaxStack - existing.Value;
                int add = Mathf.Min(amount, space);
                existing.Value += add;
                amount -= add;
            }

            while (amount > 0 && def.MaxStack > 0)
            {
                int stack = Mathf.Min(amount, def.MaxStack);
                _items.Add(new InventoryItemData(id, stack));
                amount -= stack;
            }

            OnChanged?.Invoke();
        }

        public void Remove(string id, int amount = 1)
        {
            if (amount <= 0) return;
            var item = GetItem(id);
            while (item != null && amount > 0)
            {
                if (item.Value <= amount)
                {
                    amount -= item.Value;
                    _items.Remove(item);
                    item = GetItem(id);
                }
                else
                {
                    item.Value -= amount;
                    amount = 0;
                }
            }
            OnChanged?.Invoke();
        }

        public void RemoveAt(int index, int amount = 1)
        {
            if (index < 0 || index >= _items.Count) return;
            var item = _items[index];

            // Контейнер удаляется целиком
            if (item.IsContainer)
            {
                _items.RemoveAt(index);
                OnChanged?.Invoke();
                return;
            }

            if (amount >= item.Value)
                _items.RemoveAt(index);
            else
                item.Value -= amount;
            OnChanged?.Invoke();
        }

        public InventoryItemData GetItem(string id) => _items.Find(i => i.Id == id);
        public int Count(string id) => _items.Where(i => i.Id == id).Sum(i => i.Value);

        // --- Работа с контейнерами (требуют registry) ---

        // Добавить контейнер в инвентарь
        public void AddContainer(string itemId, string containerId, InventoryRegistry registry)
        {
            var def = DefsFacade.Instance.Items.Get(itemId);
            if (def.IsVoid || !def.IsContainer) return;

            // Создаём запись в реестре
            registry.CreateContainer(containerId, itemId);

            // Добавляем слот
            var slot = new InventoryItemData(itemId, containerId, 1);
            _items.Add(slot);
            OnChanged?.Invoke();
        }

        // Проверить, можно ли положить предмет в контейнер
        public bool CanPlaceIntoContainer(string containerId, InventoryItemData item, InventoryRegistry registry)
        {
            var entry = registry.GetEntry(containerId);
            if (entry == null) return false;

            var containerDef = DefsFacade.Instance.Items.Get(entry.ItemId);
            if (containerDef.IsVoid || !containerDef.IsContainer) return false;

            var itemDef = DefsFacade.Instance.Items.Get(item.Id);
            return containerDef.ContainerConfig.Accepts(itemDef.Category);
        }

        // Переместить предмет из sourceInventory в контейнер с containerId
        public void PlaceIntoContainer(InventoryData sourceInventory, int sourceSlotIndex, string targetContainerId, int amount, InventoryRegistry registry)
        {
            if (sourceSlotIndex < 0 || sourceSlotIndex >= sourceInventory._items.Count) return;
            var sourceSlot = sourceInventory._items[sourceSlotIndex];
            if (sourceSlot == null || sourceSlot.IsContainer) return; // запрещаем класть контейнер в контейнер

            if (!CanPlaceIntoContainer(targetContainerId, sourceSlot, registry)) return;

            var targetInventory = registry.GetContainer(targetContainerId);
            if (targetInventory == null) return;

            // Переносим
            targetInventory.Add(sourceSlot.Id, amount);
            sourceInventory.RemoveAt(sourceSlotIndex, amount);
            OnChanged?.Invoke();
            targetInventory.OnChanged?.Invoke();
        }

        // --- Перемещение/обмен между инвентарями ---

        public void Swap(int indexA, int indexB)
        {
            if (indexA == indexB) return;
            if (indexA < 0 || indexA >= _items.Count || indexB < 0 || indexB >= _items.Count) return;
            (_items[indexA], _items[indexB]) = (_items[indexB], _items[indexA]);
            OnChanged?.Invoke();
        }

        public void MoveTo(InventoryData target, int fromIndex, int targetIndex = -1)
        {
            if (fromIndex < 0 || fromIndex >= _items.Count) return;
            var item = _items[fromIndex];

            // Если цель — контейнер (требуется registry, но здесь его нет, поэтому этот случай не обрабатываем)
            // Лучше использовать PlaceIntoContainer для контейнеров.
            if (targetIndex >= 0 && targetIndex < target._items.Count && target._items[targetIndex].IsContainer)
            {
                Debug.LogWarning("Для перемещения в контейнер используйте PlaceIntoContainer.");
                return;
            }

            if (targetIndex >= 0 && targetIndex < target._items.Count)
            {
                var targetItem = target._items[targetIndex];
                if (targetItem.Id == item.Id && !targetItem.IsContainer && !item.IsContainer)
                {
                    int space = DefsFacade.Instance.Items.Get(item.Id).MaxStack - targetItem.Value;
                    if (space > 0)
                    {
                        int transfer = Mathf.Min(item.Value, space);
                        targetItem.Value += transfer;
                        item.Value -= transfer;
                        if (item.Value <= 0) _items.RemoveAt(fromIndex);
                        if (targetItem.Value <= 0) target._items.RemoveAt(targetIndex);
                        OnChanged?.Invoke();
                        target.OnChanged?.Invoke();
                        return;
                    }
                }
                // Обмен местами, если разные Id или стакинг невозможен
                (_items[fromIndex], target._items[targetIndex]) = (target._items[targetIndex], _items[fromIndex]);
            }
            else
            {
                // Перенос в конец целевого инвентаря
                target._items.Add(item);
                _items.RemoveAt(fromIndex);
            }
            OnChanged?.Invoke();
            target.OnChanged?.Invoke();
        }

        // --- Вес ---

        public float GetTotalWeight(InventoryRegistry registry)
        {
            float total = 0f;
            var weightMods = new Dictionary<ItemCategory, float>();

            // Собираем модификаторы от контейнеров на этом уровне
            foreach (var slot in _items)
            {
                if (!slot.IsContainer) continue;
                var entry = registry.GetEntry(slot.ContainerId);
                if (entry == null) continue;
                var def = DefsFacade.Instance.Items.Get(entry.ItemId);
                if (def.IsVoid || !def.IsContainer) continue;
                var config = def.ContainerConfig;
                if (config == null || !config.IsValid()) continue;

                foreach (var cat in config.AcceptedCategories)
                {
                    if (!weightMods.ContainsKey(cat) || weightMods[cat] > config.WeightMultiplier)
                        weightMods[cat] = config.WeightMultiplier;
                }
            }

            // Считаем вес всех предметов
            foreach (var slot in _items)
            {
                var def = DefsFacade.Instance.Items.Get(slot.Id);
                if (def.IsVoid) continue;

                float w = def.Weight * slot.Value;
                if (weightMods.TryGetValue(def.Category, out float mod))
                    w *= mod;

                // Если это контейнер — добавляем вес его содержимого
                if (slot.IsContainer && registry != null)
                {
                    var containerInv = registry.GetContainer(slot.ContainerId);
                    if (containerInv != null)
                    {
                        w += containerInv.GetTotalWeight(registry);
                    }
                }

                total += w;
            }

            return total;
        }

        // --- Сортировка (заглушки) ---
        public void SortByWeight(bool ascending = true) { /* реализация */ }
        public void SortByValue(bool ascending = true) { /* реализация */ }
    }
}
