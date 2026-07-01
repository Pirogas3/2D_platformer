using Assets.Scripts.Model.Data.Properties;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [CreateAssetMenu(menuName = "Data/GameSettings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private FloatPersistentProperty _Music;
        [SerializeField] private FloatPersistentProperty _SFX;

        public FloatPersistentProperty Music => _Music;
        public FloatPersistentProperty SFX => _SFX;

        private static GameSettings _instance;
        public static GameSettings I => _instance == null ? LoadGameSettings() : _instance;

        private void OnEnable()
        {
            _Music = new FloatPersistentProperty(1, SoundSetting.Music.ToString());
            _SFX = new FloatPersistentProperty(1, SoundSetting.SFX.ToString());
        }

        private static GameSettings LoadGameSettings()
        {
            return _instance = Resources.Load<GameSettings>("GameSettings");
        }

        private void OnValidate()
        {
            _Music.Validate();
            _SFX.Validate();
        }
    }

    public enum SoundSetting
    {
        Music,
        SFX
    }
}
