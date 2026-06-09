using Assets.Scripts.Components;
using System.Collections;
using UnityEngine;

namespace Scripts.Creatures
{
    public class EnemyBotAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private LayerCheck _attackRange;

        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _attackCooldown = 1f;

        private Coroutine _currentCoroutine;
        private GameObject _target;
        private SpawnListComponent _particles;
        private Creature _creature;

        private void Awake()
        {
            _particles = GetComponent<SpawnListComponent>();
            _creature = GetComponent<Creature>();
        }

        private void Start()
        {
            StartState(Patrolling());
        }

        private IEnumerator Patrolling()
        {
            yield return null;
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
                    StartState(Attack());
                }
                else
                {
                    SetDirectionToTarget();
                }
                yield return null;
            }
        }

        private IEnumerator Attack()
        {
            while (_attackRange.IsTouchingLayer)
            {
                Vector2 stopMove = Vector2.zero;
                _creature.SetMovementDirection(stopMove);
                _creature.Attack();
                yield return new WaitForSeconds(_attackCooldown);
            }

            if (_vision.IsTouchingLayer)
            {
                StartState(GoToHero());
            }
            else
            {
                Vector2 stopMove = Vector2.zero;
                _creature.SetMovementDirection(stopMove);
                StartState(Patrolling());
            }
        }

        private void SetDirectionToTarget()
        {
            var direction = _target.transform.position - transform.position;
            direction.y = 0;
            _creature.SetMovementDirection(direction);
        }

        public void OnHeroInVision(GameObject target)
        {
            _target = target;

            StartState(AgroToHero());
        }
    }
}
