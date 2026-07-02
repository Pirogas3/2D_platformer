using Assets.Scripts.Model;
using Assets.Scripts.UI.Widgets;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    public class HealthHudController : MonoBehaviour
    {
        [SerializeField] private ProgresBarWidget _healthBar;
        private GameSession _session;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            _session.PlayerData.HpOnChanged += OnHealthChanged;

            OnHealthChanged(_session.PlayerData.Hp);
        }

        private void OnHealthChanged(int newValue)
        {
            var maxHealth = _session.PlayerData.MaxHp;
            var value = (float) newValue / maxHealth;
            _healthBar.SetProgress(value);
        }

        private void OnDestroy()
        {
            _session.PlayerData.HpOnChanged -= OnHealthChanged;
        }
    }
}
