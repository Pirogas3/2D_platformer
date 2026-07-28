using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class SpawnComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            var instantiate = Instantiate(_prefab, _target.position, Quaternion.identity);
            instantiate.transform.localScale = _target.lossyScale;
        }

        public void Spawn(int damage, int attack)
        {
            var instantiate = Instantiate(_prefab, _target.position, Quaternion.identity);
            instantiate.transform.localScale = _target.lossyScale;

            var damageComponent = instantiate.GetComponent<DamageComponent>();
            if (damageComponent != null)
            {
                damageComponent.SetDamage(damage);
                damageComponent.SetAttack(attack);
            }
        }

        public void SpawnDelayed(float delay)
        {
            StartCoroutine(SpawnRoutine(delay));
        }

        protected IEnumerator SpawnRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            Spawn();
        }
    }
}
