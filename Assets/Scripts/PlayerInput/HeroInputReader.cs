using Assets.Scripts.Creatures;
using Assets.Scripts.Creatures.Ability;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts.PlayerInput
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        [SerializeField] private float _meleeAttackCooldown = 0.3f; // задержка между атаками
        [SerializeField] private ShieldAbility _shieldAbility;

        private float _throwPressStartTime;
        private bool _isChargingThrow;
        private float _lastAttackTime;
        private Coroutine _chargeCheckRoutine;
        private bool _multiShotChargedNotified = false;

        public void OnMovement2D(InputAction.CallbackContext context)
        {
            Vector2 moveVector = context.ReadValue<Vector2>();

            _hero.SetMovementDirection(moveVector);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                return;

            if (context.performed)
                _hero.JumpRequest();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _hero.Interact();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Если курсор над UI – игнорируем атаку
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                // Проверяем кулдаун
                if (Time.time - _lastAttackTime < _meleeAttackCooldown)
                    return;

                _hero.Attack();
                _lastAttackTime = Time.time;
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isChargingThrow = true;
                _throwPressStartTime = Time.time;
                _multiShotChargedNotified = false;
                // Запускаем корутину для отслеживания заряда
                _chargeCheckRoutine = StartCoroutine(ChargeCheckRoutine());
            }
            else if (context.canceled && _isChargingThrow)
            {
                // Останавливаем корутину
                if (_chargeCheckRoutine != null)
                    StopCoroutine(_chargeCheckRoutine);
                _chargeCheckRoutine = null;

                // Если курсор над UI – игнорируем бросок
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    _isChargingThrow = false;
                    _multiShotChargedNotified = false;
                    return;
                }
                float holdTime = Time.time - _throwPressStartTime;
                _hero.ThrowAttack(holdTime);
                _isChargingThrow = false;
                _multiShotChargedNotified = false;
            }
        }

        private IEnumerator ChargeCheckRoutine()
        {
            while (_isChargingThrow)
            {
                float holdTime = Time.time - _throwPressStartTime;
                // Когда заряд достиг 1 секунды и ещё не было уведомления
                if (holdTime >= 1.0f && !_multiShotChargedNotified)
                {
                    _hero.ShowMultiShotCharged();
                    _multiShotChargedNotified = true;
                }
                yield return new WaitForSeconds(0.1f); // проверяем каждые 0.1 сек
            }
        }

        public void OnUseQuickSlot(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.UseQuickSlot();
        }

        public void OnDrag(InputAction.CallbackContext context)
        {
            if (context.performed)
                _hero.StartDragging();
            else if (context.canceled)
                _hero.StopDragging();
        }

        public void OnShield(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_shieldAbility != null)
                    _shieldAbility.ActivateShield();
                else
                    Debug.LogWarning("ShieldAbility not assigned!");
            }
        }
    }
}
