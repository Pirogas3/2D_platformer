using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Components
{
    public class ExitLevelComponent : MonoBehaviour
    {
        [SerializeField] private string _sceneName = null;

        public void Exit()
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
