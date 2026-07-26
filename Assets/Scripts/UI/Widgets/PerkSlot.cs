using Assets.Scripts.Creatures;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
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
        [SerializeField] private GameObject _boughtText;          // надпись "Макс." или "Куплен"
        [SerializeField] private GameObject _levelTextObject;     // объект с текстом уровня (опционально)
        [SerializeField] private TextMeshProUGUI _levelText;      // текст уровня
        [SerializeField] private GameObject _unlockedOverlay;     // оверлей при наведении (доступно)
        [SerializeField] private GameObject _notEnoughOverlay;    // оверлей при наведении (недостаточно ресурсов)

        private GameSession _session;
        private PlayerData _playerData;
        private bool _isPointerOver = false;
        private PerkDef _perkDef;

        public string GetPerkId() => _perkId;

        private void Start()
        {
            _session = GameSession.Instance;
            if (_session == null) return;
            _playerData = _session.PlayerData;

            _perkDef = DefsFacade.Instance.Perks.Get(_perkId);
            if (_perkDef == null || _perkDef.IsVoid)
            {
                Debug.LogError($"Perk with id '{_perkId}' not found in PerksDef!");
                return;
            }

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
            if (_perkDef == null) return;

            int currentLevel = _playerData.PerkData.GetLevel(_perkId);
            if (currentLevel >= _perkDef.MaxLevel)
            {
                Debug.Log("Perk already at max level!");
                return;
            }

            int cost = _perkDef.Cost;
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

            // Снимаем монеты
            _playerData.Inventory.Remove("GoldCoin", cost);
            // Увеличиваем уровень перка
            _playerData.PerkData.AddLevel(_perkId, 1);
            // Применяем эффект
            ApplyPerkEffect();
            // Обновляем UI
            UpdateUI();
        }

        private void ApplyPerkEffect()
        {
            var hero = FindObjectOfType<Hero>();
            if (hero == null) return;

            // Для каждого типа перка своя логика
            // Например, DoubleJump увеличивает количество дополнительных прыжков на 1 за каждый уровень
            switch (_perkId)
            {
                case "DoubleJump":
                    hero.AddExtraJump(1);
                    break;
            }
        }

        private void UpdateUI()
        {
            if (_perkDef == null) return;

            int currentLevel = _playerData.PerkData.GetLevel(_perkId);
            int maxLevel = _perkDef.MaxLevel;
            int cost = _perkDef.Cost;
            int coins = _playerData.Inventory.Count("GoldCoin");
            bool canAfford = coins >= cost;

            bool isMaxLevel = currentLevel >= maxLevel;

            // ---- Состояние "Максимальный уровень" ----
            if (isMaxLevel)
            {
                if (_boughtText != null)
                {
                    _boughtText.SetActive(true);
                }
                if (_costText != null)
                {
                    _costText.text = string.Empty;
                    _costText.gameObject.SetActive(false);
                }
                if (_levelText != null)
                    _levelText.text = $"{currentLevel}/{maxLevel}";
                if (_levelTextObject != null)
                    _levelTextObject.SetActive(true);
                if (_buyButton != null)
                    _buyButton.interactable = false;
                if (_unlockedOverlay != null)
                    _unlockedOverlay.SetActive(false);
                if (_notEnoughOverlay != null)
                    _notEnoughOverlay.SetActive(false);
                return;
            }

            // ---- Состояние "Не максимальный" ----
            if (_boughtText != null)
                _boughtText.SetActive(false);

            if (_costText != null)
            {
                _costText.text = cost.ToString();
                _costText.gameObject.SetActive(true);
            }

            if (_levelText != null)
                _levelText.text = $"{currentLevel}/{maxLevel}";
            if (_levelTextObject != null)
                _levelTextObject.SetActive(true);

            // Кнопка всегда интерактивна, если не максимальный уровень
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
