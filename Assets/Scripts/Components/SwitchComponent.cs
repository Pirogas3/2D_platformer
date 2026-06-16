using UnityEngine;

namespace Scripts.Components
{
    public class SwitchComponent : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _state; //вкл или выкл, true or false
        [SerializeField] private string _animationKey;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                if(_animator == null) Debug.LogError($"На объект {name} не назначен {_animator}");
            }
            if (_animationKey == null)
            {
                Debug.LogError($"На объект {name} не назначен {_animationKey}");
            }
        }

        private void Start()
        {
            _animator.SetBool(_animationKey, _state);
        }

        [ContextMenu("Switch")]
        public void Switch()
        {
            _state = !_state;
            _animator.SetBool(_animationKey, _state);
        }
    }
}
