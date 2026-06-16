using UnityEngine;
using Assets.Scripts.Model.Data;

namespace Assets.Scripts.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        public PlayerData PlayerData => _playerData;

        private PlayerData _initialPlayerData;

        private void Awake()
        {
            if (IsSessionExit())
            {
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(this);

            _initialPlayerData = _playerData.Clone();
        }

        public void ResetToInitialState()
        {
            _playerData = _initialPlayerData.Clone();
        }

        private bool IsSessionExit()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var session in sessions)
            {
                if (session != this)
                    return true;
            }
            return false;
        }
    }
}
