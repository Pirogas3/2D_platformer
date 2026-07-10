using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class CheckPoint : InteractableComponent
    {
        [SerializeField] protected int _id;
        [SerializeField] protected SpriteAnimationComponent _animation;

        protected bool _isActivated = false;
        protected GameSession _gameSession;

        protected void Awake()
        {
            if (_animation == null)
            {
                _animation = GetComponent<SpriteAnimationComponent>();
            }
        }

        protected void Start()
        {
            _gameSession = FindObjectOfType<GameSession>();
            if (_gameSession == null)
            {
                Debug.LogError("GameSession not found!");
                return;
            }

            _isActivated = _gameSession.PlayerData.EnviromentData.IsCheckPointActivated(_id);

            if (_isActivated)
            {
                _animation.Play("Idle");
                Debug.Log("Работает?");
            }
            else
                _animation.Play("NoFlag");
        }

        public override void Interact(GameObject target)
        {
            if (_isActivated)
            {
                Debug.Log("Checkpoint already activated.");
                return;
            }

            _gameSession.PlayerData.EnviromentData.ActivateCheckPoint(_id);
            _isActivated = true;

            _gameSession.AutoSave();
            _action?.Invoke(target);
        }
    }
}
