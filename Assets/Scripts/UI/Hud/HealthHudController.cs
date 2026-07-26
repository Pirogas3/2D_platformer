using Assets.Scripts.Model;
using Assets.Scripts.UI.Widgets;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hud
{
    public class HealthHudController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ProgresBarWidget _healthBar;
        [SerializeField] private Text _healthValue;
        private GameSession _session;

        private void Start()
        {
            _session = GameSession.Instance;
            _session.PlayerData.HpOnChanged += OnHealthChanged;

            OnHealthChanged(_session.PlayerData.Hp);
        }

        private void OnDestroy()
        {
            _session.PlayerData.HpOnChanged -= OnHealthChanged;
        }

        // --- Клик ---
        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(ShowHealthValue());
        }

        private IEnumerator ShowHealthValue()
        {
            _healthValue.enabled = true;
            yield return new WaitForSeconds(2);
            _healthValue.enabled = false;
        }

        private void OnHealthChanged(int newValue)
        {
            var maxHealth = _session.PlayerData.MaxHp;
            var value = (float) newValue / maxHealth;
            _healthBar.SetProgress(value);
            _healthValue.text = newValue.ToString();
        }
    }
}
