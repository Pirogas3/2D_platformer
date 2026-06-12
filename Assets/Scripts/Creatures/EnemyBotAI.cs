using Assets.Scripts.Components;
using Assets.Scripts.Creatures;
using System.Collections;
using UnityEngine;

namespace Scripts.Creatures
{
    public class EnemyBotAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private LayerCheck _attackRange;
        [SerializeField] private Patrol _patrol;

        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _attackCooldown = 1f;

        private Coroutine _currentCoroutine;
        private GameObject _target;
        private SpawnListComponent _particles;
        private Creature _creature;
        private Animator _animator;
        private bool _isDead = false;

        private static readonly int IsDeadKey = Animator.StringToHash("is_dead");

        private void Awake()
        {
            _particles = GetComponent<SpawnListComponent>();
            _creature = GetComponent<Creature>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            StartState(Patrolling());
        }

        private IEnumerator Patrolling()
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

        private void StartState(IEnumerator coroutine)
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _currentCoroutine = StartCoroutine(coroutine);
        }

        private IEnumerator AgroToHero()
        {
            _particles.Spawn("Exclamation");
            yield return new WaitForSeconds(_alarmDelay);
            StartState(GoToHero());
        }

        private IEnumerator GoToHero()
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

        private IEnumerator Attacking()
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

        private void SetDirectionToTarget()
        {
            var direction = _target.transform.position - transform.position;
            direction.y = 0;
            _creature.SetMovementDirection(direction.normalized);
        }

        public void OnHeroInVision(GameObject target)
        {
            if (_isDead)
                return;

            _target = target;

            StartState(AgroToHero());
        }

        public void OnDie()
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _isDead = true;
            _animator.SetBool(IsDeadKey, _isDead);
        }
    }
}
