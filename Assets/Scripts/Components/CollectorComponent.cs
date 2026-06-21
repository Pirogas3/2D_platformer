using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class CollectorComponent : MonoBehaviour
    {
        [SerializeField] private List<InventoryItemData> _items = new List<InventoryItemData>();

        public void AddInInventory(string id, int value)
        {
            _items.Add(new InventoryItemData(id) { Value = value });
        }

        public void DropInInventory()
        {
            var session = FindObjectOfType<GameSession>();
            foreach (var invenotryItemData in _items)
            {
                session.PlayerData.Inventory.Add(invenotryItemData.Id, invenotryItemData.Value);
            }

            _items.Clear();
        }
    }
}
