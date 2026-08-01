using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DraggableObject : MonoBehaviour
    {
        [SerializeField] private float _followSpeed = 8f;
        [SerializeField] private float _mouseFollowSpeed = 15f;
        [SerializeField] private float _dragGravityScale = 0f;

        private Rigidbody2D _rb;
        private bool _isDragging = false;
        private float _originalGravityScale;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
                _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _originalGravityScale = _rb.gravityScale;
        }

        public void StartDrag()
        {
            if (_isDragging) return;
            _isDragging = true;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = _dragGravityScale;
        }

        public void StopDrag()
        {
            if (!_isDragging) return;
            _isDragging = false;
            _rb.gravityScale = _originalGravityScale;
        }

        public void Drag(Vector2 targetPosition, bool useMouseSpeed = false)
        {
            if (!_isDragging) return;
            float speed = useMouseSpeed ? _mouseFollowSpeed : _followSpeed;
            Vector2 newPos = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            _rb.MovePosition(newPos);
        }

        public bool IsDragging => _isDragging;
    }
}
