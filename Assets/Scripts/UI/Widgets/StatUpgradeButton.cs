using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.UI.Hud.CharacterWindow;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Widgets
{
    public enum StatType
    {
        MaxHp,
        Attack,
        Defense,
        MoveSpeed
    }

    public class StatUpgradeButton : MonoBehaviour
    {
        [Header("Stat Settings")]
        [SerializeField] private StatType _statType;
        [SerializeField] private int _hpBonus = 5;          // для MaxHp
        [SerializeField] private int _attackBonus = 1;      // для Attack
        [SerializeField] private int _defenseBonus = 1;     // для Defense
        [SerializeField] private float _speedBonus = 0.5f;  // для MoveSpeed
        [SerializeField] private int _costPerUpgrade = 1;   // очков параметров за одно повышение
        [SerializeField] private int _maxLevel = 10;        // максимальное количество повышений (не значение, а количество раз)

        private Button _button;
        private GameSession _session;
        private PlayerData _playerData;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button == null)
                Debug.LogError("StatUpgradeButton requires Button component!");

            _session = FindObjectOfType<GameSession>();
            if (_session == null)
            {
                Debug.LogError("GameSession not found!");
                return;
            }
            _playerData = _session.PlayerData;

            // Подписка на изменение очков параметров
            _playerData.LevelData.OnPointsChanged += UpdateButtonState;
        }

        private void Start()
        {
            UpdateButtonState();
        }

        private void OnDestroy()
        {
            if (_playerData != null && _playerData.LevelData != null)
                _playerData.LevelData.OnPointsChanged -= UpdateButtonState;
        }

        private int GetUpgradeCount()
        {
            return _playerData.StatUpgradeData.GetUpgradeCount(_statType);
        }

        private bool CanUpgrade()
        {
            return _playerData.LevelData.AvailablePoints >= _costPerUpgrade &&
                   GetUpgradeCount() < _maxLevel;
        }

        public void OnClick()
        {
            if (!CanUpgrade()) return;

            if (!_playerData.LevelData.SpendPoints(_costPerUpgrade)) return;

            // Применяем бонус
            switch (_statType)
            {
                case StatType.MaxHp:
                    _playerData.MaxHp += _hpBonus;
                    break;
                case StatType.Attack:
                    _playerData.Attack += _attackBonus;
                    break;
                case StatType.Defense:
                    _playerData.Defense += _defenseBonus;
                    break;
                case StatType.MoveSpeed:
                    _playerData.MoveSpeed += _speedBonus;
                    break;
            }

            _playerData.StatUpgradeData.AddUpgrade(_statType);

            // Обновляем UI
            var controller = FindObjectOfType<CharacterWindowController>();
            if (controller != null)
                controller.RefreshStats();

            UpdateButtonState();
        }

        private void UpdateButtonState(int _)
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            if (_button == null) return;
            _button.interactable = CanUpgrade();
        }
    }
}
