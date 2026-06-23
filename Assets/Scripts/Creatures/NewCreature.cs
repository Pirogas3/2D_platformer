using Assets.Scripts.Audio;
using Assets.Scripts.Components;
using Assets.Scripts.Creatures.Weapon;
using Assets.Scripts.Props.Traps;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    /// <summary>
    /// Новый базовый класс существа (только для управления ИИ)
    /// </summary>
    public class NewCreature : MonoBehaviour, IDamageFromSpikes
    {
        [Header("Movement")]
        [SerializeField] protected float _speed = 3f;
        [SerializeField] protected float _jumpPower = 5f;
        [SerializeField] protected int _maxExtraJumps = 0;

        [Header("Ground Check")]
        [SerializeField] protected LayerCheck _groundCheck;

        [Header("Attack")]
        [SerializeField] protected AttackHitbox _attackHitbox;

        [Header("Particles & Sounds")]
        [SerializeField] protected SpawnListComponent _particles;
        [SerializeField] protected PlaySoundsComponent _sounds;

        protected Rigidbody2D _rigidbody;
        protected Animator _animator;
        protected SpriteRenderer _spriteRenderer;

        // Состояние движения
        protected Vector2 _moveDirection = Vector2.zero;
        protected bool _isGrounded;
        protected int _jumpsLeft;
        protected bool _jumpRequested;
        protected bool _doubleJumpUsed;
        protected float _timeInAir;

        // Анимационные хэши (Is - для bool, F - для float, I - для int, Key - для trigger)
        protected static readonly int IsGround = Animator.StringToHash("is_ground");
        protected static readonly int IsRunning = Animator.StringToHash("is_running");
        protected static readonly int FVerticalVelocity = Animator.StringToHash("vertical_velocity");
        protected static readonly int HitKey = Animator.StringToHash("hit");
        protected static readonly int MeleeAttackKey = Animator.StringToHash("melee_attack");
        protected static readonly int RangeAttackKey = Animator.StringToHash("range_attack");
        protected static readonly int DieKey = Animator.StringToHash("die");

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Start()
        {
            _jumpsLeft = _maxExtraJumps;
        }

        protected virtual void FixedUpdate()
        {
            Move();
            HandleJump();
            HandleFallingParticles();
            FlipSprite();
        }

        protected virtual void Update()
        {
            // Обновление анимаций
            _isGrounded = _groundCheck.IsTouchingLayer;
            _animator.SetBool(IsGround, _isGrounded);
            _animator.SetBool(IsRunning, _moveDirection.magnitude > 0.01f);
            _animator.SetFloat(FVerticalVelocity, _rigidbody.velocity.y);
        }

        // Движение
        protected virtual void Move()
        {
            _rigidbody.velocity = new Vector2(_moveDirection.x * _speed, _rigidbody.velocity.y);
        }

        // Прыжки
        public void JumpRequest()
        {
            _jumpRequested = true;
        }

        private void HandleJump()
        {
            if (_jumpRequested)
            {
                if (_isGrounded)
                {
                    PerformJump();
                    _jumpsLeft = _maxExtraJumps;
                    _doubleJumpUsed = false;
                }
                else if (_jumpsLeft > 0)
                {
                    PerformJump();
                    _jumpsLeft--;
                    _doubleJumpUsed = true;
                }
                _jumpRequested = false;
            }

            if (_isGrounded)
                _jumpsLeft = _maxExtraJumps;
        }

        protected virtual void PerformJump()
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            _sounds?.PlayClip("Jump");
        }

        // Падение
        private void HandleFallingParticles()
        {
            bool inAir = !_isGrounded;
            if (inAir)
                _timeInAir += Time.deltaTime;

            if ((_doubleJumpUsed || _timeInAir > 2f) && _isGrounded)
            {
                _particles.Spawn("Fall");
            }

            if (_isGrounded)
            {
                _timeInAir = 0f;
                _doubleJumpUsed = false;
            }
        }

        // Поворот спрайта
        protected virtual void FlipSprite()
        {
            if (_moveDirection.x > 0.01f)
                transform.localScale = Vector3.one;
            else if (_moveDirection.x < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // Установка направления (для AI)
        public virtual void SetMovementDirection(Vector2 direction)
        {
            _moveDirection = direction;
        }

        // Ближняя атака (активация анимации атаки)
        public virtual void MeleeAttack()
        {
            _animator.SetTrigger(MeleeAttackKey);
            _sounds?.PlayClip("Melee");
        }

        // Дальняя атака (активация анимации атаки)
        public virtual void RangeAttack()
        {
            _animator.SetTrigger(MeleeAttackKey);
            _sounds?.PlayClip("Range");
        }

        // Метод для непосредственного нанесения урона через хитбокс оружия (это скорее всего только для Melee атак)
        // можно вызывать по какой-то логике извне или же в самой анимации атаки
        public virtual void PerformDamage()
        {
            if (_attackHitbox != null)
                _attackHitbox.Attack();
        }

        // Методы для вызова анимации получения урона
        public virtual void TakeDamageSimple()
        {
            _animator.SetTrigger(HitKey);
        }

        // Для того чтобы существо подлетало если падает на пики (шипы на земле)
        // Этот метод есть в интерфейсе IDamageFromSpikes для того, чтобы пики корректно подкидывали существо, не получая ссылку на него целиком
        public virtual void TakeDamageFromSpikes()
        {
            _animator.SetTrigger(HitKey);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }

        // Метод для вызова анимации смерти
        public virtual void Die()
        {
            IsDead = true; //смерть для "мозгов" ИИ, чтобы больше ничего не делал
            _animator.SetTrigger(DieKey); //объект удаляется целиком после проигрывания анимации смерти
        }

        // --- Свойства для AI ---
        public bool IsGrounded => _isGrounded;
        public Vector2 Position => transform.position;
        public bool IsDead { get; set; }
    }
}
