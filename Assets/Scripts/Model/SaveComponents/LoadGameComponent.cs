using UnityEngine;

namespace Assets.Scripts.Model.SaveComponents
{
    public class LoadGameComponent : MonoBehaviour
    {
        [SerializeField] private string _slotName = "ManualSave";

        public void Load()
        {
            var session = GameSession.Instance;
            if (session != null)
                session.LoadFromSlot(_slotName);
        }
    }
}
