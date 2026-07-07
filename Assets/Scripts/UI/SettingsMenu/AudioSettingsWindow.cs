using Assets.Scripts.Model.Data;
using Assets.Scripts.UI.Widgets;
using UnityEngine;

namespace Assets.Scripts.UI.SettingsMenu
{
    public class AudioSettingsWindow : AnimatedWindow
    {
        [SerializeField] private AudioSettingsWidget _music;
        [SerializeField] private AudioSettingsWidget _sfx;

        protected override void Start()
        {
            base.Start();

            _music.SetModel(GameSettings.I.Music);
            _sfx.SetModel(GameSettings.I.SFX);
        }
    }
}
