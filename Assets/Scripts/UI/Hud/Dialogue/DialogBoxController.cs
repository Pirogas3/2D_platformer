using Assets.Scripts.Model.Data;
using System;
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

        [Space][SerializeField] private float _textSpeed = 0.09f;

        [Header("Sounds")]
        [SerializeField] private AudioClip _typing;
        [SerializeField] private AudioClip _open;
        [SerializeField] private AudioClip _close;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Test")]
        [SerializeField] private DialogData _testData;

        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        private DialogData _data;
        private int _currentSentence;
        private Coroutine _typingRoutine;

        private void Awake()
        {
            _container.SetActive(false);
        }

        public void ShowDialog(DialogData data)
        {
            _data = data;
            _currentSentence = 0;
            _text.text = string.Empty;

            _container.SetActive(true);
            _sfxSource?.PlayOneShot(_open);
            _animator.SetBool(IsOpen, true);
        }

        public void CloseDialog()
        {
            _animator.SetBool(IsOpen, false);
            _sfxSource?.PlayOneShot(_close);
        }

        public void OnSkip()
        {
            if (_typingRoutine == null) return;

            StopTypeAnimation();
            _text.text = _data.Sentences[_currentSentence];
        }

        public void OnContinue()
        {
            StopTypeAnimation();
            _currentSentence++;

            var isDialogComplited = _currentSentence >= _data.Sentences.Length;
            if (isDialogComplited)
            {
                CloseDialog();
            }
            else
            {
                OnStartDialogAnimation();
            }
        }

        public void OnClick()
        {
            if (_typingRoutine != null)
            {
                OnSkip();
            }
            else
            {
                OnContinue();
            }
        }

        private void StopTypeAnimation()
        {
            if (_typingRoutine != null)
            {
                StopCoroutine(_typingRoutine);
            }
            _typingRoutine = null;
        }

        private void OnStartDialogAnimation()
        {
            _typingRoutine = StartCoroutine(TypeDialogText());
        }

        private void OnCloseDialogAnimation()
        {

        }

        private IEnumerator TypeDialogText()
        {
            _text.text = string.Empty;
            var sentences = _data.Sentences[_currentSentence];
            foreach (var letter in sentences)
            {
                _text.text += letter;
                _sfxSource?.PlayOneShot(_typing);
                yield return new WaitForSeconds(_textSpeed);
            }

            _typingRoutine = null;
        }

        [ContextMenu("TestShowDialog")]
        public void Test()
        {
            ShowDialog(_testData);
        }
    }
}
