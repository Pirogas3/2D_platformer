using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Creatures.Totems
{
    public class TotemHeadAI : MonoBehaviour
    {
        [SerializeField] protected LayerCheck _vision;
        [SerializeField] protected SpawnListComponent _spawner;

        protected Animator _animator;

        protected static readonly int Attacking = Animator.StringToHash("Attacking");

        protected void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected void Update()
        {
            if (_vision.IsTouchingLayer)
            {
                _animator.SetBool(Attacking, true);
            }
            else
            {
                _animator.SetBool(Attacking, false);
            }
        }
    }
}
