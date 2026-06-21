using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class InventoryRegistry
    {
        [SerializeField] private List<ContainerEntry> _entries = new List<ContainerEntry>();

        public InventoryData GetContainer(string containerId)
        {
            var entry = _entries.Find(e => e.Id == containerId);
            if (entry != null) return entry.Inventory;
            return null;
        }

        public ContainerEntry GetEntry(string containerId)
        {
            return _entries.Find(e => e.Id == containerId);
        }

        public void CreateContainer(string containerId, string itemId)
        {
            if (GetEntry(containerId) != null)
            {
                Debug.LogWarning($"Контейнер с id {containerId} уже существует.");
                return;
            }
            _entries.Add(new ContainerEntry { Id = containerId, ItemId = itemId, Inventory = new InventoryData() });
        }

        public void RemoveContainer(string containerId)
        {
            _entries.RemoveAll(e => e.Id == containerId);
        }

        [Serializable]
        public class ContainerEntry
        {
            public string Id;
            public string ItemId;
            public InventoryData Inventory = new InventoryData();
        }
    }
}
