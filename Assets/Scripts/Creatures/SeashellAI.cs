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
            while (_vision.IsTouchingLayer && !_isDead)
            {
                if (_vision.IsTouchingLayer && !_attackRange.IsTouchingLayer)
                {
                    _creature.ThrowAttack(0f); //holdTime не используем
                }
                else if (_vision.IsTouchingLayer && _attackRange.IsTouchingLayer)
                {
                    _creature.Attack();
                }
                yield return new WaitForSeconds(_attackCooldown);
            }
            StartState(Patrolling());
        }

        protected override IEnumerator Patrolling()
        {
            while (true)
            {
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
