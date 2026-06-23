using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public enum AIState
    {
        Patrol,
        Chase,
        MeleeAttack,
        Cooldown,
        Idle // если нет патруля и цели
    }

    public class BaseAI : MonoBehaviour
    {
        [Header("Vision & Attack Ranges")]
        [SerializeField] protected LayerCheck _vision;
        [SerializeField] protected LayerCheck _meleeAttackRange;

        [Header("Patrol")]
        [SerializeField] protected Patrol _patrol;

        [Header("Timings")]
        [SerializeField] protected float _meleeAttackCooldown = 1.5f;
        [SerializeField] protected float _lostInterestTime = 3f;

        protected NewCreature _creature;
        protected Animator _animator;
        protected GameObject _target;
        protected AIState _currentState = AIState.Idle;
        protected float _stateTimer = 0f;
        protected float _cooldownTime = 0f;
        protected bool _isDead { get => _creature.IsDead; set => _creature.IsDead = value; }

        protected virtual void Awake()
        {
            _creature = GetComponent<NewCreature>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            SwitchState(AIState.Idle);
        }

        protected virtual void Update()
        {
            if (_isDead) return;
            _stateTimer += Time.deltaTime;

            switch (_currentState)
            {
                case AIState.Idle:
                    UpdateIdle();
                    break;
                case AIState.Patrol:
                    UpdatePatrol();
                    break;
                case AIState.Chase:
                    UpdateChase();
                    break;
                case AIState.MeleeAttack:
                    UpdateMeleeAttack();
                    break;
                case AIState.Cooldown:
                    UpdateCooldown();
                    break;
            }
        }

        // ---------- Методы входа в состояния ----------
        protected virtual void EnterIdle()
        {
            _creature.SetMovementDirection(Vector2.zero);
        }

        protected virtual void EnterPatrol()
        {
            _creature.SetMovementDirection(Vector2.zero);
            if (_patrol == null)
            {
                // Если патруля нет, переходим в Idle
                SwitchState(AIState.Idle);
            }
        }

        protected virtual void EnterChase()
        {
            // не останавливаем движение, направление обновится в UpdateChase
        }

        protected virtual void EnterMeleeAttack()
        {
            _creature.SetMovementDirection(Vector2.zero); // стоим перед атакой
        }

        protected virtual void EnterCooldown(float duration)
        {
            _creature.SetMovementDirection(Vector2.zero);
            _cooldownTime = duration;
        }

        // ---------- Методы обновления состояний ----------
        protected virtual void UpdateIdle()
        {
            if (_vision.IsTouchingLayer)
            {
                SwitchState(AIState.Chase);
            }
        }

        protected virtual void UpdatePatrol()
        {
            if (_vision.IsTouchingLayer)
            {
                SwitchState(AIState.Chase);
                return;
            }
            // Логика патрулирования (пока заглушка)
            _creature.SetMovementDirection(Vector2.zero);
        }

        protected virtual void UpdateChase()
        {
            Vector2 direction;
            if (_target == null)
            {
                SwitchState(AIState.Patrol);
                return;
            }

            if (!_vision.IsTouchingLayer)
            {
                if (_stateTimer > _lostInterestTime)
                {
                    _target = null;
                    SwitchState(AIState.Patrol);
                    return;
                }
                // Если потеряли из виду, но не прошло время потери интереса – двигаемся к цели
                direction = (_target.transform.position - transform.position).normalized;
                direction.y = 0;
                _creature.SetMovementDirection(direction);
                return;
            }

            // Цель видна – двигаемся к ней
            direction = (_target.transform.position - transform.position).normalized;
            direction.y = 0;
            _creature.SetMovementDirection(direction);

            // Проверяем радиус атаки
            if (_meleeAttackRange.IsTouchingLayer)
            {
                SwitchState(AIState.MeleeAttack);
            }
        }

        protected virtual void UpdateMeleeAttack()
        {
            if (_stateTimer > _meleeAttackCooldown)
            {
                if (_meleeAttackRange.IsTouchingLayer && _target != null)
                {
                    // Повторная атака
                    SwitchState(AIState.MeleeAttack);
                }
                else
                {
                    SwitchState(AIState.Chase);
                }
            }
        }

        protected virtual void UpdateCooldown()
        {
            if (_stateTimer >= _cooldownTime)
            {
                // Кулдаун закончился – решаем, куда перейти
                if (_target != null && _vision.IsTouchingLayer)
                {
                    if (_meleeAttackRange.IsTouchingLayer)
                        SwitchState(AIState.MeleeAttack);
                    else
                        SwitchState(AIState.Chase);
                }
                else
                {
                    _target = null;
                    SwitchState(AIState.Patrol);
                }
            }
        }

        // ---------- Метод переключения состояний ----------
        protected virtual void SwitchState(AIState newState)
        {
            _currentState = newState;
            _stateTimer = 0f;

            switch (newState)
            {
                case AIState.Idle: EnterIdle(); break;
                case AIState.Patrol: EnterPatrol(); break;
                case AIState.Chase: EnterChase(); break;
                case AIState.MeleeAttack: EnterMeleeAttack(); break;
                case AIState.Cooldown: EnterCooldown(_meleeAttackCooldown); break; // по умолчанию
            }
        }

        // ---------- Внешние события ----------
        public virtual void OnHeroInVision(GameObject target)
        {
            if (_isDead || _target != null) return;
            _target = target;
            if (_currentState != AIState.MeleeAttack && _currentState != AIState.Cooldown)
            {
                SwitchState(AIState.Chase);
            }
        }

        public virtual void OnDie()
        {
            _creature.SetMovementDirection(Vector2.zero);
            _creature.Die();
        }

        // Метод для принудительного перехода в кулдаун с указанием времени
        public virtual void SwitchToCooldown(float duration)
        {
            _currentState = AIState.Cooldown;
            _stateTimer = 0f;
            _cooldownTime = duration;
            EnterCooldown(duration);
        }

        // Метод для сброса кулдауна
        public virtual void CooldownReset()
        {
            if (_currentState == AIState.Cooldown)
            {
                _cooldownTime = 0f; // закончить кулдаун мгновенно
            }
        }
    }
}
