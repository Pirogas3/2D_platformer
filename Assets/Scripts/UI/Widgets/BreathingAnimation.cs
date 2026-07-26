using UnityEngine;
using System.Collections;

namespace Assets.Scripts.UI.Widgets
{
    public class BreathingAnimation : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _amplitude = 0.05f;        // насколько увеличивается/уменьшается масштаб
        [SerializeField] private float _speed = 1.5f;            // скорость пульсации
        [SerializeField] private bool _playOnStart = true;

        private RectTransform _rectTransform;
        private Coroutine _breathingCoroutine;
        private bool _isPlaying = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null)
                _rectTransform = gameObject.AddComponent<RectTransform>();
        }

        private void Start()
        {
            if (_playOnStart)
                Play();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (_isPlaying) return;
            _isPlaying = true;
            if (_breathingCoroutine != null)
                StopCoroutine(_breathingCoroutine);
            _breathingCoroutine = StartCoroutine(BreathingRoutine());
        }

        public void Stop()
        {
            _isPlaying = false;
            if (_breathingCoroutine != null)
            {
                StopCoroutine(_breathingCoroutine);
                _breathingCoroutine = null;
            }
            // Возвращаем исходный масштаб
            _rectTransform.localScale = Vector3.one;
        }

        private IEnumerator BreathingRoutine()
        {
            Vector3 baseScale = Vector3.one;
            float time = 0f;

            while (_isPlaying)
            {
                time += Time.deltaTime * _speed;
                float factor = 1f + Mathf.Sin(time) * _amplitude;
                _rectTransform.localScale = baseScale * factor;
                yield return null;
            }
        }

        private void OnDisable()
        {
            Stop();
        }
    }
}
