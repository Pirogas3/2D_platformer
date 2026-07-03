using System;
using UnityEngine;

namespace Assets.Scripts.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/ItemProperties", fileName = "ItemProperties")]
    public class ItemPropertiesDef : ScriptableObject
    {
        [SerializeField] private ItemPropertyEntry[] _entries;

        public ItemProperties Get(string id)
        {
            foreach (var entry in _entries)
                if (entry.Id == id) return entry.Properties;
            return null;
        }
    }

    [Serializable]
    public class ItemPropertyEntry
    {
        [InventoryId] public string Id;
        public ItemProperties Properties;
    }

    [Serializable]
    public class ItemProperties
    {
        public int Damage;
        public int Healing;
        public int SpeedBoost;
        public float Duration;
        public float ThrowCooldown;
    }
}
