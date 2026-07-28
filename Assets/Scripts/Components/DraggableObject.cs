using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DraggableObject : MonoBehaviour
    {
        [SerializeField] private float _followSpeed = 8f;
        [SerializeField] private float _checkRadius = 0.3f;
        [SerializeField] private Vector2 _checkOffset = Vector2.zero;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _dragGravityScale = 0.1f; // гравитация во время перетаскивания

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

        public void Drag(Vector2 targetPosition)
        {
            if (!_isDragging) return;

            Vector2 checkPos = (Vector2)transform.position + _checkOffset;
            if (!IsPositionFree(targetPosition))
            {
                Vector2 targetX = new Vector2(targetPosition.x, transform.position.y);
                if (IsPositionFree(targetX))
                    targetPosition = targetX;
                else
                {
                    Vector2 targetY = new Vector2(transform.position.x, targetPosition.y);
                    if (IsPositionFree(targetY))
                        targetPosition = targetY;
                    else
                        return;
                }
            }

            Vector2 newPos = Vector2.MoveTowards(transform.position, targetPosition, _followSpeed * Time.deltaTime);
            _rb.MovePosition(newPos);
        }

        private bool IsPositionFree(Vector2 position)
        {
            Vector2 checkPos = position + _checkOffset;
            Collider2D hit = Physics2D.OverlapCircle(checkPos, _checkRadius, _obstacleMask);
            return hit == null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 checkPos = (Vector2)transform.position + _checkOffset;
            Gizmos.DrawWireSphere(checkPos, _checkRadius);
        }
    }
}
