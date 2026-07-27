using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// Маркер для объектов, состояние которых нужно сохранять (включен/выключен или удален).
    /// </summary>
    public class PersistentObjectState : MonoBehaviour
    {
        [SerializeField] protected string _uniqueId;
        [SerializeField] protected bool _destroyOnDeactivate = false; // true — удалить при загрузке, false — просто выключить
        [SerializeField] protected bool _isActiveOnStart = true;

        public string UniqueId => _uniqueId;
        public bool DestroyOnDeactivate => _destroyOnDeactivate;

        protected virtual void Awake()
        {
            // Если ID не задан, генерируем на основе позиции
            if (string.IsNullOrEmpty(_uniqueId))
                _uniqueId = $"{gameObject.name}_{transform.position.x:F1}_{transform.position.y:F1}";

            if (!_isActiveOnStart)
                gameObject.SetActive(false);
        }

        public virtual void RegisterState(bool isActive)
        {
            var session = GameSession.Instance;
            if (session != null)
            {
                session.PlayerData.EnviromentData.RegisterObjectState(_uniqueId, isActive);
            }
        }
    }
}
