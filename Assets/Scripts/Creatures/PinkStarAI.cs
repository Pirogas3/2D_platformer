using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class PinkStarAI : BaseAI
    {
        [Header("Charge Settings")]
        [SerializeField] private float _chargeDuration = 2f;

        private PinkStar _pinkStar;
        private bool _isCharging = false;
        private bool _chargeInterrupted = false;

        protected override void Awake()
        {
            base.Awake();
            _pinkStar = GetComponent<PinkStar>();
        }

        protected override void EnterMeleeAttack()
        {
            base.EnterMeleeAttack();
            // Фиксируем направление заряда
            if (_target != null)
                _pinkStar.ChargeDirection = (_target.transform.position - transform.position).normalized;
            // Запускаем заряд
            _pinkStar.StartCharge();
            _isCharging = true;
            _chargeInterrupted = false;
        }

        protected override void UpdateMeleeAttack()
        {
            // Если заряд прерван извне
            if (_chargeInterrupted)
            {
                StopChargeAndCooldown();
                return;
            }

            // Проверяем, не остановился ли заряд сам (например, из-за коллайдера)
            if (_isCharging && !_pinkStar.IsCharging)
            {
                // Заряд остановлен извне (например, коллайдер вызвал StopCharge)
                StopChargeAndCooldown();
                return;
            }

            // Проверяем таймаут
            if (_stateTimer >= _chargeDuration)
            {
                StopChargeAndCooldown();
                return;
            }

            // Если заряд всё ещё идёт, но цель потеряна – прерываем
            if (_target == null || !_vision.IsTouchingLayer)
            {
                StopChargeAndCooldown();
                return;
            }
        }

        // Метод для прерывания заряда извне (столкновение, урон)
        public void OnChargeInterrupted()
        {
            if (!_isCharging) return;
            _chargeInterrupted = true;
        }

        // Остановка заряда и переход в кулдаун
        private void StopChargeAndCooldown()
        {
            if (!_isCharging) return;
            _isCharging = false;
            if (_pinkStar.IsCharging)
                _pinkStar.StopCharge();
            SwitchToCooldown(_meleeAttackCooldown);
        }

        // Переопределяем вход в кулдаун, чтобы гарантированно остановить заряд
        protected override void EnterCooldown(float duration)
        {
            base.EnterCooldown(duration);
            if (_pinkStar.IsCharging)
                _pinkStar.StopCharge();
            _isCharging = false;
            _chargeInterrupted = false;
        }
    }
}