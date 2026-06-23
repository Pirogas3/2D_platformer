using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class PinkStar : NewCreature
    {
        [Header("PinkStar Specific")]
        [SerializeField] protected float _chargeSpeedMultiplier = 1.5f;
        [SerializeField] protected Collider2D _chargeCollider;

        protected bool _isCharging = false;
        protected Vector2 _chargeDirection = Vector2.zero;
        public Vector2 ChargeDirection { get => _chargeDirection; set => _chargeDirection = value; }

        protected static readonly int IsCharg = Animator.StringToHash("is_charge");
        public bool IsCharging => _isCharging;

        protected override void Start()
        {
            base.Start();
            _chargeCollider.enabled = false;
        }

        // Метод для запуска заряда (вызывается из AI)
        public void StartCharge()
        {
            _isCharging = true;
            _chargeCollider.enabled = true; // включаем коллайдер для обработки столкновений и нанесения урона
            _animator.SetBool(IsCharg, true); // запускаем анимацию чарджа
        }

        // Метод для остановки заряда (вызывается из AI по истечении времени чарджа или при столкновении)
        public void StopCharge()
        {
            _particles.Spawn("Exclamation");
            _chargeDirection = Vector2.zero;
            _isCharging = false;
            _chargeCollider.enabled = false; // выключаем коллайдер для обработки столкновений и нанесения урона
            _animator.SetBool(IsCharg, false); // остановить анимацию чарджа
        }

        protected override void Move()
        {
            if (_isCharging)
            {
                // Во время чарджа двигаемся с ускоренной скоростью в фиксированном направлении
                _rigidbody.velocity = new Vector2(_chargeDirection.x * _speed * _chargeSpeedMultiplier, _rigidbody.velocity.y);
            }
            else
            {
                base.Move();
            }
        }

        public override void SetMovementDirection(Vector2 direction)
        {
            if (_isCharging) return; // Не меняем направление во время чарджа
            base.SetMovementDirection(direction);
        }

        // В общем теперь он сразу переходит к чарджу, без подготовки
        public override void MeleeAttack()
        {
            StartCharge();
            _sounds?.PlayClip("charge");
        }

        public override void TakeDamageSimple()
        {
            _animator.SetTrigger(HitKey);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * (_jumpPower / 2), ForceMode2D.Impulse);
        }
    }
}
