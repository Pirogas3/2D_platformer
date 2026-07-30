using Assets.Scripts.Components.InventoryComponents;
using Assets.Scripts.Creatures;
using Assets.Scripts.Model.Data;
using Assets.Scripts.UI.Hud;
using Assets.Scripts.UI.Hud.CharacterWindow;
using Assets.Scripts.UI.Hud.Inventory;
using SheetXExample;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        public PlayerData PlayerData => _playerData;

        private PlayerData _sceneStartState; // состояние на начало текущей сцены
        private InventoryWindowController _invWindowController; // он устанавливается самим InventoryWindowController-ом при его загрузке
        public InventoryWindowController InvWindowController { get => _invWindowController; set { _invWindowController = value; } }
        private EscController _escController; // он устанавливается самим EscController-ом при его загрузке
        public EscController EscController { get => _escController; set { _escController = value; } }
        private CharacterWindowController _characterWindowController; // он устанавливается самим CharacterWindowController-ом при его загрузке
        public CharacterWindowController CharacterWindowController { get => _characterWindowController; set { _characterWindowController = value; } }

        public static GameSession Instance { get; private set; }

        public event System.Action OnSaved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);

            // Загружаем сцену с экраном загрузки, если она ещё не загружена
            if (!SceneManager.GetSceneByName("LoadingScreen").isLoaded)
            {
                SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
            }

            // Инициализация локализации, получаем сохранённый язык (по умолчанию english)
            string savedLanguage = PlayerPrefs.GetString("Language", "english");
            LocalizationsManager.Init(savedLanguage);

            SaveSceneStartState();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            // Закрытие контекстного меню по клику левой кнопкой мыши вне его
            if (Input.GetMouseButtonDown(0))
            {
                ContextMenuManager.HandleGlobalClick();
            }

            // Закрытие контекстного меню и инвентаря по Esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ContextMenuManager.IsMenuOpen)
                {
                    ContextMenuManager.CloseMenu();
                    return;
                }
                else if (_invWindowController.IsOpen)
                {
                    _invWindowController.ToggleWindow();
                    return;
                }
                else if (_characterWindowController.IsOpen)
                {
                    _characterWindowController.ToggleWindow();
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void LoadHud()
        {
            // Проверяем, загружена ли сцена с именем "Hud"
            Scene hudScene = SceneManager.GetSceneByName("Hud");
            if (hudScene.isLoaded)
            {
                Debug.Log("Hud уже загружена, пропускаем.");
                return;
            }

            // Не загружаем в MainMenu
            if (SceneManager.GetActiveScene().name == "MainMenu")
                return;

            SceneManager.LoadScene("Hud", LoadSceneMode.Additive);
            Debug.Log("Hud загружена");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Если загружена игровая сцена (не MainMenu)
            if (scene.name != "MainMenu" && scene.name != "Hud" && scene.name != "LoadingScreen")
            {
                LoadHud();

                // Восстанавливаем позицию игрока
                var hero = FindObjectOfType<Hero>();
                if (hero != null && _playerData.PosX != 0 && _playerData.PosY != 0)
                {
                    hero.transform.position = new Vector3(_playerData.PosX, _playerData.PosY, 0f);
                }

                // Восстанавливаем сундуки
                _playerData.EnviromentData.LoadChests();

                // Применяем состояния объектов
                _playerData.EnviromentData.ApplyObjectStates();

                // Удаляем уничтоженные объекты
                _playerData.EnviromentData.ApplyDestroyedObjects();
            }
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

        [ContextMenu("Auto Save")]
        public void AutoSave()
        {
            string slotName = SaveManager.GetNextAutoSaveSlot();
            SaveToSlot(slotName);
        }

        [ContextMenu("Auto Load")]
        public void AutoLoad() => LoadFromSlot("AutoSave");

        // Сохранение в слот
        public void SaveToSlot(string slotName)
        {
            OnSaved?.Invoke();

            // Обновляем текущую сцену перед сохранением
            _playerData.CurrentScene = SceneManager.GetActiveScene().name;

            // Сохраняем позиция игрока
            var hero = FindObjectOfType<Hero>();
            if (hero != null)
            {
                _playerData.PosX = hero.transform.position.x;
                _playerData.PosY = hero.transform.position.y;
            }

            // Сохраняем сундуки
            var chests = FindObjectsOfType<ChestComponent>();
            _playerData.EnviromentData.SaveChests(chests);

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

        public void ClearEnviromentData()
        {
            _playerData.EnviromentData.ClearAll();
        }

        public void ResetPlayerData()
        {
            _playerData = new PlayerData();
        }
    }
}
