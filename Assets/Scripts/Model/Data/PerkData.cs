using Assets.Scripts.Model.Definitions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class PerkData
    {
        [SerializeField] private List<PerkEntry> _entries = new List<PerkEntry>();

        public int GetLevel(string perkId)
        {
            var entry = _entries.Find(e => e.Id == perkId);
            return entry != null ? entry.Level : 0;
        }

        public void AddLevel(string perkId, int delta = 1)
        {
            var entry = _entries.Find(e => e.Id == perkId);
            if (entry != null)
                entry.Level += delta;
            else
                _entries.Add(new PerkEntry { Id = perkId, Level = delta });
        }

        public bool IsMaxLevel(string perkId)
        {
            int current = GetLevel(perkId);
            int max = DefsFacade.Instance.Perks.Get(perkId)?.MaxLevel ?? 0;
            return current >= max;
        }

        public bool IsUnlocked(string perkId)
        {
            return GetLevel(perkId) > 0;
        }

        [Serializable]
        public class PerkEntry
        {
            public string Id;
            public int Level;
        }
    }
}
