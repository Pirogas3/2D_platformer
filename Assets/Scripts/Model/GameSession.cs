using Assets.Scripts.Model.Data;
using Scripts.Creatures;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        public PlayerData PlayerData => _playerData;

        private PlayerData _sceneStartState; // состояние на начало текущей сцены

        public static GameSession Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);

            SaveSceneStartState();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // При загрузке новой сцены обновляем "начало сцены"
            SaveSceneStartState();
        }

        // Сохранить состояние на начало сцены
        public void SaveSceneStartState()
        {
            _sceneStartState = _playerData.Clone();
        }

        // Восстановить состояние до начала сцены (рестарт уровня)
        public void ResetToSceneStartState()
        {
            if (_sceneStartState != null)
                _playerData = _sceneStartState.Clone();
        }

        [ContextMenu("Quick Save")]
        public void QuickSave() => SaveToSlot("QuickSave");

        [ContextMenu("Quick Load")]
        public void QuickLoad() => LoadFromSlot("QuickSave");

        // Сохранение в слот
        public void SaveToSlot(string slotName)
        {
            // Обновляем текущую сцену перед сохранением
            _playerData.CurrentScene = SceneManager.GetActiveScene().name;

            var hero = FindObjectOfType<Hero>();
            if (hero != null)
            {
                _playerData.PosX = hero.transform.position.x;
                _playerData.PosY = hero.transform.position.y;
            }

            SaveManager.SaveToFile(_playerData, slotName);
            Debug.Log($"Игра сохранена в слот '{slotName}'.");
        }

        // Загрузка из слота
        public void LoadFromSlot(string slotName)
        {
            var loadedData = SaveManager.LoadFromFile<PlayerData>(slotName);
            if (loadedData == null)
            {
                Debug.LogWarning($"Слот '{slotName}' не найден или повреждён.");
                return;
            }

            // Восстанавливаем данные
            _playerData = loadedData;

            // Загружаем сохранённую сцену
            SceneManager.LoadScene(_playerData.CurrentScene);
            Debug.Log($"Загружен слот '{slotName}'.");
        }

        // Загрузка последнего сделанного сохранения
        public void LoadLastSlot()
        {
            string latestSlot = SaveManager.GetLatestSlot();
            if (!string.IsNullOrEmpty(latestSlot))
            {
                LoadFromSlot(latestSlot);
            }
            else
            {
                // Если сохранений нет
                Debug.Log("Нет сохранений.");
                return;
            }
        }

        // Удаление слота
        public void DeleteSlot(string slotName)
        {
            SaveManager.DeleteSlot(slotName);
        }
    }
}
