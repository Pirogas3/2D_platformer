using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.UI.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI.Hud.CharacterWindow
{
    public class CharacterWindowController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _window;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private ProgresBarWidget _expProgressBar;
        [SerializeField] private TextMeshProUGUI _maxHpText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _defenseText;
        [SerializeField] private TextMeshProUGUI _moveSpeedText;
        [SerializeField] private TextMeshProUGUI _pointsText;

        [Header("Input")]
        [SerializeField] private InputActionReference _toggleAction;

        private GameSession _session;
        private PlayerData _playerData;
        private LevelData _levelData;
        private bool _isOpen = false;

        private void OnEnable()
        {
            if (_toggleAction != null)
                _toggleAction.action.performed += OnTogglePerformed;
        }

        private void OnDisable()
        {
            if (_toggleAction != null)
                _toggleAction.action.performed -= OnTogglePerformed;
        }

        private void Start()
        {
            _session = GameSession.Instance;
            if (_session == null)
            {
                Debug.LogError("GameSession not found!");
                return;
            }

            _session.CharacterWindowController = this;
            _playerData = _session.PlayerData;
            _levelData = _playerData.LevelData;

            // Подписка на события
            _levelData.OnLevelUp += UpdateLevel;
            _levelData.OnExpChanged += UpdateExp;
            _levelData.OnPointsChanged += UpdatePoints;

            // Подписка на изменение характеристик (добавим события в PlayerData позже, пока обновляем при открытии)
            // Для здоровья уже есть HpOnChanged, но мы покажем MaxHp, поэтому обновим при открытии.

            _window.SetActive(false);
            _isOpen = false;
        }

        private void OnDestroy()
        {
            if (_levelData != null)
            {
                _levelData.OnLevelUp -= UpdateLevel;
                _levelData.OnExpChanged -= UpdateExp;
                _levelData.OnPointsChanged -= UpdatePoints;
            }
        }

        private void OnTogglePerformed(InputAction.CallbackContext context)
        {
            ToggleWindow();
        }

        [ContextMenu("Toggle Window")]
        public void ToggleWindow()
        {
            _isOpen = !_isOpen;
            _window.SetActive(_isOpen);
            if (_isOpen)
                RefreshUI();
        }

        private void RefreshUI()
        {
            UpdateLevel(_levelData.Level);
            UpdateExp(_levelData.CurrentExp);
            UpdatePoints(_levelData.AvailablePoints);
            UpdateStats();
        }

        private void UpdateLevel(int level) => _levelText.text = level.ToString();

        private void UpdateExp(int exp)
        {
            int required = _levelData.GetExpRequiredForNextLevel();
            _expText.text = $"{exp} / {required}";
            _expProgressBar.SetProgress(_levelData.GetProgressToNextLevel());
        }

        private void UpdatePoints(int points) => _pointsText.text = points.ToString();

        private void UpdateStats()
        {
            if (_playerData == null) return;
            _maxHpText.text = _playerData.MaxHp.ToString();
            _attackText.text = _playerData.Attack.ToString();
            _defenseText.text = _playerData.Defense.ToString();
            _moveSpeedText.text = _playerData.MoveSpeed.ToString("F1"); // с одной цифрой после запятой
        }

        // Метод для внешнего вызова (если характеристики изменились из другого места)
        public void RefreshStats() => UpdateStats();

        public bool IsOpen => _isOpen;
    }
}
