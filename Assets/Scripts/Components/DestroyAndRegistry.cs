using Assets.Scripts.Model;

namespace Assets.Scripts.Components
{
    public class DestroyAndRegistry : DestroyObjectComponent
    {
        public override void DestroyObject()
        {
            RegisterDestroyedObject(GetComponent<PersistentObjectState>().UniqueId);

            base.DestroyObject();
        }

        public void RegisterDestroyedObject(string id)
        {
            var session = GameSession.Instance;
            session.PlayerData.EnviromentData.AddDestroyedObject(id);
        }
    }
}
