using Assets.Scripts.Components;
using Assets.Scripts.Creatures;
using System.Collections;
using UnityEngine;

namespace Scripts.Creatures
{
    public class EnemyBotAI : MonoBehaviour
    {
        [SerializeField] protected LayerCheck _vision;
        [SerializeField] protected LayerCheck _attackRange;
        [SerializeField] protected Patrol _patrol;

        [SerializeField] protected float _alarmDelay = 0.5f;
        [SerializeField] protected float _attackCooldown = 1f;

        protected Coroutine _currentCoroutine;
        protected GameObject _target;
        protected SpawnListComponent _particles;
        protected Creature _creature;
        protected Animator _animator;
        protected bool _isDead = false;

        protected static readonly int IsDeadKey = Animator.StringToHash("die");

        protected virtual void Awake()
        {
            _particles = GetComponent<SpawnListComponent>();
            _creature = GetComponent<Creature>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            StartState(Patrolling());
        }

        protected virtual IEnumerator Patrolling()
        {
            while (true)
            {
                if (_patrol != null)
                {
                    yield return _patrol.DoPatrol(_creature);
                }
                else
                {
                    yield return null;
                }
            }
        }

        protected void StartState(IEnumerator coroutine)
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _currentCoroutine = StartCoroutine(coroutine);
        }

        protected virtual IEnumerator AgroToHero()
        {
            _particles.Spawn("Exclamation");
            yield return new WaitForSeconds(_alarmDelay);
            StartState(GoToHero());
        }

        protected virtual IEnumerator GoToHero()
        {
            while (_vision.IsTouchingLayer)
            {
                if (_attackRange.IsTouchingLayer)
                {
                    StartState(Attacking());
                }
                else
                {
                    SetDirectionToTarget();
                }
                yield return null;
            }
            yield return new WaitForSeconds(_alarmDelay);

            _creature.SetMovementDirection(Vector2.zero);
            yield return new WaitForSeconds(_alarmDelay);

            StartState(Patrolling());
        }

        protected virtual IEnumerator Attacking()
        {
            while (_attackRange.IsTouchingLayer)
            {
                _creature.SetMovementDirection(Vector2.zero);
                _creature.Attack();
                yield return new WaitForSeconds(_attackCooldown);
            }

            if (_vision.IsTouchingLayer)
            {
                StartState(GoToHero());
            }
            else
            {
                _creature.SetMovementDirection(Vector2.zero);
                yield return new WaitForSeconds(_alarmDelay);
                StartState(Patrolling());
            }
        }

        protected virtual void SetDirectionToTarget()
        {
            var direction = _target.transform.position - transform.position;
            direction.y = 0;
            _creature.SetMovementDirection(direction.normalized);
        }

        public virtual void OnHeroInVision(GameObject target)
        {
            if (_isDead)
                return;

            _target = target;

            StartState(AgroToHero());
        }

        public virtual void OnDie()
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _isDead = true;
            _animator.SetTrigger(IsDeadKey);
        }
    }
}
