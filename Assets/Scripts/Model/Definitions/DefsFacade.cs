using Assets.Scripts.Model.Data;
using UnityEngine;

namespace Assets.Scripts.Model.Definitions
{
    /// <summary>
    /// Фасад для доступа ко всем определениям (предметы, характеристики и т.д.).
    /// Реализован как синглтон, загружаемый из Resources.
    /// </summary>
    [CreateAssetMenu(menuName = "Defs/DefsFacade", fileName = "DefsFacade")]
    public class DefsFacade : ScriptableObject
    {
        [SerializeField] private InventoryItemsDef _items;
        [SerializeField] private ItemPropertiesDef _properties;
        [SerializeField] private PerksDef _perks;

        private static DefsFacade _instance;
        /// <summary>
        /// Единственный экземпляр фасада. Загружается из Resources при первом обращении.
        /// </summary>
        public static DefsFacade Instance => (_instance == null) ? LoadDefs() : _instance;

        /// <summary>Определения всех предметов инвентаря.</summary>
        public InventoryItemsDef Items => _items;
        /// <summary>Определения доп свойств предметов инвентаря.</summary>
        public ItemPropertiesDef Properties => _properties;
        /// <summary>Определения всех перков в игре.</summary>
        public PerksDef Perks => _perks;

        /// <summary>
        /// Загрузить фасад из Resources.
        /// </summary>
        private static DefsFacade LoadDefs()
        {
            return _instance = Resources.Load<DefsFacade>("DefsFacade");
        }
    }
}
