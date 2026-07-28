using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Components
{
    public class ExitLevelComponent : MonoBehaviour
    {
        [SerializeField] private string _sceneName = null;

        public void Exit()
        {
            var session = GameSession.Instance;
            session.PlayerData.PosX = 0;
            session.PlayerData.PosY = 0;
            session.ClearEnviromentData();
            SceneManager.LoadScene(_sceneName);
        }
    }
}
