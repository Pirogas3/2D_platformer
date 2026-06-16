using UnityEngine;
using UnityEngine.InputSystem;
using Scripts.Creatures;

namespace Assets.Scripts.PlayerInput
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;

        private float _throwPressStartTime;
        private bool _isChargingThrow;

        public void OnMovement2D(InputAction.CallbackContext context)
        {
            Vector2 moveVector = context.ReadValue<Vector2>();

            _hero.SetMovementDirection(moveVector);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
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
                _hero.Attack();
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _isChargingThrow = true;
                _throwPressStartTime = Time.time;
            }
            else if (context.canceled && _isChargingThrow)
            {
                float holdTime = Time.time - _throwPressStartTime;
                _hero.ThrowAttack(holdTime);
                _isChargingThrow = false;
            }
        }
    }
}
