using Assets.Scripts.Creatures;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Widgets
{
    public class PerkSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _perkId;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private GameObject _boughtText;          // надпись "Куплен"
        [SerializeField] private GameObject _unlockedOverlay;     // оверлей при наведении (доступно)
        [SerializeField] private GameObject _notEnoughOverlay;    // оверлей при наведении (недостаточно ресурсов)

        private GameSession _session;
        private PlayerData _playerData;
        private bool _isPointerOver = false;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            if (_session == null) return;
            _playerData = _session.PlayerData;

            _playerData.Inventory.OnChanged += UpdateUI;
            UpdateUI();
        }

        private void OnDestroy()
        {
            if (_playerData?.Inventory != null)
                _playerData.Inventory.OnChanged -= UpdateUI;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerOver = true;
            UpdateUI();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerOver = false;
            UpdateUI();
        }

        public void OnBuyClick()
        {
            if (_playerData.PerkData.IsUnlocked(_perkId))
            {
                Debug.Log("Perk already unlocked!");
                return;
            }

            int cost = _session.GetPerkCost(_perkId);
            if (cost <= 0)
            {
                Debug.Log("Invalid perk cost!");
                return;
            }

            if (_playerData.Inventory.Count("GoldCoin") < cost)
            {
                Debug.Log("Not enough coins! Need " + cost);
                return;
            }

            _playerData.Inventory.Remove("GoldCoin", cost);
            _playerData.PerkData.Unlock(_perkId);
            ApplyPerkEffect();
            UpdateUI();
        }

        private void ApplyPerkEffect()
        {
            var hero = FindObjectOfType<Hero>();
            if (hero == null) return;

            if (_perkId == "DoubleJump")
                hero.SetExtraJumps(1);
            // Добавляйте другие перки по мере необходимости
        }

        private void UpdateUI()
        {
            bool isUnlocked = _playerData.PerkData.IsUnlocked(_perkId);
            int cost = _session.GetPerkCost(_perkId);
            int coins = _playerData.Inventory.Count("GoldCoin");
            bool canAfford = coins >= cost;

            // ---- Состояние "Куплен" ----
            if (isUnlocked)
            {
                if (_boughtText != null)
                    _boughtText.SetActive(true);
                if (_costText != null)
                {
                    _costText.text = string.Empty;
                    _costText.gameObject.SetActive(false);
                }
                if (_buyButton != null)
                    _buyButton.interactable = false;
                if (_unlockedOverlay != null)
                    _unlockedOverlay.SetActive(false);
                if (_notEnoughOverlay != null)
                    _notEnoughOverlay.SetActive(false);
                return;
            }

            // ---- Состояние "Не куплен" ----
            if (_boughtText != null)
                _boughtText.SetActive(false);
            if (_costText != null)
            {
                _costText.text = cost.ToString();
                _costText.gameObject.SetActive(true);
            }

            // Кнопка всегда интерактивна, если не куплена
            if (_buyButton != null)
                _buyButton.interactable = true;

            // ---- Оверлеи при наведении ----
            if (_isPointerOver)
            {
                if (canAfford)
                {
                    if (_unlockedOverlay != null)
                        _unlockedOverlay.SetActive(true);
                    if (_notEnoughOverlay != null)
                        _notEnoughOverlay.SetActive(false);
                }
                else
                {
                    if (_unlockedOverlay != null)
                        _unlockedOverlay.SetActive(false);
                    if (_notEnoughOverlay != null)
                        _notEnoughOverlay.SetActive(true);
                }
            }
            else
            {
                if (_unlockedOverlay != null)
                    _unlockedOverlay.SetActive(false);
                if (_notEnoughOverlay != null)
                    _notEnoughOverlay.SetActive(false);
            }
        }
    }
}
