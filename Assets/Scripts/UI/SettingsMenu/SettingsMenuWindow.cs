using UnityEngine;

namespace Assets.Scripts.UI.SettingsMenu
{
    public class SettingsMenuWindow : AnimatedWindow
    {
        public void OnShowAudioMenu()
        {
            var window = Resources.Load<GameObject>("UI/AudioSettingsWindow");
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void OnShowLanguesMenu()
        {
            var window = Resources.Load<GameObject>("UI/LanguesSettingsWindow");
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }
    }
}
