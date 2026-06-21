using Assets.Scripts.Model;
using Assets.Scripts.Model.Definitions;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class RequireItemComponent : MonoBehaviour
    {
        [SerializeField] private RequiredItem[] _required;
        [SerializeField] private bool _removeAfterUse;

        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;

        public void Check()
        {
            var session = FindObjectOfType<GameSession>();
            var inventory = session.PlayerData.Inventory;
            var areAllRequirementsMet = true;

            foreach (var req in _required)
            {
                if (inventory.Count(req.Id) < req.Amount)
                {
                    areAllRequirementsMet = false;
                    break;
                }
            }

            if (areAllRequirementsMet)
            {
                if (_removeAfterUse)
                {
                    foreach (var req in _required)
                    {
                        inventory.Remove(req.Id, req.Amount);
                    }
                }
                _onSuccess?.Invoke();
            }
            else
            {
                Debug.Log("Условия не выполнены");
                _onFail?.Invoke();
            }
        }
    }

    [Serializable]
    public class RequiredItem
    {
        [InventoryId] public string Id;
        public int Amount;
    }
}
