using Assets.Scripts.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private TooltipView _tooltipPrefab;

        private TooltipView _tooltipInstance;
        private bool _isVisible = false;

        public static TooltipManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Создаём экземпляр тултипа сразу и выключаем
            _tooltipInstance = Instantiate(_tooltipPrefab, transform);
            _tooltipInstance.gameObject.SetActive(false);
        }

        public void ShowTooltip(Vector2 position, string header, string description, string cost = "")
        {
            if (_tooltipInstance == null) return;

            _tooltipInstance.SetData(header, description, cost);
            _tooltipInstance.SetPosition(position);
            _tooltipInstance.gameObject.SetActive(true);
            _isVisible = true;
        }

        public void HideTooltip()
        {
            if (_tooltipInstance == null || !_isVisible) return;
            _tooltipInstance.gameObject.SetActive(false);
            _isVisible = false;
        }

        public bool IsVisible => _isVisible;
    }
}
