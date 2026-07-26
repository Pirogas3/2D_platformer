using Assets.Scripts.Model;
using Assets.Scripts.Model.Definitions;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components.InventoryComponents
{
    public class RequireItemComponent : MonoBehaviour
    {
        [SerializeField] private RequiredItem[] _required;
        [SerializeField] private bool _removeAfterUse;

        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;

        public void ResetRequiredItem()
        {
            _required = new RequiredItem[0];
        }

        public void Check()
        {
            var session = GameSession.Instance;
            if (session == null)
            {
                Debug.LogError("GameSession не найден!");
                return;
            }

            var inventory = session.PlayerData.Inventory;
            var registry = session.PlayerData.ContainerRegistry;
            var areAllRequirementsMet = true;

            // Проверяем наличие всех требуемых предметов (с учётом содержимого сумок)
            foreach (var req in _required)
            {
                if (inventory.CountTotal(req.Id, registry) < req.Amount)
                {
                    areAllRequirementsMet = false;
                    break;
                }
            }

            if (areAllRequirementsMet)
            {
                if (_removeAfterUse)
                {
                    // Удаляем предметы (сначала из сумок, потом из основного инвентаря)
                    foreach (var req in _required)
                    {
                        inventory.RemoveFromAll(req.Id, req.Amount, registry);
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
