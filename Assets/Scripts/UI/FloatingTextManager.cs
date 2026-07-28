using Assets.Scripts.UI.Widgets;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class FloatingTextManager : MonoBehaviour
    {
        [SerializeField] private FloatingText _prefab;
        [SerializeField] private float _defaultDuration = 1.5f;
        [SerializeField] private Color _defaultColor = Color.white;

        public static FloatingTextManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowFloatingText(string message, Vector3 worldPosition, Color? color = null, float duration = -1)
        {
            if (_prefab == null)
            {
                Debug.LogError("FloatingTextManager: префаб не назначен!");
                return;
            }

            var instance = Instantiate(_prefab, worldPosition, Quaternion.identity);
            instance.Show(
                message,
                worldPosition,
                color ?? _defaultColor,
                duration > 0 ? duration : _defaultDuration
            );
        }
    }
}
