using Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class Seashell : Creature
    {
        protected override void FixedUpdate()
        {
            //ничего не надо делать, так как моб неподвижный
        }

        public override void ThrowAttack(float holdTime)
        {
            _animator.SetTrigger(ThrowKey);
        }

        public override void TakeDamageSimple()
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            base.TakeDamageSimple();
        }
    }
}
