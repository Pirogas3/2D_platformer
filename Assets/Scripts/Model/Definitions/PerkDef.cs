using System;
using UnityEngine;

namespace Assets.Scripts.Model.Definitions
{
    [Serializable]
    public class PerkDef
    {
        [SerializeField] private string _id;
        public string Id => _id;

        [SerializeField] private string _description;
        public string Description => _description;

        [SerializeField] private int _cost;
        public int Cost => _cost;

        [SerializeField] private int _maxLevel = 1; // если перк может иметь несколько уровней (опционально)
        public int MaxLevel => _maxLevel;

        public bool IsVoid => string.IsNullOrEmpty(_id);
    }
}
