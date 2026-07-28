using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.PlayerInput
{
    /// <summary>
    /// Обрабатывает глобальные горячие клавиши (не привязанные к герою).
    /// </summary>
    public class GlobalInputHandler : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference _quickSaveAction;
        [SerializeField] private InputActionReference _quickLoadAction;

        private void OnEnable()
        {
            if (_quickSaveAction != null)
                _quickSaveAction.action.performed += OnQuickSavePerformed;

            if (_quickLoadAction != null)
                _quickLoadAction.action.performed += OnQuickLoadPerformed;
        }

        private void OnDisable()
        {
            if (_quickSaveAction != null)
                _quickSaveAction.action.performed -= OnQuickSavePerformed;

            if (_quickLoadAction != null)
                _quickLoadAction.action.performed -= OnQuickLoadPerformed;
        }

        private void OnQuickSavePerformed(InputAction.CallbackContext context)
        {
            var session = GameSession.Instance;
            if (session != null)
            {
                session.QuickSave();
                Debug.Log("QuickSave triggered (F5)");
            }
        }

        private void OnQuickLoadPerformed(InputAction.CallbackContext context)
        {
            var session = GameSession.Instance;
            if (session != null)
            {
                session.QuickLoad();
                Debug.Log("QuickLoad triggered (F9)");
            }
        }
    }
}
