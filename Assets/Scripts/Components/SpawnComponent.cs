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

        [ContextMenu("Spawn As Child")]
        public void SpawnAsChild()
        {
            Transform parent = _target != null ? _target : transform;
            var instance = Instantiate(_prefab, parent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one; // или сохранять масштаб родителя
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
