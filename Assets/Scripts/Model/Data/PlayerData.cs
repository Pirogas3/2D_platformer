using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [Header("Inventory")]
        [SerializeField] private InventoryData _inventory = new InventoryData();
        public InventoryData Inventory => _inventory;

        [SerializeField] private InventoryRegistry _containerRegistry = new InventoryRegistry();
        public InventoryRegistry ContainerRegistry => _containerRegistry;

        [Header("Physical parameters")]
        [SerializeField] private int _maxHp = 20;
        public int MaxHp => _maxHp;
        [SerializeField] private int _hp = 10;
        public event Action<int> HpOnChanged;
        public int Hp
        {
            get => _hp;
            set
            {
                var isEquals = _hp.Equals(value);
                if (isEquals) return;

                if (value >= _maxHp)
                {
                    _hp = _maxHp;
                    HpOnChanged?.Invoke(_hp);
                    return;
                }

                var oldValue = _hp;
                _hp = value;

                HpOnChanged?.Invoke(_hp);
            }
        }

        // Пока что не используются просто записываются при сохранении
        public float PosX;
        public float PosY;
        // Правило вооружен или нет
        public bool IsArmed => Inventory.Count("Sword") >= 1 ? true : false;

        [Header("Scene")]
        public string CurrentScene; // используется для сохранений и загрузки

        public PlayerData Clone()
        {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}
