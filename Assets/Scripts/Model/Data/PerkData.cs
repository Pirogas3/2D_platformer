using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class PerkData
    {
        [SerializeField] private List<PerkEntry> _entries = new List<PerkEntry>();

        public bool IsUnlocked(string perkId)
        {
            var entry = _entries.Find(e => e.Id == perkId);
            return entry != null && entry.IsUnlocked;
        }

        public void Unlock(string perkId)
        {
            var entry = _entries.Find(e => e.Id == perkId);
            if (entry != null)
                entry.IsUnlocked = true;
            else
                _entries.Add(new PerkEntry { Id = perkId, IsUnlocked = true });
        }

        [Serializable]
        public class PerkEntry
        {
            public string Id;
            public bool IsUnlocked;
        }
    }
}
