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
        private GameSession _gameSession;

        // Быстрый доступ к событиям
        public event Action OnChanged
        {
            add => _data.OnChanged += value;
            remove => _data.OnChanged -= value;
        }

        public void Awake()
        {
            _gameSession = FindObjectOfType<GameSession>();
        }

        public float GetTotalWeight(InventoryRegistry registry) => _data.GetTotalWeight(registry);

        // Прокси-методы, если нужно
        public void Add(string id, int amount) => _data.Add(id, amount);
        public void Remove(string id, int amount) => _data.Remove(id, amount);

        // Чисто для проверки работоспособности метода переноса вещей из инвентаря в другой инвентарь
        public void MoveAllTo()
        {
            Data.MoveAllTo(_gameSession.PlayerData.Inventory);
        }

        // ... остальное по необходимости
    }
}
