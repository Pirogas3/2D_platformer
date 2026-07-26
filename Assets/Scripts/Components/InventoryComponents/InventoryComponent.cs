using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.Components.InventoryComponents
{
    public class InventoryComponent : MonoBehaviour
    {
        [SerializeField] private InventoryData _data = new InventoryData();
        public InventoryData Data => _data;

        // Быстрый доступ к событиям
        public event Action OnChanged
        {
            add => _data.OnChanged += value;
            remove => _data.OnChanged -= value;
        }

        public float GetTotalWeight(InventoryRegistry registry) => _data.GetTotalWeight(registry);

        public void Add(string id, int amount) => _data.Add(id, amount);
        public void Remove(string id, int amount) => _data.Remove(id, amount);
    }
}
