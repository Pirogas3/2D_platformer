using Assets.Scripts.UI.Hud;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class AnimatedWindow : MonoBehaviour
    {
        [SerializeField] protected Transform _windowsContainer;
        protected EscController _escController;

        protected Animator _animator;
        protected static readonly int Show = Animator.StringToHash("Show");
        protected static readonly int Hide = Animator.StringToHash("Hide");

        protected virtual void Awake()
        {
            _escController = GetComponentInParent<EscController>();
            if (_escController != null)
                _escController.RegisterWindow(gameObject);
        }

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();

            _animator.SetTrigger(Show);
        }

        public void Close()
        {
            _animator.SetTrigger(Hide);
        }

        public virtual void OnCloseAnimationComplete()
        {
            if (_escController != null)
            {
                _escController.CloseWindow(gameObject);
                return;
            }

            Destroy(gameObject);
            EventSystem.current?.SetSelectedGameObject(null);
        }
    }
}
