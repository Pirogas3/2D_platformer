using Assets.Scripts.Model.Definitions;
using Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [InventoryId][SerializeField] private string _id;
        [SerializeField] private int _count;

        public void Add(GameObject go)
        {
            var hero = go.GetComponent<Hero>();
            if (hero != null)
            {
                hero.AddInInventory(_id, _count);
            }
        }
    }
}
