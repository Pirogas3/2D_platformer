using Assets.Scripts.Model.Data;
using UnityEngine;

namespace Assets.Scripts.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/DefsFacade", fileName = "DefsFacade")]
    public class DefsFacade : ScriptableObject
    {
        [SerializeField] private InventoryItemsDef _items;

        private static DefsFacade _instance;
        public static DefsFacade Instance => (_instance == null) ? LoadDefs() : _instance;

        public InventoryItemsDef Items => _items;

        private static DefsFacade LoadDefs()
        {
            return _instance = Resources.Load<DefsFacade>("DefsFacade");
        }
    }
}
