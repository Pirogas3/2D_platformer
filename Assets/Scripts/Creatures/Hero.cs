using Assets.Scripts.Components;
using Assets.Scripts.Model;
using Scripts.Components;
using UnityEditor.Animations;
using UnityEngine;

namespace Scripts.Creatures
{
    public class Hero : Creature
    {
        //Данные для игрока
        private GameSession _gameSession;

        //Интерактивность
        [Header("Interaction settings")]
        [SerializeField] private float _interactionRadius; //радиус в котором проверяются интерактивные объекты
        [SerializeField] private LayerMask _interactionLayer; //слой интерактивных объектов
        private Collider2D[] _interactionResult = new Collider2D[1];

        //Анимации
        [Header("Animations")]
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private AnimatorController _heroUnarmed;
        [SerializeField] private AnimatorController _heroArmed;

        private Transform _activePlatform; //Запоминаем текущую платформу

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            _gameSession = FindObjectOfType<GameSession>();

            if (_gameSession.PlayerData.IsArmed)
                _animator.runtimeAnimatorController = _heroArmed;
            else
                _animator.runtimeAnimatorController = _heroUnarmed;

            HealthComponent health = GetComponent<HealthComponent>();
            if (health != null)
            {
                health.SetHealth(_gameSession.PlayerData.Hp);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                _activePlatform = collision.transform;
                transform.SetParent(_activePlatform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                _activePlatform = null;
                transform.SetParent(null);
            }
        }

        public override void TakeDamageFromSpikes()
        {
            base.TakeDamageFromSpikes();

            if (_gameSession.PlayerData.Money > 0)
            {
                SpawnCoins();
            }
        }

        public void OnHeroHealthChanged(int newHealth)
        {
            _gameSession.PlayerData.Hp = newHealth;
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_gameSession.PlayerData.Money, 5);
            _gameSession.PlayerData.Money -= numCoinsToDispose;

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.Play();
        }

        public void CollectCoin(int cost)
        {
            _gameSession.PlayerData.Money += cost;
            Debug.Log($"У игрока: {_gameSession.PlayerData.Money} денег.");
        }

        public void Interact()
        {
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, _interactionRadius, _interactionResult, _interactionLayer);
            for (int i = 0; i < size; i++)
            {
                var interactable = _interactionResult[i].GetComponent<InteractableComponent>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        public override void Attack()
        {
            if (!_gameSession.PlayerData.IsArmed) return;
            base.Attack();
        }

        public void ChangeArmedOrUnarmed()
        {
            _gameSession.PlayerData.IsArmed = !_gameSession.PlayerData.IsArmed;
            _animator.runtimeAnimatorController = _gameSession.PlayerData.IsArmed ? _heroArmed : _heroUnarmed;
        }
    }
}
