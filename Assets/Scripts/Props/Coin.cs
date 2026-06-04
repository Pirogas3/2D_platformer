using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Coin : MonoBehaviour
    {
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

        public void СollectСoin()
        {
            if (_hero != null)
            {
                _hero.CollectCoin(_cost);
            }

            _collider.enabled = false;
        }
    }
}
