using Assets.Scripts.Components;
using Assets.Scripts.Model;
using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Creatures.Ability
{
    public class ShieldAbility : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _shieldDuration = 2f;
        [SerializeField] private float _cooldown = 15f;
        [SerializeField] private int _shieldDefenseBonus = 100;
        [SerializeField] private SpawnListComponent _particles;

        private bool _isShieldActive = false;
        private bool _isCooldown = false;
        private GameSession _gameSession;
        private Hero _hero;

        private void Start()
        {
            _gameSession = GameSession.Instance;
            _hero = GetComponent<Hero>();
        }

        public void ActivateShield()
        {
            if (_gameSession == null || _gameSession.PlayerData.PerkData.GetLevel("Shield") <= 0)
            {
                Debug.Log("Shield perk not unlocked!");
                return;
            }

            if (_isShieldActive || _isCooldown) return;

            _isShieldActive = true;
            if (_hero != null)
                _hero.defenseBonus = _shieldDefenseBonus;

            if (_particles != null)
                _particles.SpawnAsChild("Shield");

            StartCoroutine(ShieldRoutine());
        }

        private IEnumerator ShieldRoutine()
        {
            yield return new WaitForSeconds(_shieldDuration);
            if (_hero != null)
                _hero.defenseBonus -= _shieldDefenseBonus;
            _isShieldActive = false;
            _isCooldown = true;

            yield return new WaitForSeconds(_cooldown);
            Vector3 pos = transform.position + Vector3.up * 1f;
            FloatingTextManager.Instance.ShowFloatingText(
                "Shield ready!",
                pos,
                Color.green,
                duration: 2f
            );
            _isCooldown = false;
        }

        public bool IsShieldActive => _isShieldActive;
        public bool IsCooldown => _isCooldown;
    }
}
