using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : MonoBehaviour
    {
        [Header("Idle Settings")]
        [SerializeField] private int _idleFrameRate;
        [SerializeField] private bool _idleLoop;
        [SerializeField] private Sprite[] _idleSprites;
        [SerializeField] private UnityEvent _onCompliteIdle;

        [Header("Hit Settings")]
        [SerializeField] private int _hitFrameRate;
        [SerializeField] private bool _hitLoop;
        [SerializeField] private Sprite[] _hitSprites;
        [SerializeField] private UnityEvent _onCompliteHit;

        [Header("Destruction Settings")]
        [SerializeField] private int _destFrameRate;
        [SerializeField] private bool _destLoop;
        [SerializeField] private Sprite[] _destSprites;
        [SerializeField] private UnityEvent _onCompliteDest;

        private int _tagAnimation; //1 - Idle, 2 - Hit, 3 - Destruction
        private bool _isPlaying = true;
        private int _frameRate;
        private bool _loop;
        private Sprite[] _curentSprites;
        private SpriteRenderer _renderer;
        private float _secondPerFrame;
        private int _currentSpriteIndex;
        private float _nextFrameTime;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            ChangeSpritesWhenIdle();
        }

        private void Update()
        {
            if (!_isPlaying || _nextFrameTime > Time.time) return;

            if (_currentSpriteIndex >= _curentSprites.Length)
            {
                if (_loop)
                {
                    _currentSpriteIndex = 0;
                }
                else
                {
                    _isPlaying = false;
                    InvokeCompletionEventByTag();
                    return;
                }
            }

            _renderer.sprite = _curentSprites[_currentSpriteIndex];
            _nextFrameTime += _secondPerFrame;
            _currentSpriteIndex++;
        }

        private void InvokeCompletionEventByTag()
        {
            switch (_tagAnimation)
            {
                case 1:
                    _onCompliteIdle?.Invoke();
                    break;
                case 2:
                    _onCompliteHit?.Invoke();
                    break;
                case 3:
                    _onCompliteDest?.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void ChangeSpritesWhenIdle()
        {
            _loop = _idleLoop;
            _frameRate = _idleFrameRate;
            _curentSprites = _idleSprites;
            _currentSpriteIndex = 0;
            _tagAnimation = 1;
            _isPlaying = true;

            _secondPerFrame = 1f / _frameRate;
            _nextFrameTime = Time.time + _secondPerFrame;
        }

        public void ChangeSpritesWhenHit()
        {
            _loop = _hitLoop;
            _frameRate = _hitFrameRate;
            _curentSprites = _hitSprites;
            _currentSpriteIndex = 0;
            _tagAnimation = 2;
            _isPlaying = true;

            _secondPerFrame = 1f / _frameRate;
            _nextFrameTime = Time.time + _secondPerFrame;
        }

        public void ChangeSpritesWhenDest()
        {
            _loop = _destLoop;
            _frameRate = _destFrameRate;
            _curentSprites = _destSprites;
            _currentSpriteIndex = 0;
            _tagAnimation = 3;
            _isPlaying = true;

            _secondPerFrame = 1f / _frameRate;
            _nextFrameTime = Time.time + _secondPerFrame;
        }
    }
}
