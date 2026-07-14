using UnityEngine;

namespace Assets.Scripts.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/Perks", fileName = "PerksDef")]
    public class PerksDef : ScriptableObject
    {
        [SerializeField] private PerkDef[] _perks;

        public PerkDef Get(string id)
        {
            foreach (var perk in _perks)
                if (perk.Id == id)
                    return perk;
            return null;
        }

        public int GetCost(string id)
        {
            var perk = Get(id);
            return perk != null ? perk.Cost : -1;
        }

        public PerkDef[] AllPerks => _perks;
    }
}
