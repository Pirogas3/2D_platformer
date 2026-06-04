using UnityEngine;

namespace Assets.Scripts.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        public PlayerData PlayerData => _playerData;

        private void Awake()
        {
            if (IsSessionExit())
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
            }
        }

        private bool IsSessionExit()
        {
            var session = FindObjectsOfType<GameSession>();
            foreach (var gameSession in session)
            {
                if (gameSession != this)
                    return true;
            }

            return false;
        }
    }
}
