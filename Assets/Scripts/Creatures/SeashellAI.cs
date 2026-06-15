using Scripts.Creatures;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class SeashellAI : EnemyBotAI
    {
        protected override IEnumerator GoToHero()
        {
            StartState(Attacking());
            yield break;
        }

        protected override IEnumerator Attacking()
        {
            while (true)
            {
                // Если мёртв или цель пропала – выходим
                if (_isDead || _target == null || !_vision.IsTouchingLayer)
                {
                    _creature.SetMovementDirection(Vector2.zero);
                    yield return new WaitForSeconds(_alarmDelay);
                    StartState(Patrolling());
                    yield break;
                }

                // Если в радиусе ближней атаки – бьём
                if (_attackRange.IsTouchingLayer)
                {
                    _creature.SetMovementDirection(Vector2.zero);
                    _creature.Attack();
                }
                // Иначе если в поле зрения – стреляем
                else if (_vision.IsTouchingLayer)
                {
                    _creature.SetMovementDirection(Vector2.zero);
                    _creature.ThrowAttack(0f); // holdTime не используем
                }

                yield return new WaitForSeconds(_attackCooldown);
            }
        }

        protected override IEnumerator Patrolling()
        {
            while (true)
            {
                _creature.SetMovementDirection(Vector2.zero);
                yield return null;
            }
        }

        protected override void SetDirectionToTarget()
        {
            
        }

        public override void OnDie()
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _isDead = true;
            _animator.SetBool(IsDeadKey, _isDead);
            _particles.Spawn("Dead");
        }
    }
}
