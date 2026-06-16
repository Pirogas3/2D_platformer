using Assets.Scripts.Model.Definitions;
using Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Props
{
    public class Key : MonoBehaviour
    {
        [InventoryId][SerializeField] private string _id;
        [SerializeField] private int _cost;

        private Hero _hero;
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            GameObject heroGO = GameObject.FindWithTag("Player");
            if (heroGO != null)
                _hero = heroGO.GetComponent<Hero>();
        }

        public void СollectKey()
        {
            if (_hero != null)
            {
                _hero.AddInInventory(_id, _cost);
            }

            _collider.enabled = false;
        }
    }
}
