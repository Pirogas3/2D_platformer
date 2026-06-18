using Assets.Scripts.Model.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        public PlayerData PlayerData => _playerData;

        private PlayerData _sceneStartState; // состояние на начало текущей сцены
        private PlayerData _checkpointState; //состояние на момент чекпоинта

        private void Awake()
        {
            if (IsSessionExit())
            {
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(this);

            // При первом запуске – запоминаем состояние как начало первой сцены
            SaveSceneStartState();

            // Подписываемся на загрузку сцен, чтобы обновлять "начало сцены" при переходе
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Отписываемся, чтобы избежать утечек
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // При загрузке любой новой сцены запоминаем текущее состояние как "начало сцены"
            // Это позволяет переходить между уровнями, сохраняя прогресс (инвентарь, здоровье)
            SaveSceneStartState();
        }

        public void SaveSceneStartState()
        {
            _sceneStartState = _playerData.Clone();
        }

        // Восстановить состояние до начала сцены (для рестарта уровня)
        public void ResetToSceneStartState()
        {
            if (_sceneStartState != null)
                _playerData = _sceneStartState.Clone();
        }

        public void SaveCheckpoint()
        {
            _checkpointState = _playerData.Clone();
        }

        public void LoadCheckpoint()
        {
            if (_checkpointState != null)
                _playerData = _checkpointState.Clone();
        }

        private bool IsSessionExit()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var session in sessions)
            {
                if (session != this) return true;
            }
            return false;
        }
    }
}
