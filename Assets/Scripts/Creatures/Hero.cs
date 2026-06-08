using Assets.Scripts.Model;
using Scripts.Components;
using UnityEditor.Animations;
using UnityEngine;

namespace Scripts.Creatures
{
    public class Hero : MonoBehaviour
    {
        //Данные для игрока
        private GameSession _gameSession;

        //Скорость персонажа, сила прыжка и макс. кол. доп прыжков
        [Header("Movement settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpPower;
        [SerializeField] private int _maxExtraJumps;

        //Граунд чекер
        [Header("Ground Checker")]
        [SerializeField] private LayerCheck _groundCheck;

        //Интерактивность (настройка слоя интерактивных объектов и радиуса в котором проверяются интерактивные объекты)
        [Header("Interaction settings")]
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayer;

        //Партикл анимации
        [Header("Animations")]
        [SerializeField] private SpawnComponent _footStepParticles;
        [SerializeField] private SpawnComponent _jumpParticles;
        [SerializeField] private SpawnComponent _fallParticles;
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private AnimatorController _heroUnarmed;
        [SerializeField] private AnimatorController _heroArmed;

        //Настройка атаки
        [Header("Attack settings")]
        [SerializeField] private int _attackDamage;
        [SerializeField] private AttackHitbox _attackHitbox;

        private Vector2 _moveDirection;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private Collider2D[] _interactionResult = new Collider2D[1];

        private static readonly int IsGround = Animator.StringToHash("is_ground");
        private static readonly int IsRunning = Animator.StringToHash("is_running");
        private static readonly int VerticalVelocity = Animator.StringToHash("vertical_velocity");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        private int _jumpsLeft;
        private bool _jumpRequested;
        private bool _doubleJumpUsedThisAirborne;
        private float _timeInAir;
        private Transform _activePlatform; //Запоминаем текущую платформу

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _gameSession = FindObjectOfType<GameSession>();

            if (_gameSession.PlayerData.IsArmed)
                _animator.runtimeAnimatorController = _heroArmed;
            else
                _animator.runtimeAnimatorController = _heroUnarmed;

            HealthComponent health = GetComponent<HealthComponent>();
            if (health != null)
            {
                health.SetHealth(_gameSession.PlayerData.Hp);
            }
        }

        private void FixedUpdate()
        {
            //горизонтальное движение
            _rigidbody.velocity = new Vector2(_moveDirection.x * _speed, _rigidbody.velocity.y);

            //обработка прыжка и логики падения
            JumpCalc();
            LogicOfFalling();

            //обработка анимаций
            _animator.SetBool(IsGround, IsGrounded());
            _animator.SetBool(IsRunning, _moveDirection.x != 0);
            _animator.SetFloat(VerticalVelocity, _rigidbody.velocity.y);

            //обработка направления спрайта персонажа
            SpriteDirection();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                _activePlatform = collision.transform;
                transform.SetParent(_activePlatform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                _activePlatform = null;
                transform.SetParent(null);
            }
        }

        private void LogicOfFalling()
        {
            bool isGrounded = IsGrounded();
            bool inTheAir = isGrounded ? false : true;

            if (inTheAir)
            {
                _timeInAir += Time.deltaTime;
            }

            if ((_doubleJumpUsedThisAirborne || _timeInAir > 2f) && isGrounded == true)
            {
                SpawnFallDast();
            }

            if (isGrounded)
            {
                _timeInAir = 0;
                _doubleJumpUsedThisAirborne = false;
            }
        }

        public void JumpRequest()
        {
            _jumpRequested = true;
        }

        private void JumpCalc()
        {
            if (_jumpRequested)
            {
                if (IsGrounded())
                {
                    PerformJump();
                    _jumpsLeft = _maxExtraJumps;
                    _doubleJumpUsedThisAirborne = false;
                }
                else if (_jumpsLeft > 0)
                {
                    PerformJump();
                    _jumpsLeft--;
                    _doubleJumpUsedThisAirborne = true;
                }
                _jumpRequested = false;
            }

            if (IsGrounded())
            {
                _jumpsLeft = _maxExtraJumps;
            }
        }

        private void PerformJump()
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }

        private bool IsGrounded()
        {
            return _groundCheck.IsTouchingLayer;
        }

        private void SpriteDirection()
        {
            if (_moveDirection.x > 0)
            {
                transform.localScale = Vector3.one;
                if (_attackHitbox != null)
                    _attackHitbox.transform.localRotation = Quaternion.identity;
            }
            else if (_moveDirection.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                if (_attackHitbox != null)
                    _attackHitbox.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }

        public void SetMovementDirection(Vector2 direction)
        {
            _moveDirection = direction;
        }

        public void TakeDamage()
        {
            _animator.SetTrigger(Hit);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);

            if (_gameSession.PlayerData.Money > 0)
            {
                SpawnCoins();
            }
        }

        public void OnHeroHealthChanged(int newHealth)
        {
            _gameSession.PlayerData.Hp = newHealth;
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_gameSession.PlayerData.Money, 5);
            _gameSession.PlayerData.Money -= numCoinsToDispose;

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.Play();
        }

        public void CollectCoin(int cost)
        {
            _gameSession.PlayerData.Money += cost;
            Debug.Log($"У игрока: {_gameSession.PlayerData.Money} денег.");
        }

        public void Interact()
        {
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, _interactionRadius, _interactionResult, _interactionLayer);
            for (int i = 0; i < size; i++)
            {
                var interactable = _interactionResult[i].GetComponent<InteractableComponent>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        public void SpawnFootDast()
        {
            _footStepParticles.Spawn();
        }

        public void SpawnJumpDast()
        {
            _jumpParticles.Spawn();
        }

        public void SpawnFallDast()
        {
            _fallParticles.Spawn();
        }

        public void Attack()
        {
            if (!_gameSession.PlayerData.IsArmed) return;
            _animator.SetTrigger(AttackKey);
        }

        public void PerformDamage()
        {
            if (_attackHitbox != null)
                _attackHitbox.Attack(_attackDamage);
        }

        public void ChangeArmedOrUnarmed()
        {
            _gameSession.PlayerData.IsArmed = !_gameSession.PlayerData.IsArmed;
            _animator.runtimeAnimatorController = _gameSession.PlayerData.IsArmed ? _heroArmed : _heroUnarmed;
        }
    }
}
