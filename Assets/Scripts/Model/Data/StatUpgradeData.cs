using Assets.Scripts.UI.Widgets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class StatUpgradeData
    {
        [SerializeField] private List<StatUpgradeEntry> _entries = new List<StatUpgradeEntry>();

        public int GetUpgradeCount(StatType statType)
        {
            var entry = _entries.Find(e => e.StatType == statType);
            return entry != null ? entry.Count : 0;
        }

        public void AddUpgrade(StatType statType)
        {
            var entry = _entries.Find(e => e.StatType == statType);
            if (entry != null)
                entry.Count++;
            else
                _entries.Add(new StatUpgradeEntry { StatType = statType, Count = 1 });
        }

        [Serializable]
        public class StatUpgradeEntry
        {
            public StatType StatType;
            public int Count;
        }
    }
}
