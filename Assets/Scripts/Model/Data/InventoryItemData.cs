using Assets.Scripts.Model.Definitions;
using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class InventoryItemData
    {
        [InventoryId] public string Id;
        public int Value;

        [SerializeField] private string _containerId;
        public string ContainerId => _containerId;

        public bool IsContainer => !string.IsNullOrEmpty(_containerId);

        public InventoryItemData(string id, int value = 0)
        {
            Id = id;
            Value = value;
            _containerId = string.Empty;
        }

        public InventoryItemData(string id, string containerId, int value = 1)
        {
            Id = id;
            Value = value;
            _containerId = containerId;
        }

        public void InitializeContainer(string containerId)
        {
            if (string.IsNullOrEmpty(containerId)) return;
            _containerId = containerId;
            Value = 1; // контейнер всегда нестакаемый
        }

        public float GetSelfWeight()
        {
            var def = DefsFacade.Instance.Items.Get(Id);
            return def.IsVoid ? 0f : def.Weight * Value;
        }
    }
}
