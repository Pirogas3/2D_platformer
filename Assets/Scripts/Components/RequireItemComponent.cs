using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    public class RequireItemComponent : MonoBehaviour
    {
        [SerializeField] private InventoryItemData[] _required;
        [SerializeField] private bool _removeAfterUse;

        [SerializeField] private UnityEvent _onSuccess;
        [SerializeField] private UnityEvent _onFail;

        public void Check()
        {
            var session = FindObjectOfType<GameSession>();
            var areAllRequirementsMet = true;

            foreach (var item in _required)
            {
                var numItems = session.PlayerData.Inventory.Count(item.Id);
                if(numItems < item.Value)
                    areAllRequirementsMet = false;
            }

            if (areAllRequirementsMet)
            {
                if (_removeAfterUse)
                {
                    foreach (var item in _required)
                    {
                        session.PlayerData.Inventory.Remove(item.Id, item.Value);
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
}
