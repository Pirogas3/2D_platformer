using UnityEngine;

namespace Assets.Scripts.UI.SettingsMenu
{
    public class SettingsMenuWindow : AnimatedWindow
    {
        private void Awake()
        {
            if (_windowsContainer == null)
                _windowsContainer = GameObject.Find("MenuContainer").transform;
        }

        public void OnShowAudioMenu()
        {
            var window = Resources.Load<GameObject>("UI/AudioSettingsWindow");

            if (_windowsContainer != null)
            {
                Instantiate(window, _windowsContainer);
                return;
            }

            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void OnShowLanguesMenu()
        {
            var window = Resources.Load<GameObject>("UI/LanguesSettingsWindow");

            if (_windowsContainer != null)
            {
                Instantiate(window, _windowsContainer);
                return;
            }

            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }
    }
}
