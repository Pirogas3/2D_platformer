using Assets.Scripts.Components;
using Assets.Scripts.Model;
using Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Creatures
{
    public class Creature : MonoBehaviour
    {
        //Партикл анимации
        [Header("Particles")]
        [SerializeField] private SpawnListComponent _particles;

        //Скорость существа, сила прыжка и макс. кол. доп прыжков
        [Header("Movement settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpPower;
        [SerializeField] private int _maxExtraJumps;

        //Чекеры
        [Header("Checkers")]
        [SerializeField] private LayerCheck _groundCheck;

        //Настройка атаки
        [Header("Attack settings")]
        [SerializeField] private int _attackDamage; //можно и не задавать, если использовать DamageComponent на оружие или существе
        [SerializeField] private AttackHitbox _attackHitbox;

        private Vector2 _moveDirection;
        private Rigidbody2D _rigidbody;
        protected Animator _animator;
        private int _jumpsLeft;
        private bool _jumpRequested;
        private bool _doubleJumpUsedThisAirborne;
        private float _timeInAir;

        private static readonly int IsGround = Animator.StringToHash("is_ground");
        private static readonly int IsRunning = Animator.StringToHash("is_running");
        private static readonly int VerticalVelocity = Animator.StringToHash("vertical_velocity");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void FixedUpdate()
        {
            //горизонтальное движение
            _rigidbody.velocity = new Vector2(_moveDirection.x * _speed, _rigidbody.velocity.y);

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

        public void SetMovementDirection(Vector2 direction)
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
        }

        public void JumpRequest()
        {
            _jumpRequested = true;
        }

        protected virtual void LogicOfFalling()
        {
            if (gameObject.tag == "Player")
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
            else
            {

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
        }

        public void PerformDamage()
        {
            if (_attackHitbox != null)
                _attackHitbox.Attack(_attackDamage);
        }
    }
}
