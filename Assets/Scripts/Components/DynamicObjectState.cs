using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DynamicObjectState : PersistentObjectState
    {
        private bool _isRegistered = false;

        protected virtual void Start()
        {
            var session = GameSession.Instance;
            if (session != null)
            {
                // Подписываемся на событие сохранения
                session.OnSaved += OnSaved;

                // Восстанавливаем позицию
                Vector2 savedPos = session.PlayerData.EnviromentData.GetPosObjectState(_uniqueId);
                if (savedPos != Vector2.zero)
                {
                    transform.position = new Vector3(savedPos.x, savedPos.y, 0f);
                }

                // Регистрируем текущее состояние при старте
                RegisterState(gameObject.activeSelf);
                _isRegistered = true;
            }
        }

        protected virtual void OnSaved()
        {
            if (_isRegistered)
            {
                RegisterState(gameObject.activeSelf);
            }
        }

        public override void RegisterState(bool isActive)
        {
            var session = GameSession.Instance;
            if (session != null)
            {
                session.PlayerData.EnviromentData.RegisterObjectState(_uniqueId, isActive, transform.position.x, transform.position.y);
            }
        }

        protected virtual void OnDestroy()
        {
            var session = GameSession.Instance;
            if (session != null)
            {
                session.OnSaved -= OnSaved;
            }
        }
    }
}
