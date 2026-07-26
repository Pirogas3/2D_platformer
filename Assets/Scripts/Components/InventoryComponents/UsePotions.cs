using Assets.Scripts.Creatures;
using System.Collections;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Definitions;
using UnityEngine;

namespace Assets.Scripts.Components.InventoryComponents
{
    /// <summary>
    /// Компонент для применения эффектов зелий на герое.
    /// Вешается на объект Hero и ссылается на него.
    /// </summary>
    public class UsePotions : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        [SerializeField] private HeroHealthComponent _heroHealthComponent;

        private GameSession _gameSession;
        private float _baseJumpPower;
        private Coroutine _jumpBoostCoroutine;

        private void Awake()
        {
            if (_hero == null)
                _hero = GetComponent<Hero>();

            if (_heroHealthComponent == null)
                _heroHealthComponent = GetComponent<HeroHealthComponent>();
        }

        private void Start()
        {
            _gameSession = GameSession.Instance;
            if (_gameSession == null)
            {
                Debug.LogError("UsePotions: GameSession not found!", this);
                return;
            }

            // Сохраняем базовую силу прыжка из Creature
            _baseJumpPower = _hero.JumpPower;
        }

        /// <summary>
        /// Применить эффекты зелья по его ID.
        /// Вызывается из Hero.UseItem при использовании предмета категории Potion.
        /// </summary>
        public void ApplyPotionEffects(string itemId)
        {
            var def = DefsFacade.Instance.Properties.Get(itemId);
            if (def == null)
            {
                Debug.LogWarning($"UsePotions: ItemProperties for {itemId} not found!");
                return;
            }

            // Лечение
            if (def.Healing > 0)
            {
                ApplyHealing(def.Healing, itemId);
            }

            // Усиление прыжка
            if (def.JumpBoost > 0 && def.Duration > 0)
            {
                ApplyJumpBoost(def.JumpBoost, def.Duration, itemId);
            }

            // Здесь можно добавить другие эффекты (ускорение, защита и т.д.)
        }

        private void ApplyHealing(int healingAmount, string itemId)
        {
            if (_gameSession.PlayerData.Hp == _gameSession.PlayerData.MaxHp)
            {
                Debug.Log("HP максимальное - зелье здоровья не использовано!");
                return;
            }

            _gameSession.PlayerData.Hp += healingAmount;
            _heroHealthComponent.SetHealth(_gameSession.PlayerData.Hp);
            _gameSession.PlayerData.Inventory.Remove(itemId, 1);
            Debug.Log($"Вы выпили зелье! + {healingAmount} HP. Текущее здоровье: {_gameSession.PlayerData.Hp}");
        }

        private void ApplyJumpBoost(int boostAmount, float duration, string itemId)
        {
            // Останавливаем предыдущий эффект, если был
            if (_jumpBoostCoroutine != null)
            {
                StopCoroutine(_jumpBoostCoroutine);
                _jumpBoostCoroutine = null;
                _hero.JumpPower = _baseJumpPower;
            }

            // Применяем буст
            _hero.JumpPower = _baseJumpPower + boostAmount;
            _gameSession.PlayerData.Inventory.Remove(itemId, 1);
            _jumpBoostCoroutine = StartCoroutine(JumpBoostRoutine(duration));

            Debug.Log($"Применено зелье усиления прыжка +{boostAmount} на {duration} сек.");
        }

        private IEnumerator JumpBoostRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            _hero.JumpPower = _baseJumpPower;
            _jumpBoostCoroutine = null;
            Debug.Log("Эффект усиления прыжка закончился.");
        }

        private void OnDestroy()
        {
            // Сбрасываем эффект при уничтожении объекта
            if (_jumpBoostCoroutine != null)
            {
                StopCoroutine(_jumpBoostCoroutine);
                _jumpBoostCoroutine = null;
                _hero.JumpPower = _baseJumpPower;
            }
        }
    }
}
