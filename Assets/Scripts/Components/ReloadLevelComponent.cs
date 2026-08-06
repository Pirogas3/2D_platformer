using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
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

            string latestSlot = SaveManager.GetLatestSlot();
            if (!string.IsNullOrEmpty(latestSlot))
            {
                // Загружаем данные из файла, чтобы проверить сцену
                var playerData = SaveManager.LoadFromFile<PlayerData>(latestSlot);
                if (playerData != null && playerData.CurrentScene == SceneManager.GetActiveScene().name)
                {
                    // Сохранение сделано в текущей сцене — загружаем его
                    _session.LoadFromSlot(latestSlot);
                    return;
                }
                // Иначе сохранение из другой сцены — игнорируем
            }

            // Если сохранения нет или оно не подходит — сбрасываем состояние и перезагружаем текущую сцену
            _session.ResetToSceneStartState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
