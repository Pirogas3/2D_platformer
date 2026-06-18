using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory;
        public InventoryData Inventory => _inventory;

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
