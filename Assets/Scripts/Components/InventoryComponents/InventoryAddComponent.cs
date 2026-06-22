using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Components.InventoryComponents
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [InventoryId][SerializeField] private string _id;
        [SerializeField] private int _count;

        public void Add(GameObject go)
        {
            var hero = go.GetInterface<ICanAddInInventory>();
            hero?.AddInInventory(_id, _count);
        }

        public void SmartAdd(GameObject go)
        {
            var hero = go.GetInterface<ICanAddInInventory>();
            hero?.SmartAddInInventory(_id, _count);
        }
    }
}
