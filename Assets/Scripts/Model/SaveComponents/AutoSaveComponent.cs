using UnityEngine;

namespace Assets.Scripts.Model.SaveComponents
{
    public class AutoSaveComponent : MonoBehaviour
    {
        [SerializeField] private string _slotName = "AutoSave";

        public void AutoSave()
        {
            var session = GameSession.Instance;
            if (session != null)
                session.SaveToSlot(_slotName);
        }
    }
}
