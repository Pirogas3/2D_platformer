using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [CreateAssetMenu(menuName = "Defs/InventoryItems", fileName = "InventoryItems")]
    public class InventoryItemsDef : ScriptableObject
    {
        [SerializeField] private ItemDef[] _items;

        public ItemDef Get(string id)
        {
            foreach (var item in _items)
            {
                if (item.Id == id) return item;
            }

            return default;
        }
#if UNITY_EDITOR
        public ItemDef[] ItemsForEditor => _items;
#endif
    }

    [Serializable]
    public class ItemDef
    {
        [SerializeField] private string _id;
        public string Id => _id;

        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField, TextArea] private string _description;
        public string Description => _description;

        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField] private float _weight; // 0 = нет веса
        public float Weight => _weight;

        [SerializeField] private int _baseValue; // стоимость продажи/покупки, 0 = без цены
        public int BaseValue => _baseValue;

        [SerializeField] private int _maxStack = 99;
        public int MaxStack => _maxStack;

        [SerializeField] private ItemCategory _category;
        public ItemCategory Category => _category;

        // Для сумок: если null – обычный предмет
        [SerializeField] private ContainerConfig _containerConfig;
        public ContainerConfig ContainerConfig => _containerConfig;
        public bool IsContainer => _containerConfig != null && _containerConfig.IsValid();

        public bool IsVoid => string.IsNullOrEmpty(_id);
    }

    [Serializable]
    public enum ItemCategory
    {
        Misc, Weapon, Armor, Arrow, Money, Food, Potion, Container // добавляй по необходимости
    }

    [Serializable]
    public class ContainerConfig
    {
        [SerializeField] private List<ItemCategory> _acceptedCategories;
        public List<ItemCategory> AcceptedCategories => _acceptedCategories;

        [SerializeField, Range(0f, 1f)] private float _weightMultiplier = 1f;
        public float WeightMultiplier => _weightMultiplier;

        // Считаем конфиг действительным, только если есть хотя бы одна разрешённая категория
        public bool IsValid()
        {
            return _acceptedCategories != null && _acceptedCategories.Count > 0;
        }

        public bool Accepts(ItemCategory category)
        {
            if (!IsValid()) return false;
            return _acceptedCategories.Contains(category);
        }
    }
}
