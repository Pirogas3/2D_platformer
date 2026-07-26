using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Components
{
    public class ReloadLevelComponent : MonoBehaviour
    {
        private GameSession _session;

        private void Start()
        {
            _session = GameSession.Instance;
        }

        public void Reload()
        {
            if (_session != null)
            {
                _session.ResetToSceneStartState(); // ← восстановление начала сцены
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
