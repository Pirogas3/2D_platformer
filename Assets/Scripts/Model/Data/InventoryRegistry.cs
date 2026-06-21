using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    /// <summary>
    /// Реестр всех контейнеров (сумок, колчанов и т.п.) в игре.
    /// Хранит инвентари для каждого экземпляра контейнера, доступные по уникальному идентификатору.
    /// </summary>
    [Serializable]
    public class InventoryRegistry
    {
        // Список записей о контейнерах (каждая запись содержит идентификатор, ItemId и инвентарь)
        [SerializeField] private List<ContainerEntry> _entries = new List<ContainerEntry>();

        /// <summary>
        /// Получить инвентарь контейнера по его уникальному идентификатору.
        /// </summary>
        /// <param name="containerId">Уникальный идентификатор контейнера.</param>
        /// <returns>Инвентарь контейнера или null, если контейнер не найден.</returns>
        public InventoryData GetContainer(string containerId)
        {
            var entry = _entries.Find(e => e.Id == containerId);
            if (entry != null) return entry.Inventory;
            return null;
        }

        /// <summary>
        /// Получить полную запись о контейнере (включая ItemId и инвентарь).
        /// </summary>
        /// <param name="containerId">Уникальный идентификатор контейнера.</param>
        /// <returns>Запись контейнера или null, если не найдена.</returns>
        public ContainerEntry GetEntry(string containerId)
        {
            return _entries.Find(e => e.Id == containerId);
        }

        /// <summary>
        /// Создать новый контейнер в реестре.
        /// </summary>
        /// <param name="containerId">Уникальный идентификатор для нового контейнера.</param>
        /// <param name="itemId">Идентификатор предмета-контейнера (из Defs), чтобы знать его параметры (категории, модификаторы).</param>
        public void CreateContainer(string containerId, string itemId)
        {
            // Проверяем, не существует ли уже контейнер с таким Id
            if (GetEntry(containerId) != null)
            {
                Debug.LogWarning($"Контейнер с id {containerId} уже существует.");
                return;
            }
            // Добавляем новую запись с пустым инвентарём
            _entries.Add(new ContainerEntry { Id = containerId, ItemId = itemId, Inventory = new InventoryData() });
        }

        /// <summary>
        /// Удалить контейнер из реестра по его идентификатору.
        /// </summary>
        /// <param name="containerId">Уникальный идентификатор контейнера.</param>
        public void RemoveContainer(string containerId)
        {
            _entries.RemoveAll(e => e.Id == containerId);
        }

        /// <summary>
        /// Запись о контейнере в реестре.
        /// </summary>
        [Serializable]
        public class ContainerEntry
        {
            public string Id; // Уникальный идентификатор экземпляра контейнера (генерируется при создании)
            public string ItemId; // Идентификатор предмета-контейнера (из ItemDef), чтобы знать его параметры
            public InventoryData Inventory = new InventoryData(); // Инвентарь, хранящий содержимое контейнера
        }
    }
}
