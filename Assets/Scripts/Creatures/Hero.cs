using Assets.Scripts.Model;
using Scripts.Components;
using System.Collections;
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

        //Дальняя атака
        [Header("Dist Attack")]
        [SerializeField] private float _throwCooldown = 0.5f;
        private float _lastThrowTime;

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

        public void CollectSword(int count)
        {
            if (!_gameSession.PlayerData.IsArmed)
            {
                ChangeArmedOrUnarmed();
            }

            _gameSession.PlayerData.SwordCount += count;
            Debug.Log($"У игрока: {_gameSession.PlayerData.SwordCount} мечей.");
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

        public override void ThrowAttack(float holdTime)
        {
            if (!_gameSession.PlayerData.IsArmed
               || _gameSession.PlayerData.SwordCount <= 1
               || Time.time < _lastThrowTime + _throwCooldown)
            {
                Debug.Log("Бросок ещё не готов");
                return;
            }
            _lastThrowTime = Time.time;

            if (holdTime > 1.0f && _gameSession.PlayerData.SwordCount >= 4)
            {
                _gameSession.PlayerData.SwordCount -= 3;
                StartCoroutine(MultiThrowAttack(holdTime));
            }
            else
            {
                _gameSession.PlayerData.SwordCount -= 1;
                base.ThrowAttack(holdTime);
            }
        }

        public IEnumerator MultiThrowAttack(float holdTime)
        {
            if (!_gameSession.PlayerData.IsArmed) yield break;

            int throwCount = holdTime > 1.0f ? 3 : 1;

            for (int i = 0; i < throwCount; i++)
            {
                _animator.SetTrigger(ThrowKey);
                _particles.Spawn("Throw");

                if (i < throwCount - 1)
                    yield return new WaitForSeconds(0.2f);
            }
        }

        public override void TakeDamageSimple()
        {
            base.TakeDamageSimple();
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            _rigidbody.AddForce(Vector2.up * (_jumpPower / 3), ForceMode2D.Impulse);
        }

        public void ChangeArmedOrUnarmed()
        {
            _gameSession.PlayerData.IsArmed = !_gameSession.PlayerData.IsArmed;
            _animator.runtimeAnimatorController = _gameSession.PlayerData.IsArmed ? _heroArmed : _heroUnarmed;
        }
    }
}
