using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class AnimatedWindow : MonoBehaviour
    {
        protected Animator _animator;
        protected static readonly int Show = Animator.StringToHash("Show");
        protected static readonly int Hide = Animator.StringToHash("Hide");

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
            Destroy(gameObject);
        }
    }
}
