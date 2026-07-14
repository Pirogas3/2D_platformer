using Assets.Scripts.Audio;
using Assets.Scripts.Components;
using Assets.Scripts.Creatures.Weapon;
using Assets.Scripts.Props.Traps;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class Creature : MonoBehaviour, IDamageFromSpikes
    {
        //Партикл анимации
        [Header("Particles")]
        [SerializeField] protected SpawnListComponent _particles;

        //Скорость существа, сила прыжка и макс. кол. доп прыжков
        [Header("Movement settings")]
        [SerializeField] protected float _speed;
        [SerializeField] protected float _jumpPower;
        [SerializeField] protected int _maxExtraJumps;

        protected virtual float MoveSpeed => _speed;

        //Чекеры
        [Header("Checkers")]
        [SerializeField] private LayerCheck _groundCheck;

        //Настройка атаки
        [Header("Attack settings")]
        [SerializeField] protected AttackHitbox _attackHitbox;

        protected Vector2 _moveDirection;
        protected Rigidbody2D _rigidbody;
        protected Animator _animator;
        protected PlaySoundsComponent _sounds;
        private int _jumpsLeft;
        private bool _jumpRequested;
        private bool _doubleJumpUsedThisAirborne;
        private float _timeInAir;
        private Transform _activePlatform; //Запоминаем текущую платформу

        protected static readonly int IsGround = Animator.StringToHash("is_ground");
        protected static readonly int IsRunning = Animator.StringToHash("is_running");
        protected static readonly int VerticalVelocity = Animator.StringToHash("vertical_velocity");
        protected static readonly int Hit = Animator.StringToHash("hit");
        protected static readonly int AttackKey = Animator.StringToHash("attack");
        protected static readonly int ThrowKey = Animator.StringToHash("throw");
        protected static readonly int Die = Animator.StringToHash("die");

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _sounds = GetComponent<PlaySoundsComponent>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void FixedUpdate()
        {
            //горизонтальное движение
            Move();

            //обработка прыжка и логики падения
            JumpCalc();
            LogicOfFalling();

            //обработка для анимаций
            _animator.SetBool(IsGround, IsGrounded());
            _animator.SetBool(IsRunning, _moveDirection.x != 0);
            _animator.SetFloat(VerticalVelocity, _rigidbody.velocity.y);

            //обработка направления спрайта существа
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

        protected virtual void Move()
        {
            _rigidbody.velocity = new Vector2(_moveDirection.x * MoveSpeed, _rigidbody.velocity.y);
        }

        protected bool IsGrounded()
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

        public virtual void SetMovementDirection(Vector2 direction)
        {
            _moveDirection = direction;
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
            if (_sounds != null) _sounds.PlayClip("Jump");
        }

        public void JumpRequest()
        {
            _jumpRequested = true;
        }

        protected virtual void LogicOfFalling()
        {
            bool isGrounded = IsGrounded();
            bool inTheAir = isGrounded ? false : true;

            if (inTheAir)
            {
                _timeInAir += Time.deltaTime;
            }

            if ((_doubleJumpUsedThisAirborne || _timeInAir > 2f) && isGrounded == true)
            {
                _particles.Spawn("Fall");
            }

            if (isGrounded)
            {
                _timeInAir = 0;
                _doubleJumpUsedThisAirborne = false;
            }
        }

        public virtual void TakeDamageSimple()
        {
            _animator.SetTrigger(Hit);
        }

        public virtual void TakeDamageFromSpikes()
        {
            _animator.SetTrigger(Hit);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }

        public virtual void TakeDamageFromExplosion()
        {
            _animator.SetTrigger(Hit);
        }

        public virtual void Attack()
        {
            _animator.SetTrigger(AttackKey);
            if (_sounds != null) _sounds.PlayClip("Melee");
        }

        public virtual void ThrowAttack(float holdTime)
        {
            _animator.SetTrigger(ThrowKey);
            _particles.Spawn("Throw");
            if (_sounds != null) _sounds.PlayClip("Range");
        }

        public virtual void PerformDamage()
        {
            if (_attackHitbox != null)
                _attackHitbox.Attack();
        }
    }
}
