using Assets.Scripts.Components;
using Assets.Scripts.Components.CameraComponents;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using Assets.Scripts.UI.Hud.Dialogue;
using Assets.Scripts.UI.Hud.QucikInventory;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class Hero : Creature, ICanAddInInventory
    {
        //Данные для игрока
        private GameSession _gameSession;
        private QuickInventoryController _quickInventoryController;
        private HeroHealthComponent _heroHealthComponent;

        //Интерактивность
        [Header("Interaction settings")]
        [SerializeField] private float _interactionRadius; //радиус в котором проверяются интерактивные объекты
        [SerializeField] private LayerMask _interactionLayer; //слой интерактивных объектов
        private Collider2D[] _interactionResult = new Collider2D[5];

        //Анимации
        [Header("Animations")]
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private AnimatorController _heroUnarmed;
        [SerializeField] private AnimatorController _heroArmed;

        [Header("Player Input")]
        [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;

        [Header("Camera Zoom")]
        [SerializeField] private CameraZoomPPU _cameraZoom;

        private DialogBoxController _dialogBoxController;

        private int _meleeDamage = 0;
        private int _rangeDamage = 0;
        private float _throwCooldown = 0f;
        private float _lastThrowTime;

        protected override float MoveSpeed
        {
            get
            {
                if (_gameSession != null)
                    return _gameSession.PlayerData.MoveSpeed;
                else
                    return base.MoveSpeed;
            }
        }

        public int attack
        {
            get
            {
                if (_gameSession != null)
                    return _gameSession.PlayerData.Attack;
                else
                    return 0;
            }
        }

        public int defense
        {
            get
            {
                if (_gameSession != null)
                    return _gameSession.PlayerData.Defense;
                else
                    return 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            _gameSession = FindObjectOfType<GameSession>();

            ChangeArmedOrUnarmed(_gameSession.PlayerData.WeaponItemId);

            _heroHealthComponent = GetComponent<HeroHealthComponent>();
            if (_heroHealthComponent != null)
            {
                _heroHealthComponent.SetHealth(_gameSession.PlayerData.Hp);
            }

            _quickInventoryController = FindObjectOfType<QuickInventoryController>();
            if (_quickInventoryController == null)
                Debug.LogWarning("QuickInventoryController not found in scene!");

            ApplyPerks();

            if (_cameraZoom == null)
                _cameraZoom = FindObjectOfType<CameraZoomPPU>();

            _dialogBoxController = FindObjectOfType<DialogBoxController>();
            if (_dialogBoxController != null)
            {
                _dialogBoxController.OnDialogOpened += OnDialogOpened;
                _dialogBoxController.OnDialogClosed += OnDialogClosed;
            }
            else
            {
                Debug.LogWarning("DialogBoxController not found!");
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void OnDestroy()
        {
            if (_dialogBoxController != null)
            {
                _dialogBoxController.OnDialogOpened -= OnDialogOpened;
                _dialogBoxController.OnDialogClosed -= OnDialogClosed;
            }
        }

        private void OnDialogOpened()
        {
            if (_playerInput != null)
                _playerInput.DeactivateInput(); // или _playerInput.enabled = false;

            SetMovementDirection(Vector2.zero);
            _rigidbody.velocity = Vector2.zero;

            if (_cameraZoom != null)
                _cameraZoom.ZoomIn();
        }

        private void OnDialogClosed()
        {
            if (_playerInput != null)
                _playerInput.ActivateInput(); // или _playerInput.enabled = true;

            if (_cameraZoom != null)
                _cameraZoom.ZoomOut();
        }

        public void OnHeroHealthChanged(int newHealth)
        {
            _gameSession.PlayerData.Hp = newHealth;
        }

        private void SpawnCoins()
        {
            Debug.Log($"{_gameSession.PlayerData.Inventory.CountTotal("SilverCoin", _gameSession.PlayerData.ContainerRegistry)}");
            var numCoinsToDispose = Mathf.Min(_gameSession.PlayerData.Inventory.CountTotal("SilverCoin", _gameSession.PlayerData.ContainerRegistry), 5);
            _gameSession.PlayerData.Inventory.RemoveFromAll("SilverCoin", numCoinsToDispose, _gameSession.PlayerData.ContainerRegistry);

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.Play();
        }

        public void AddInInventory(string id, int amount)
        {
            if (id == "Sword" && _gameSession.PlayerData.Inventory.Count("Sword") < 1)
            {
                _gameSession.PlayerData.Inventory.Add(id, amount);
            }
            else _gameSession.PlayerData.Inventory.Add(id, amount);
        }

        public void SmartAddInInventory(string id, int amount)
        {
            if (id == "Sword" && _gameSession.PlayerData.Inventory.Count("Sword") < 1)
            {
                _gameSession.PlayerData.Inventory.Add(id, amount);
            }
            else _gameSession.PlayerData.Inventory.AddToSuitableContainer(id, amount, _gameSession.PlayerData.ContainerRegistry);
        }

        public void Interact()
        {
            int size = Physics2D.OverlapCircleNonAlloc(transform.position, _interactionRadius, _interactionResult, _interactionLayer);

            if (size == 0) return;

            Transform playerTransform = transform;
            float minDistance = float.MaxValue;
            InteractableComponent closestInteractable = null;

            for (int i = 0; i < size; i++)
            {
                Collider2D col = _interactionResult[i];
                if (col == null) continue;

                var interactable = col.GetComponent<InteractableComponent>();
                if (interactable == null) continue;

                float distance = Vector2.Distance(playerTransform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestInteractable = interactable;
                }
            }

            if (closestInteractable != null)
            {
                closestInteractable.Interact(gameObject);
            }
        }

        public override void Attack()
        {
            if (!_gameSession.PlayerData.IsArmed) return;
            _animator.SetTrigger(AttackKey);
        }

        public override void PerformDamage()
        {
            if (_attackHitbox != null)
                _attackHitbox.Attack(_meleeDamage, attack);
        }

        public override void ThrowAttack(float holdTime)
        {
            if (_gameSession.PlayerData.IsArmed == false || _rangeDamage == 0) return;

            if (_gameSession.PlayerData.Inventory.Count("Sword") <= 1 || Time.time < _lastThrowTime + _throwCooldown)
            {
                Debug.Log("Бросок не готов");
                return;
            }
            _lastThrowTime = Time.time;

            if (holdTime > 1.0f && _gameSession.PlayerData.Inventory.Count(_gameSession.PlayerData.WeaponItemId) >= 4)
            {
                _gameSession.PlayerData.Inventory.Remove(_gameSession.PlayerData.WeaponItemId, 3);
                StartCoroutine(MultiThrowAttack(holdTime));
            }
            else
            {
                _gameSession.PlayerData.Inventory.Remove(_gameSession.PlayerData.WeaponItemId, 1);
                _animator.SetTrigger(ThrowKey);
                _particles.Spawn("Throw", _rangeDamage, attack); // передаём урон и атаку
                if (_sounds != null) _sounds.PlayClip("Range");
                //base.ThrowAttack(holdTime);
            }
        }

        public IEnumerator MultiThrowAttack(float holdTime)
        {
            if (!_gameSession.PlayerData.IsArmed) yield break;

            int throwCount = holdTime > 1.0f ? 3 : 1;

            for (int i = 0; i < throwCount; i++)
            {
                _animator.SetTrigger(ThrowKey);
                _particles.Spawn("Throw", _rangeDamage, attack); // передаём урон и атаку
                if (_sounds != null) _sounds.PlayClip("Range");

                if (i < throwCount - 1)
                    yield return new WaitForSeconds(0.2f);
            }
        }

        public override void TakeDamageSimple()
        {
            base.TakeDamageSimple();
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * (_jumpPower / 3), ForceMode2D.Impulse);
        }

        public override void TakeDamageFromSpikes()
        {
            base.TakeDamageFromSpikes();
            if (_hitParticles != null)
                SpawnCoins();
        }

        public void UseQuickSlot()
        {
            if (_quickInventoryController == null)
            {
                Debug.LogWarning("QuickInventoryController not assigned!");
                return;
            }

            var itemData = _quickInventoryController.GetSelectedSlotData();
            if (itemData == null)
            {
                Debug.Log("Quick slot is empty.");
                return;
            }

            UseItem(itemData.Id);
        }

        public void ChangeArmedOrUnarmed(string itemId)
        {
            if (itemId == null || _gameSession.PlayerData.IsArmed == false)
            {
                _meleeDamage = 0;
                _rangeDamage = 0;
                _throwCooldown = 0f;
                _animator.runtimeAnimatorController = _heroUnarmed;
                return;
            }

            if (_gameSession.PlayerData.IsArmed)
            {
                var def = DefsFacade.Instance.Properties.Get(itemId);
                _meleeDamage = def.MeleeDamage;
                _rangeDamage = def.RangeDamage;
                _throwCooldown = def.ThrowCooldown;
                _animator.runtimeAnimatorController = _heroArmed;
            }
        }

        public void UseItem(string itemId)
        {
            var def = DefsFacade.Instance.Items.Get(itemId);
            if (def.IsVoid)
            {
                Debug.LogWarning($"Предмет {itemId} не найден в определениях.");
                return;
            }

            // Проверяем категорию предмета и выполняем действие
            switch (def.Category)
            {
                case ItemCategory.Weapon:
                    EquipWeapon(itemId);
                    break;
                case ItemCategory.Potion:
                    ApplyPotion(itemId);
                    break;

                case ItemCategory.Food:
                    Debug.Log($"Использование еды {def.Name} (пока не реализовано).");
                    break;

                case ItemCategory.Container:
                    Debug.Log($"Открытие контейнера {def.Name} (пока не реализовано).");
                    break;

                default:
                    Debug.Log($"Использование предмета {def.Name} не реализовано.");
                    break;
            }
        }

        private void EquipWeapon(string itemId)
        {
            _gameSession.PlayerData.EquipWeapon(itemId);
            ChangeArmedOrUnarmed(itemId);
        }

        private void ApplyPotion(string itemId)
        {
            var def = DefsFacade.Instance.Properties.Get(itemId);
            if (def.SpeedBoost > 0)
            {
                // потом реализуем зелье ускорения
            }
            if (def.Healing > 0)
            {
                if (_gameSession.PlayerData.Hp == _gameSession.PlayerData.MaxHp)
                {
                    Debug.Log($"HP максимальное - зелье здоровья не использовано!");
                    return;
                }

                _gameSession.PlayerData.Hp += def.Healing;
                _heroHealthComponent.SetHealth(_gameSession.PlayerData.Hp);
                _gameSession.PlayerData.Inventory.Remove(itemId, 1);
                Debug.Log($"Вы выпили зелье! + {def.Healing} HP. Текущее здоровье: {_gameSession.PlayerData.Hp}");
            }
        }

        public void AddExperience(int amount)
        {
            if (_gameSession == null) return;
            _gameSession.PlayerData.LevelData.AddExp(amount);
        }

        [ContextMenu("Add500XP")]
        public void AddExperience()
        {
            if (_gameSession == null) return;
            _gameSession.PlayerData.LevelData.AddExp(500);
        }

        private void ApplyPerks()
        {
            var perkData = _gameSession.PlayerData.PerkData;
            int doubleJumpLevel = perkData.GetLevel("DoubleJump");
            if (doubleJumpLevel > 0)
            {
                _maxExtraJumps += doubleJumpLevel;
            }
        }

        public void AddExtraJump(int amount)
        {
            _maxExtraJumps += amount;
        }
    }
}
