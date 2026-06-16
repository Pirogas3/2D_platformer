using Assets.Scripts.Model.Definitions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class InventoryData
    {
        [SerializeField] private List<InventoryItemData> _inventory = new List<InventoryItemData>();

        public void Add(string id, int value)
        {
            if (value <= 0) return;

            var itemDef = DefsFacade.Instance.Items.Get(id);
            if (itemDef.IsVoid) return;

            var item = GetItem(id);
            if (item == null)
            {
                item = new InventoryItemData(id);
                _inventory.Add(item);
            }

            item.Value += value;
        }

        public void Remove(string id, int value)
        {
            var itemDef = DefsFacade.Instance.Items.Get(id);
            if (itemDef.IsVoid) return;

            var item = GetItem(id);
            if (item == null) return;

            item.Value -= value;

            if (item.Value <= 0)
            {
                _inventory.Remove(item);
            }
        }

        private InventoryItemData GetItem(string id)
        {
            foreach (var itemData in _inventory)
            {
                if(itemData.Id == id)
                    return itemData;
            }

            return null;
        }

        public int Count(string id)
        {
            var count = 0;
            foreach (var item in _inventory)
            {
                if (item.Id == id)
                {
                    count += item.Value;
                }
            }

            return count;
        }
    }

    [Serializable]
    public class InventoryItemData
    {
        [InventoryId] public string Id;
        public int Value;

        public InventoryItemData(string id)
        {
            Id = id;
            Value = 0;
        }
        public InventoryItemData(string id, int value)
        {
            Id= id;
            Value= value;
        }
    }
}
