using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Widgets
{
    public enum ScrollDirection
    {
        Up,
        Down
    }

    /// <summary>
    /// Прокручивает ScrollRect при зажатии кнопки.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Target")]
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Settings")]
        [SerializeField] private ScrollDirection _direction = ScrollDirection.Down;
        [SerializeField] private float _scrollSpeed = 0.5f; // скорость прокрутки (чем выше, тем быстрее)

        private bool _isPressed = false;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_scrollRect == null)
                _scrollRect = GetComponentInParent<ScrollRect>();

            if (_scrollRect == null)
                Debug.LogError("ScrollRect not found!", this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            _isPressed = true;
            StartCoroutine(ScrollRoutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPressed = false;
        }

        private System.Collections.IEnumerator ScrollRoutine()
        {
            float direction = _direction == ScrollDirection.Up ? 1f : -1f;
            while (_isPressed && _button.interactable)
            {
                _scrollRect.verticalNormalizedPosition += direction * _scrollSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
