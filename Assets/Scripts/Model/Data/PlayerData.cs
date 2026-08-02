using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [Header("Inventory")]
        [SerializeField] private InventoryData _inventory = new InventoryData();
        public InventoryData Inventory => _inventory;

        [SerializeField] private InventoryRegistry _containerRegistry = new InventoryRegistry();
        public InventoryRegistry ContainerRegistry => _containerRegistry;

        [Header("Enviroment data")]
        [SerializeField] private EnviromentData _enviromentData = new EnviromentData();
        public EnviromentData EnviromentData => _enviromentData;

        [Header("Progression")]
        [SerializeField] private LevelData _levelData = new LevelData();
        public LevelData LevelData => _levelData;

        [SerializeField] private StatUpgradeData _statUpgradeData = new StatUpgradeData();
        public StatUpgradeData StatUpgradeData => _statUpgradeData;

        [Header("Perks")]
        [SerializeField] private PerkData _perkData = new PerkData();
        public PerkData PerkData => _perkData;

        [Header("Physical parameters")]
        [SerializeField] private int _maxHp = 20;
        public int MaxHp { get => _maxHp; set => _maxHp = value; }

        [SerializeField] private int _hp = 5;
        public event Action<int> HpOnChanged;
        public int Hp
        {
            get => _hp;
            set
            {
                var isEquals = _hp.Equals(value);
                if (isEquals) return;

                if (value >= _maxHp)
                {
                    _hp = _maxHp;
                    HpOnChanged?.Invoke(_hp);
                    return;
                }

                var oldValue = _hp;
                _hp = value;

                HpOnChanged?.Invoke(_hp);
            }
        }

        [SerializeField] private int _attack = 0;
        [SerializeField] private int _defense = 0;
        public int Attack { get => _attack; set => _attack = value; }
        public int Defense { get => _defense; set => _defense = value; }

        [SerializeField] private float _moveSpeed = 3f;
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        [SerializeField] private float _jumpPower = 9;
        public float JumpPower { get => _jumpPower; set => _jumpPower = value; }

        [Header("Equip parameters")]
        // Правило вооружен или нет и сохраняем id предмета которым вооружен
        [SerializeField] private bool _isArmed = false;
        [SerializeField] private string _weaponItemId = null;
        public bool IsArmed => _isArmed;
        public string WeaponItemId => _weaponItemId;

        [Header("Position")]
        public float PosX;
        public float PosY;

        [Header("Scene")]
        public string CurrentScene;

        // Команда одеть экпипированное оружие
        public void EquipWeapon(string itemId)
        {
            // Если пытаемся одеть тот же самый предмет - значит это команда его снять
            if (_weaponItemId == itemId)
            {
                _isArmed = false;
                _weaponItemId = null;
                return;
            }

            _isArmed = true;
            _weaponItemId = itemId;
        }

        public PlayerData Clone()
        {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}
