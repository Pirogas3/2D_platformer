using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Scripts.Components;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace Scripts.Creatures
{
    public class Hero : Creature, ICanAddInInventory
    {
        //Данные для игрока
        private GameSession _gameSession;

        //Интерактивность
        [Header("Interaction settings")]
        [SerializeField] private float _interactionRadius; //радиус в котором проверяются интерактивные объекты
        [SerializeField] private LayerMask _interactionLayer; //слой интерактивных объектов
        private Collider2D[] _interactionResult = new Collider2D[5];

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

        public void OnHeroHealthChanged(int newHealth)
        {
            _gameSession.PlayerData.Hp = newHealth;
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_gameSession.PlayerData.Inventory.Count("Coin"), 5);
            _gameSession.PlayerData.Inventory.Remove("Coin", numCoinsToDispose);

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.Play();
        }

        public void AddInInventory(string id, int value)
        {
            if (id == "Sword" && _gameSession.PlayerData.Inventory.Count("Sword") < 1)
            {
                _gameSession.PlayerData.Inventory.Add(id, value);
                ChangeArmedOrUnarmed();
            }
            else _gameSession.PlayerData.Inventory.Add(id, value);
        }

        public void Interact()
        {
            int size = Physics2D.OverlapCircleNonAlloc(transform.position, _interactionRadius, _interactionResult, _interactionLayer);

            if (size == 0) return;

            Transform playerTransform = transform;
            float minDistance = float.MaxValue;
            InteractableComponent closestInteractable = null;

            for (int i = 0; i < size; i++)
            {
                Collider2D col = _interactionResult[i];
                if (col == null) continue;

                var interactable = col.GetComponent<InteractableComponent>();
                if (interactable == null) continue;

                float distance = Vector2.Distance(playerTransform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestInteractable = interactable;
                }
            }

            if (closestInteractable != null)
            {
                closestInteractable.Interact(gameObject);
            }
        }

        public override void Attack()
        {
            if (!_gameSession.PlayerData.IsArmed) return;
            _animator.SetTrigger(AttackKey);
        }

        public override void ThrowAttack(float holdTime)
        {
            if (_gameSession.PlayerData.Inventory.Count("Sword") <= 1 || Time.time < _lastThrowTime + _throwCooldown)
            {
                Debug.Log("Бросок не готов");
                return;
            }
            _lastThrowTime = Time.time;

            if (holdTime > 1.0f && _gameSession.PlayerData.Inventory.Count("Sword") >= 4)
            {
                _gameSession.PlayerData.Inventory.Remove("Sword", 3);
                StartCoroutine(MultiThrowAttack(holdTime));
            }
            else
            {
                _gameSession.PlayerData.Inventory.Remove("Sword", 1);
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
                if (_sounds != null) _sounds.PlayClip("Range");

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

        public override void TakeDamageFromSpikes()
        {
            base.TakeDamageFromSpikes();

            if (_gameSession.PlayerData.Inventory.Count("Coin") > 0)
            {
                SpawnCoins();
            }
        }

        public void UsePotionOfHealth()
        {
            if (_gameSession.PlayerData.Inventory.Count("BluePotion") > 0)
            {
                _gameSession.PlayerData.Hp += 5; //востановить 5 здоровья
                _gameSession.PlayerData.Inventory.Remove("BluePotion", 1); //и удалить 1 зелье, потом это надо поменять все
            }
        }

        public void ChangeArmedOrUnarmed()
        {
            _animator.runtimeAnimatorController = _gameSession.PlayerData.IsArmed ? _heroArmed : _heroUnarmed;
        }
    }
}
