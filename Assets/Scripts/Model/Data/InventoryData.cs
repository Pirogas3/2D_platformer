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

        public event Action OnChanged; // Событие, вызываемое при любом изменении содержимого инвентаря (для обновления UI)

        public void CopyFrom(InventoryData other)
        {
            _items.Clear();
            foreach (var item in other._items)
                _items.Add(item);
        }

        // ------------------------------------------------------------
        // Базовые операции с предметами
        // ------------------------------------------------------------

        /// <summary>
        /// Добавить обычный предмет (не контейнер) в инвентарь.
        /// </summary>
        /// <param name="id">Идентификатор предмета из Defs.</param>
        /// <param name="amount">Количество для добавления.</param>
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

            // Попытка стакать с существующим стеком
            var existing = _items.Find(i => i.Id == id && !i.IsContainer);
            if (existing != null)
            {
                int space = def.MaxStack - existing.Value;
                int add = Mathf.Min(amount, space);
                existing.Value += add;
                amount -= add;
            }

            // Создание новых стеков, если осталось ещё количество
            while (amount > 0 && def.MaxStack > 0)
            {
                int stack = Mathf.Min(amount, def.MaxStack);
                _items.Add(new InventoryItemData(id, stack));
                amount -= stack;
            }

            OnChanged?.Invoke();
        }

        /// <summary>
        /// Добавить предмет, автоматически создавая контейнер, если это необходимо.
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="amount">Количество (для контейнера всегда 1).</param>
        /// <param name="registry">Реестр контейнеров (обязателен для контейнеров).</param>
        public void Add(string id, int amount, InventoryRegistry registry)
        {
            if (amount <= 0) return;
            var def = DefsFacade.Instance.Items.Get(id);
            if (def.IsVoid) return;

            if (def.IsContainer)
            {
                // Автоматически создаём контейнер через AddContainer
                string containerId = Guid.NewGuid().ToString();
                AddContainer(id, containerId, registry);
                return;
            }

            // Для обычных предметов вызываем базовый метод
            Add(id, amount);
        }

        /// <summary>
        /// Удалить указанное количество предметов по Id (обычные предметы).
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="amount">Количество для удаления.</param>
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

        /// <summary>
        /// Удалить часть или весь слот по индексу. Для контейнеров удаляется весь слот целиком.
        /// </summary>
        /// <param name="index">Индекс слота.</param>
        /// <param name="amount">Количество для удаления.</param>
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

        /// <summary>
        /// Получить первый слот с указанным Id (без учёта контейнеров).
        /// </summary>
        public InventoryItemData GetItem(string id) => _items.Find(i => i.Id == id);

        /// <summary>
        /// Подсчитать общее количество предметов с указанным Id (сумма по всем слотам).
        /// </summary>
        public int Count(string id) => _items.Where(i => i.Id == id).Sum(i => i.Value);

        // ------------------------------------------------------------
        // Работа с контейнерами (требуют передачи InventoryRegistry)
        // ------------------------------------------------------------

        /// <summary>
        /// Добавить контейнер (сумку) в инвентарь.
        /// </summary>
        /// <param name="itemId">Id предмета-контейнера (из Defs).</param>
        /// <param name="containerId">Уникальный идентификатор экземпляра контейнера.</param>
        /// <param name="registry">Реестр всех контейнеров (для создания записи).</param>
        public void AddContainer(string itemId, string containerId, InventoryRegistry registry)
        {
            var def = DefsFacade.Instance.Items.Get(itemId);
            if (def.IsVoid || !def.IsContainer) return;

            // Создаём запись в реестре (пустой инвентарь для содержимого)
            registry.CreateContainer(containerId, itemId);

            // Добавляем слот в основной инвентарь
            var slot = new InventoryItemData(itemId, containerId, 1);
            _items.Add(slot);
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Проверить, можно ли положить указанный предмет в контейнер.
        /// </summary>
        /// <param name="containerId">Идентификатор контейнера.</param>
        /// <param name="item">Предмет, который пытаемся положить.</param>
        /// <param name="registry">Реестр контейнеров.</param>
        /// <returns>true, если предмет подходит по категории.</returns>
        public bool CanPlaceIntoContainer(string containerId, InventoryItemData item, InventoryRegistry registry)
        {
            var entry = registry.GetEntry(containerId);
            if (entry == null) return false;

            var containerDef = DefsFacade.Instance.Items.Get(entry.ItemId);
            if (containerDef.IsVoid || !containerDef.IsContainer) return false;

            var itemDef = DefsFacade.Instance.Items.Get(item.Id);
            return containerDef.ContainerConfig.Accepts(itemDef.Category);
        }

        /// <summary>
        /// Переместить предмет из одного инвентаря в контейнер (по идентификатору контейнера).
        /// </summary>
        /// <param name="sourceInventory">Инвентарь-источник.</param>
        /// <param name="sourceSlotIndex">Индекс слота в источнике.</param>
        /// <param name="targetContainerId">Идентификатор целевого контейнера.</param>
        /// <param name="amount">Количество для перемещения.</param>
        /// <param name="registry">Реестр контейнеров.</param>
        public void PlaceIntoContainer(InventoryData sourceInventory, int sourceSlotIndex, string targetContainerId, int amount, InventoryRegistry registry)
        {
            if (sourceSlotIndex < 0 || sourceSlotIndex >= sourceInventory._items.Count) return;
            var sourceSlot = sourceInventory._items[sourceSlotIndex];
            if (sourceSlot == null || sourceSlot.IsContainer) return; // запрещаем класть контейнер в контейнер

            if (!CanPlaceIntoContainer(targetContainerId, sourceSlot, registry)) return;

            var targetInventory = registry.GetContainer(targetContainerId);
            if (targetInventory == null) return;

            // Переносим предмет
            targetInventory.Add(sourceSlot.Id, amount);
            sourceInventory.RemoveAt(sourceSlotIndex, amount);
            OnChanged?.Invoke();
            targetInventory.OnChanged?.Invoke();
        }

        // ------------------------------------------------------------
        // Перемещение/обмен между инвентарями (без учёта контейнеров)
        // ------------------------------------------------------------

        /// <summary>
        /// Поменять местами два слота внутри одного инвентаря.
        /// </summary>
        public void Swap(int indexA, int indexB)
        {
            if (indexA == indexB) return;
            if (indexA < 0 || indexA >= _items.Count || indexB < 0 || indexB >= _items.Count) return;
            (_items[indexA], _items[indexB]) = (_items[indexB], _items[indexA]);
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Переместить слот из текущего инвентаря в другой инвентарь.
        /// Если целевой слот существует и предметы одинаковые – пытается стакнуть.
        /// Если целевой слот – контейнер, использует PlaceIntoContainer (требует registry, здесь не реализовано).
        /// </summary>
        /// <param name="target">Целевой инвентарь.</param>
        /// <param name="fromIndex">Индекс слота в текущем инвентаре.</param>
        /// <param name="targetIndex">Индекс в целевом инвентаре (-1 для добавления в конец).</param>
        public void MoveTo(InventoryData target, int fromIndex, int targetIndex = -1)
        {
            if (fromIndex < 0 || fromIndex >= _items.Count) return;
            var item = _items[fromIndex];

            // Если цель — контейнер, а перемещаемый предмет тоже контейнер – запрещаем
            if (targetIndex >= 0 && targetIndex < target._items.Count && target._items[targetIndex].IsContainer)
            {
                if (item.IsContainer)
                {
                    Debug.LogWarning("Нельзя положить контейнер в контейнер.");
                    return;
                }
                // Для обычных предметов используем PlaceIntoContainer (здесь нужен registry, поэтому лучше вынести отдельно)
                Debug.LogWarning("Для перемещения в контейнер используйте PlaceIntoContainer (требует registry).");
                return;
            }

            // Если целевой слот существует и не контейнер
            if (targetIndex >= 0 && targetIndex < target._items.Count)
            {
                var targetItem = target._items[targetIndex];
                // Стакинг только для обычных предметов (не контейнеров)
                if (targetItem.Id == item.Id && !targetItem.IsContainer && !item.IsContainer)
                {
                    int space = DefsFacade.Instance.Items.Get(item.Id).MaxStack - targetItem.Value;
                    if (space > 0)
                    {
                        int transfer = Mathf.Min(item.Value, space);
                        targetItem.Value += transfer;
                        item.Value -= transfer;
                        if (item.Value <= 0) _items.RemoveAt(fromIndex);
                        OnChanged?.Invoke();
                        target.OnChanged?.Invoke();
                        return;
                    }
                }
                // Обмен местами – работает для любых предметов, включая контейнеры
                (_items[fromIndex], target._items[targetIndex]) = (target._items[targetIndex], _items[fromIndex]);
            }
            else
            {
                // Перенос в конец – контейнеры перемещаются как обычные предметы
                target._items.Add(item);
                _items.RemoveAt(fromIndex);
            }

            OnChanged?.Invoke();
            target.OnChanged?.Invoke();
        }

        /// <summary>
        /// Переместить все предметы из текущего инвентаря в целевой.
        /// </summary>
        public void MoveAllTo(InventoryData target)
        {
            // Идём с конца, чтобы не сбивать индексы
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                MoveTo(target, i, -1);
            }
        }

        // ------------------------------------------------------------
        // Вес инвентаря с учётом контейнеров и модификаторов
        // ------------------------------------------------------------

        /// <summary>
        /// Вычислить полный вес инвентаря, включая содержимое контейнеров и применяя модификаторы веса.
        /// </summary>
        /// <param name="registry">Реестр контейнеров для доступа к их содержимому.</param>
        /// <returns>Общий вес в условных единицах.</returns>
        public float GetTotalWeight(InventoryRegistry registry)
        {
            float total = 0f;
            // Словарь модификаторов веса по категориям (самый сильный модификатор для каждой категории)
            var weightMods = new Dictionary<ItemCategory, float>();

            // Собираем модификаторы от всех контейнеров на этом уровне
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
                    // Берём наименьший множитель (максимальное снижение веса)
                    if (!weightMods.ContainsKey(cat) || weightMods[cat] > config.WeightMultiplier)
                        weightMods[cat] = config.WeightMultiplier;
                }
            }

            // Проходим по всем слотам и считаем вес
            foreach (var slot in _items)
            {
                var def = DefsFacade.Instance.Items.Get(slot.Id);
                if (def.IsVoid) continue;

                // Базовый вес предмета (вес * количество)
                float w = def.Weight * slot.Value;

                // Применяем модификатор, если он есть для этой категории
                if (weightMods.TryGetValue(def.Category, out float mod))
                    w *= mod;

                // Если это контейнер, добавляем вес его содержимого (рекурсивно)
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

        // ------------------------------------------------------------
        // Продвинутые "умные" методы
        // ------------------------------------------------------------

        /// <summary>
        /// Добавить предмет, пытаясь положить его в подходящий контейнер (сумку, колчан, кошелёк) из текущего инвентаря.
        /// Если подходящий контейнер найден, предмет добавляется туда.
        /// Если нет – добавляется в основной инвентарь через стандартный Add.
        /// Контейнеры (предметы, являющиеся контейнерами) всегда добавляются в основной инвентарь (создаётся новый экземпляр).
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="amount">Количество.</param>
        /// <param name="registry">Реестр контейнеров (из PlayerData).</param>
        public void AddToSuitableContainer(string id, int amount, InventoryRegistry registry)
        {
            if (amount <= 0) return;
            var def = DefsFacade.Instance.Items.Get(id);
            if (def.IsVoid) return;

            // Если предмет сам является контейнером – его нельзя положить в другой контейнер,
            // добавляем как новый контейнер в основной инвентарь.
            if (def.IsContainer)
            {
                AddContainer(id, Guid.NewGuid().ToString(), registry);
                return;
            }

            // Поиск подходящего контейнера в основном инвентаре (на верхнем уровне)
            InventoryItemData suitableContainer = null;
            foreach (var slot in _items)
            {
                if (!slot.IsContainer) continue;
                var entry = registry.GetEntry(slot.ContainerId);
                if (entry == null) continue;
                var containerDef = DefsFacade.Instance.Items.Get(entry.ItemId);
                if (containerDef.IsVoid || !containerDef.IsContainer) continue;
                if (containerDef.ContainerConfig.Accepts(def.Category))
                {
                    suitableContainer = slot;
                    break;
                }
            }

            if (suitableContainer != null)
            {
                var containerInventory = registry.GetContainer(suitableContainer.ContainerId);
                if (containerInventory != null)
                {
                    // Добавляем в контейнер
                    containerInventory.Add(id, amount);
                    // Сигнализируем об изменении (основной инвентарь не изменился, но UI может обновить иконку контейнера)
                    OnChanged?.Invoke();
                    return;
                }
            }

            // Если подходящего контейнера нет или не удалось добавить – в основной инвентарь
            Add(id, amount);
        }

        /// <summary>
        /// Удалить указанное количество предметов по Id из всех мест: сначала из контейнеров (сумок, колчанов),
        /// затем из основного инвентаря.
        /// Если предмет не найден ни в одном контейнере и не в основном инвентаре, выводится предупреждение.
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="amount">Количество для удаления.</param>
        /// <param name="registry">Реестр контейнеров (для доступа к их инвентарям).</param>
        public void RemoveFromAll(string id, int amount, InventoryRegistry registry)
        {
            if (amount <= 0) return;
            int toRemove = amount;

            // 1. Ищем в контейнерах (перебираем все слоты-контейнеры в основном инвентаре)
            foreach (var slot in _items)
            {
                if (!slot.IsContainer) continue;
                var containerInv = registry.GetContainer(slot.ContainerId);
                if (containerInv == null) continue;

                int countInContainer = containerInv.Count(id);
                if (countInContainer > 0)
                {
                    int removeNow = Mathf.Min(toRemove, countInContainer);
                    containerInv.Remove(id, removeNow);
                    toRemove -= removeNow;
                    if (toRemove == 0)
                    {
                        OnChanged?.Invoke();
                        return;
                    }
                }
            }

            // 2. Если осталось – удаляем из основного инвентаря
            int countInMain = Count(id);
            if (countInMain >= toRemove)
            {
                Remove(id, toRemove); // используем существующий метод Remove
            }
            else
            {
                // Если в основном инвентаре меньше, чем требуется – удаляем сколько есть
                if (countInMain > 0)
                    Remove(id, countInMain);
                Debug.LogWarning($"Не удалось найти достаточное количество предметов '{id}' для удаления. Требуется {amount}, доступно {countInMain + (amount - toRemove)}.");
            }
        }

        /// <summary>
        /// Подсчитать общее количество предметов с указанным Id, включая предметы внутри контейнеров (сумок, колчанов).
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="registry">Реестр контейнеров (для доступа к их инвентарям).</param>
        /// <returns>Общее количество предметов (суммарно по всем слотам и контейнерам).</returns>
        public int CountTotal(string id, InventoryRegistry registry)
        {
            int total = Count(id); // считаем в основном инвентаре

            // Проходим по контейнерам в основном инвентаре
            foreach (var slot in _items)
            {
                if (!slot.IsContainer) continue;
                var containerInv = registry.GetContainer(slot.ContainerId);
                if (containerInv != null)
                {
                    total += containerInv.Count(id);
                }
            }

            return total;
        }

        /// <summary>
        /// Переместить все предметы из указанного инвентаря (например, сундука) в текущий инвентарь,
        /// пытаясь автоматически разложить их по подходящим контейнерам (сумкам, колчанам).
        /// Контейнеры (сумки) перемещаются как обычные предметы в основной инвентарь, сохраняя своё содержимое.
        /// </summary>
        /// <param name="sourceInventory">Инвентарь-источник (сундук, труп).</param>
        /// <param name="registry">Реестр контейнеров (из PlayerData).</param>
        public void MoveAllToSuitableContainers(InventoryData sourceInventory, InventoryRegistry registry)
        {
            if (sourceInventory == null || sourceInventory._items.Count == 0) return;

            // Идём с конца, чтобы безопасно удалять элементы
            for (int i = sourceInventory._items.Count - 1; i >= 0; i--)
            {
                var item = sourceInventory._items[i];
                if (item == null) continue;

                // Если предмет — контейнер (сумка), перемещаем как обычный предмет в основной инвентарь
                if (item.IsContainer)
                {
                    // Используем MoveTo для перемещения контейнера целиком (сохраняя ContainerId)
                    sourceInventory.MoveTo(this, i, -1);
                    // После MoveTo sourceInventory изменился, но мы идём с конца, поэтому индексы не сбиваются
                    continue;
                }

                // Обычный предмет: пытаемся положить в подходящий контейнер,
                // если нет – в основной инвентарь
                int amount = item.Value;
                AddToSuitableContainer(item.Id, amount, registry);
                // Удаляем из sourceInventory (если количество стало 0 – слот удалится)
                sourceInventory.RemoveAt(i, amount);
            }

            // События обновляются внутри AddToSuitableContainer и RemoveAt
        }

        // ------------------------------------------------------------
        // Сортировка (заглушки для будущей реализации)
        // ------------------------------------------------------------

        /// <summary>
        /// Сортировка предметов по весу (по возрастанию или убыванию).
        /// </summary>
        public void SortByWeight(bool ascending = true) { /* реализация */ }

        /// <summary>
        /// Сортировка предметов по стоимости (по возрастанию или убыванию).
        /// </summary>
        public void SortByValue(bool ascending = true) { /* реализация */ }
    }
}
