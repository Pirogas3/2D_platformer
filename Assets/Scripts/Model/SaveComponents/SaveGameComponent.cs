using UnityEngine;

namespace Assets.Scripts.Model.SaveComponents
{
    public class SaveGameComponent : MonoBehaviour
    {
        [SerializeField] private string _slotName = "ManualSave";

        public void Save()
        {
            var session = GameSession.Instance;
            if (session != null)
                session.SaveToSlot(_slotName);
        }
    }
}
