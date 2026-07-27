using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Data.Properties;
using System;
using UnityEngine;

namespace Assets.Scripts.Components.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSettingComponent : MonoBehaviour
    {
        [SerializeField] private SoundSetting _mode;
        private AudioSource _source;
        private FloatPersistentProperty _model;

        private void Start()
        {
            _source = GetComponent<AudioSource>();

            _model = FindProperty();
            if (_model != null)
            {
                _model.OnChanged += OnSoundSettingChanged;
                OnSoundSettingChanged(_model.Value, _model.Value);
            }
            else
            {
                Debug.LogWarning($"AudioSettingComponent: модель для {_mode} не найдена. Громкость не будет применена.");
            }
        }

        private void OnSoundSettingChanged(float newValue, float oldValue)
        {
            if (_source != null)
                _source.volume = newValue;
        }

        private FloatPersistentProperty FindProperty()
        {
            var settings = GameSettings.I;
            if (settings == null)
            {
                Debug.LogError("GameSettings not found!");
                return null;
            }

            switch (_mode)
            {
                case SoundSetting.Music:
                    return settings.Music;
                case SoundSetting.SFX:
                    return settings.SFX;
                default:
                    Debug.LogError($"Unknown SoundSetting: {_mode}");
                    return null;
            }
        }

        private void OnDestroy()
        {
            if (_model != null)
                _model.OnChanged -= OnSoundSettingChanged;
        }
    }
}
