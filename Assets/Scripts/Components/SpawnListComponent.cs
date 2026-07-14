using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class SpawnListComponent : MonoBehaviour
    {
        [SerializeField] private SpawnData[] _spawners;

        public void Spawn(string id)
        {
            var spawner = _spawners.FirstOrDefault(element => element.ID == id);
            spawner?.Component.Spawn();
        }

        public void Spawn(string id, int damage, int attack)
        {
            var spawner = _spawners.FirstOrDefault(element => element.ID == id);
            spawner?.Component.Spawn(damage, attack);
        }

        [Serializable]
        public class SpawnData
        {
            public string ID;
            public SpawnComponent Component;
        }
    }
}
