using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components
{
    [Serializable]
    public class SpriteAnimationClip
    {
        public string name;
        public Sprite[] sprites;
        public int frameRate;
        public bool loop;
        public UnityEvent onComplete;
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimationComponent : MonoBehaviour
    {
        [SerializeField] private List<SpriteAnimationClip> _clips;
        [SerializeField] private string _defaultClipName = "Idle";

        private SpriteRenderer _renderer;
        private Dictionary<string, SpriteAnimationClip> _clipDict;
        private SpriteAnimationClip _currentClip;
        private bool _isPlaying;
        private bool _isAnimationSet = false;
        private int _currentSpriteIndex;
        private float _nextFrameTime;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            BuildDictionary();
        }

        private void Start()
        {
            if (!_isAnimationSet)
                Play(_defaultClipName);
        }

        private void Update()
        {
            if (!_isPlaying || _currentClip == null || _currentClip.sprites.Length == 0) return;

            if (_nextFrameTime > Time.time) return;

            _currentSpriteIndex++;
            if (_currentSpriteIndex >= _currentClip.sprites.Length)
            {
                if (_currentClip.loop)
                {
                    _currentSpriteIndex = 0;
                }
                else
                {
                    _isPlaying = false;
                    _currentClip.onComplete?.Invoke();
                    return;
                }
            }

            _renderer.sprite = _currentClip.sprites[_currentSpriteIndex];
            _nextFrameTime = Time.time + (1f / _currentClip.frameRate);
        }

        private void BuildDictionary()
        {
            _clipDict = new Dictionary<string, SpriteAnimationClip>();
            foreach (var clip in _clips)
            {
                if (!_clipDict.ContainsKey(clip.name))
                    _clipDict.Add(clip.name, clip);
                else
                    Debug.LogWarning($"Duplicate clip name: {clip.name}");
            }
        }

        public void Play(string clipName)
        {
            if (!_clipDict.TryGetValue(clipName, out var clip))
            {
                Debug.LogError($"Animation clip '{clipName}' not found!");
                return;
            }
            _isAnimationSet = true;

            _currentClip = clip;
            _currentSpriteIndex = 0;
            _isPlaying = true;
            _nextFrameTime = Time.time;

            if (_currentClip.sprites.Length > 0)
                _renderer.sprite = _currentClip.sprites[0];
        }

        public void Stop()
        {
            _isPlaying = false;
        }
    }
}
