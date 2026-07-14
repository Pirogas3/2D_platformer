using UnityEngine;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// Уничтожает объект через указанное время после его активации.
    /// Полезно для снарядов, эффектов, одноразовых объектов.
    /// </summary>
    public class DestroyAfterTime : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _delay = 2f; // время до уничтожения

        [Header("Optional")]
        [SerializeField] private bool _destroyOnDisable = false; // если true, уничтожает при отключении

        private void OnEnable()
        {
            if (_delay > 0)
                Invoke(nameof(DestroyObject), _delay);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(DestroyObject));
            if (_destroyOnDisable)
                DestroyObject();
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(DestroyObject));
        }

        /// <summary>
        /// Немедленно уничтожить объект.
        /// </summary>
        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Перезапустить таймер с новым временем.
        /// </summary>
        public void ResetTimer(float newDelay = -1)
        {
            CancelInvoke(nameof(DestroyObject));
            if (newDelay > 0)
                _delay = newDelay;
            if (_delay > 0 && gameObject.activeInHierarchy)
                Invoke(nameof(DestroyObject), _delay);
        }

#if UNITY_EDITOR
        [ContextMenu("Destroy Now")]
        private void DestroyNowInEditor()
        {
            DestroyObject();
        }
#endif
    }
}
