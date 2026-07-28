using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Assets.Scripts.UI.Widgets
{
    /// <summary>
    /// Анимация для UI Image: последовательная смена спрайтов.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UISpriteAnimation : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite[] _sprites;

        [Header("Settings")]
        [SerializeField] private float _frameRate = 10f; // кадров в секунду
        [SerializeField] private bool _loop = true;
        [SerializeField] private bool _playOnAwake = true;

        [Header("Events")]
        [SerializeField] private UnityEvent _onComplete; // вызывается, когда анимация завершена (если не зациклена)

        private Image _image;
        private int _currentIndex;
        private float _timer;
        private bool _isPlaying;
        private bool _isPaused;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            if (_playOnAwake)
                Play();
        }

        private void OnEnable()
        {
            if (_playOnAwake)
                Play();
        }

        private void OnDisable()
        {
            _isPlaying = false;
            _isPaused = false;
        }

        private void Update()
        {
            if (!_isPlaying || _isPaused || _sprites == null || _sprites.Length == 0)
                return;

            _timer += Time.deltaTime;
            float frameDuration = 1f / _frameRate;

            while (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                _currentIndex++;

                if (_currentIndex >= _sprites.Length)
                {
                    if (_loop)
                    {
                        _currentIndex = 0;
                    }
                    else
                    {
                        _isPlaying = false;
                        _onComplete?.Invoke();
                        return;
                    }
                }

                _image.sprite = _sprites[_currentIndex];
            }
        }

        /// <summary>
        /// Начать воспроизведение с первого кадра.
        /// </summary>
        public void Play()
        {
            if (_sprites == null || _sprites.Length == 0)
                return;

            _isPlaying = true;
            _isPaused = false;
            _currentIndex = 0;
            _timer = 0f;
            _image.sprite = _sprites[0];
        }

        /// <summary>
        /// Начать воспроизведение с указанного кадра.
        /// </summary>
        public void PlayFrom(int index)
        {
            if (_sprites == null || _sprites.Length == 0 || index < 0 || index >= _sprites.Length)
                return;

            _isPlaying = true;
            _isPaused = false;
            _currentIndex = index;
            _timer = 0f;
            _image.sprite = _sprites[_currentIndex];
        }

        /// <summary>
        /// Остановить анимацию (сбросить на первый кадр).
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _isPaused = false;
            _currentIndex = 0;
            _timer = 0f;
            if (_sprites != null && _sprites.Length > 0)
                _image.sprite = _sprites[0];
        }

        /// <summary>
        /// Приостановить анимацию (сохранить текущий кадр).
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Возобновить анимацию.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }
    }
}
