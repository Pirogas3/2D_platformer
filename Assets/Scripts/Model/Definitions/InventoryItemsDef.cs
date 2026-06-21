using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    /// <summary>
    /// Определения предметов инвентаря (доступны через DefsFacade).
    /// Хранит массив ItemDef и предоставляет поиск по идентификатору.
    /// </summary>
    [CreateAssetMenu(menuName = "Defs/InventoryItems", fileName = "InventoryItems")]
    public class InventoryItemsDef : ScriptableObject
    {
        [SerializeField] private ItemDef[] _items;

        /// <summary>
        /// Найти определение предмета по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <returns>Определение предмета или default (пустой ItemDef), если не найдено.</returns>
        public ItemDef Get(string id)
        {
            foreach (var item in _items)
            {
                if (item.Id == id) return item;
            }

            return default;
        }
#if UNITY_EDITOR
        /// <summary>Для отображения в редакторе (массив предметов).</summary>
        public ItemDef[] ItemsForEditor => _items;
#endif
    }

    /// <summary>
    /// Определение одного предмета: его свойства, категория, настройки контейнера (если это контейнер).
    /// </summary>
    [Serializable]
    public class ItemDef
    {
        [SerializeField] private string _id;
        /// <summary>Уникальный идентификатор предмета (используется в инвентаре).</summary>
        public string Id => _id;

        [SerializeField] private string _name;
        /// <summary>Отображаемое имя предмета.</summary>
        public string Name => _name;

        [SerializeField, TextArea] private string _description;
        /// <summary>Описание предмета (отображается в подсказках).</summary>
        public string Description => _description;

        [SerializeField] private Sprite _icon;
        /// <summary>Иконка предмета.</summary>
        public Sprite Icon => _icon;

        [SerializeField] private float _weight;
        /// <summary>Вес одного экземпляра (0 = без веса).</summary>
        public float Weight => _weight;

        [SerializeField] private int _baseValue;
        /// <summary>Базовая стоимость (продажи/покупки), 0 = без цены.</summary>
        public int BaseValue => _baseValue;

        [SerializeField] private int _maxStack = 99;
        /// <summary>Максимальное количество в одном стеке.</summary>
        public int MaxStack => _maxStack;

        [SerializeField] private ItemCategory _category;
        /// <summary>Категория предмета (для фильтрации контейнерами).</summary>
        public ItemCategory Category => _category;

        [SerializeField] private ContainerConfig _containerConfig;
        /// <summary>Конфиг контейнера (если предмет является контейнером), иначе null.</summary>
        public ContainerConfig ContainerConfig => _containerConfig;

        /// <summary>Является ли предмет контейнером (сумкой, колчаном и т.п.).</summary>
        public bool IsContainer => _containerConfig != null && _containerConfig.IsValid();

        /// <summary>Пустой ли предмет (проверка на отсутствие идентификатора).</summary>
        public bool IsVoid => string.IsNullOrEmpty(_id);
    }

    /// <summary>
    /// Категории предметов для фильтрации контейнерами, группировки, использования в геймплее и т.д.
    /// </summary>
    [Serializable]
    public enum ItemCategory
    {
        Misc, Weapon, Armor, Arrow, Money, Food, Potion, Container // Добавляются по небходимости
    }

    /// <summary>
    /// Конфигурация контейнера (сумки, колчана и т.п.).
    /// Определяет, какие категории предметов можно в него класть и как модифицируется их вес.
    /// </summary>
    [Serializable]
    public class ContainerConfig
    {
        [SerializeField] private List<ItemCategory> _acceptedCategories;
        /// <summary>Список категорий, разрешённых для хранения в этом контейнере.</summary>
        public List<ItemCategory> AcceptedCategories => _acceptedCategories;

        [SerializeField, Range(0f, 1f)] private float _weightMultiplier = 1f;
        /// <summary>
        /// Множитель веса предметов внутри контейнера (например, 0.25 означает уменьшение веса на 75%).
        /// </summary>
        public float WeightMultiplier => _weightMultiplier;

        /// <summary>
        /// Проверяет, является ли конфиг действительным (имеет хотя бы одну разрешённую категорию).
        /// </summary>
        public bool IsValid()
        {
            return _acceptedCategories != null && _acceptedCategories.Count > 0;
        }

        /// <summary>
        /// Проверяет, можно ли положить предмет указанной категории в этот контейнер.
        /// </summary>
        /// <param name="category">Категория предмета.</param>
        /// <returns>true, если категория разрешена, иначе false.</returns>
        public bool Accepts(ItemCategory category)
        {
            if (!IsValid()) return false;
            return _acceptedCategories.Contains(category);
        }
    }
}
