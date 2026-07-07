using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.UI.LoadMenu
{
    public class LoadMenuWindow : AnimatedWindow
    {
        private GameSession _gameSession;

        protected override void Start()
        {
            base.Start();

            _gameSession = FindObjectOfType<GameSession>();
            if (_gameSession == null)
            {
                Debug.Log("GameSession не найден");
            }
        }

        public void LoadQuickSave()
        {
            _gameSession.QuickLoad();
        }
    }
}
