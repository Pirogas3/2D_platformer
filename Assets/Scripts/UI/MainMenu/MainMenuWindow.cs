using Assets.Scripts.Model;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuWindow : AnimatedWindow
    {
        private Action _closeAction;

        protected override void Awake()
        {
            base.Awake();

            if (_windowsContainer == null)
                _windowsContainer = GameObject.Find("MenuContainer").transform;
        }

        public void OnShowSettingsMenu()
        {
            var window = Resources.Load<GameObject>("UI/SettingsWindow");

            if (_windowsContainer != null)
            {
                Instantiate(window, _windowsContainer);
                return;
            }

            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void OnShowLoadMenu()
        {
            var window = Resources.Load<GameObject>("UI/LoadWindow");

            if (_windowsContainer != null)
            {
                Instantiate(window, _windowsContainer);
                return;
            }

            var canvas = FindObjectOfType<Canvas>();
            Instantiate(window, canvas.transform);
        }

        public void StartNewGame()
        {
            _closeAction = () =>
            {
                var session = GameSession.Instance;
                if (session != null)
                {
                    session.ResetPlayerData();
                }

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
