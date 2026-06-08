using UnityEngine;
using UnityEngine.InputSystem;
using Scripts.Creatures;

namespace Scripts
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;

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
            if (context.canceled)
            {
                _hero.Interact();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _hero.Attack();
            }
        }
    }
}
