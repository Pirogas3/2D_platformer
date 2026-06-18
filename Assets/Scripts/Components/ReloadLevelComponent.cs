using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Components
{
    public class ReloadLevelComponent : MonoBehaviour
    {

        public void Reload()
        {
            var session = FindObjectOfType<GameSession>();
            if (session != null)
            {
                session.ResetToSceneStartState(); // ← восстановление начала сцены
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
