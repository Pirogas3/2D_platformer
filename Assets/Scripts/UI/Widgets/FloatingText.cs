using UnityEngine;
using TMPro;
using System.Collections;

namespace Assets.Scripts.UI.Widgets
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _duration = 1.5f;
        [SerializeField] private float _floatSpeed = 0.5f;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private Canvas _canvas;
        private Vector3 _startPosition;
        private float _startTime;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            if (_canvas != null && _canvas.worldCamera == null)
            {
                // Находим основную камеру автоматически
                _canvas.worldCamera = Camera.main;
                if (_canvas.worldCamera == null)
                    Debug.LogError("FloatingText: не найдена основная камера (Camera.main)!");
            }
        }

        public void Show(string message, Vector3 worldPosition, Color color, float duration = -1)
        {
            if (_text == null)
                _text = GetComponentInChildren<TextMeshProUGUI>();

            _text.text = message;
            _text.color = color;
            if (duration > 0)
                _duration = duration;
            _startPosition = worldPosition;
            _startTime = Time.time;

            transform.position = worldPosition;
            gameObject.SetActive(true);

            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            float elapsed = 0f;
            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _duration;

                transform.position = _startPosition + Vector3.up * (progress * _floatSpeed);

                Color c = _text.color;
                c.a = _fadeCurve.Evaluate(progress);
                _text.color = c;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
