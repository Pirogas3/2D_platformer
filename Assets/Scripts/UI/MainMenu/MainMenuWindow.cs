using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuWindow : AnimatedWindow
    {
        private Action _closeAction;

        public void OnShowSettingsMenu()
        {
            var window = Resources.Load<GameObject>("UI/SettingsWindow");
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void OnShowLoadMenu()
        {
            var window = Resources.Load<GameObject>("UI/LoadWindow");
            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void StartNewGame()
        {
            _closeAction = () =>
            {
                SceneManager.LoadScene("SceneGame1");
            };
            Close();
        }

        public void LoadGame()
        {

        }

        public void OnExit()
        {
            _closeAction = () =>
            {
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            };
            Close();
        }

        public override void OnCloseAnimationComplete()
        {
            base.OnCloseAnimationComplete();
            _closeAction?.Invoke();
        }
    }
}
