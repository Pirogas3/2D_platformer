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
            if (_session == null) return;

            // Проверяем, есть ли сохранения
            string latestSlot = SaveManager.GetLatestSlot();
            if (!string.IsNullOrEmpty(latestSlot))
            {
                // Загружаем последнее сохранение
                _session.LoadFromSlot(latestSlot);
            }
            else
            {
                // Если сохранений нет, сбрасываем состояние до начала сцены и перезагружаем
                _session.ResetToSceneStartState();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
