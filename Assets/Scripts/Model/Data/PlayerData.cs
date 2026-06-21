using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory = new InventoryData();
        public InventoryData Inventory => _inventory;

        [SerializeField] private InventoryRegistry _containerRegistry = new InventoryRegistry();
        public InventoryRegistry ContainerRegistry => _containerRegistry;

        public int Hp;
        public int MaxHp;
        public float PosX;
        public float PosY;
        public string CurrentScene;

        public bool IsArmed => Inventory.Count("Sword") >= 1 ? true : false;

        public PlayerData Clone()
        {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}
