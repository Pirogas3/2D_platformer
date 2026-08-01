using Assets.Scripts.Model.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hud.Dialogue
{
    public class DialogBoxController : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private GameObject _container;
        [SerializeField] private Animator _animator;

        [Space]
        [SerializeField] private float _textSpeed = 0.09f;

        [Header("Sounds")]
        [SerializeField] private AudioClip _typing;
        [SerializeField] private AudioClip _open;
        [SerializeField] private AudioClip _close;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Test")]
        [SerializeField] private DialogData _testData;

        private static readonly int _isOpen = Animator.StringToHash("IsOpen");

        private DialogData _data;
        private string[] _sentencesToShow;
        private int _currentSentence;
        private Coroutine _typingRoutine;
        private bool _isTyping;
        private bool _isClosing;

        public event System.Action OnDialogOpened;
        public event System.Action OnDialogClosed;

        public bool IsOpen => _container.activeSelf;

        private void Awake()
        {
            _container.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _container.activeSelf)
            {
                OnClick();
            }
        }

        public void ShowDialog(DialogData data)
        {
            // Если диалог уже открыт или закрывается, не открываем новый
            if (_container.activeSelf || _isClosing) return;

            _data = data;
            _sentencesToShow = data.GetSentences();
            _currentSentence = 0;
            _text.text = string.Empty;
            _isTyping = false;

            _container.SetActive(true);
            _sfxSource?.PlayOneShot(_open);
            _animator.SetBool(_isOpen, true);

            OnDialogOpened?.Invoke();
        }

        public void CloseDialog()
        {
            if (_isClosing) return;

            _isClosing = true;
            _isTyping = false;
            if (_typingRoutine != null)
            {
                StopCoroutine(_typingRoutine);
                _typingRoutine = null;
            }
            _text.text = string.Empty;
            _animator.SetBool(_isOpen, false);
            _sfxSource?.PlayOneShot(_close);

            OnDialogClosed?.Invoke();
        }

        public void OnSkip()
        {
            if (!_isTyping) return;
            _isTyping = false;
            if (_typingRoutine != null)
            {
                StopCoroutine(_typingRoutine);
                _typingRoutine = null;
            }
            _text.text = _sentencesToShow[_currentSentence];
        }

        public void OnContinue()
        {
            if (_isTyping) return;
            _currentSentence++;
            if (_currentSentence >= _sentencesToShow.Length)
            {
                CloseDialog();
            }
            else
            {
                StartTypingCurrentSentence();
            }
        }

        public void OnClick()
        {
            if (_isTyping)
                OnSkip();
            else
                OnContinue();
        }

        private void StartTypingCurrentSentence()
        {
            _text.text = string.Empty;
            _isTyping = true;
            if (_typingRoutine != null)
            {
                StopCoroutine(_typingRoutine);
                _typingRoutine = null;
            }
            _typingRoutine = StartCoroutine(TypeDialogText());
        }

        private IEnumerator TypeDialogText()
        {
            string sentence = _sentencesToShow[_currentSentence];
            foreach (char letter in sentence)
            {
                if (!_isTyping)
                {
                    yield break;
                }
                _text.text += letter;
                _sfxSource?.PlayOneShot(_typing);
                yield return new WaitForSeconds(_textSpeed);
            }
            _isTyping = false;
            _typingRoutine = null;
        }

        public void OnStartDialogAnimation()
        {
            if (_sentencesToShow != null && _sentencesToShow.Length > 0)
            {
                StartTypingCurrentSentence();
            }
        }

        public void OnCloseDialogAnimation()
        {
            // Вызывается из анимации закрытия
            _isClosing = false;
            _container.SetActive(false);
            _text.text = string.Empty;
        }

        [ContextMenu("TestShowDialog")]
        public void Test()
        {
            ShowDialog(_testData);
        }
    }
}
