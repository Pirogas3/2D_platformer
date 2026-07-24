using Assets.Scripts.Model;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.LoadMenu
{
    public class LoadMenuWindow : AnimatedWindow
    {
        [Header("Quick Save")]
        [SerializeField] private Button _quickSaveButton;

        [Header("Auto Save Buttons")]
        [SerializeField] private GameObject _autoSaveButtonPrefab;
        [SerializeField] private Transform _autoSaveButtonsContainer;

        private GameSession _gameSession;

        protected override void Start()
        {
            base.Start();

            _gameSession = FindObjectOfType<GameSession>();
            if (_gameSession == null)
                Debug.LogError("GameSession not found!");
            RefreshAllButtons();
        }

        public void LoadQuickSave()
        {
            if (_gameSession != null)
                _gameSession.QuickLoad();
        }

        private void RefreshAllButtons()
        {
            RefreshQuickSaveButton();
            RefreshAutoSaveButtons();
        }

        private void RefreshQuickSaveButton()
        {
            if (_quickSaveButton == null) return;

            string quickSavePath = SaveManager.GetSlotPath("QuickSave");
            bool exists = SaveManager.SlotExists("QuickSave");
            string displayText;
            if (exists)
            {
                var lastWrite = File.GetLastWriteTime(quickSavePath);
                string dateTime = lastWrite.ToString("HH:mm dd.MM.yyyy");
                displayText = $"Quick Save: {dateTime}";
            }
            else
            {
                displayText = "No quicksave";
            }

            var texts = _quickSaveButton.GetComponentsInChildren<Text>(true);
            foreach (var t in texts)
                t.text = displayText;

            _quickSaveButton.interactable = exists;
        }

        private void RefreshAutoSaveButtons()
        {
            foreach (Transform child in _autoSaveButtonsContainer)
                Destroy(child.gameObject);

            var slots = SaveManager.GetAutoSaveSlotsWithTime();
            if (slots.Count == 0)
            {
                var emptyText = Instantiate(_autoSaveButtonPrefab, _autoSaveButtonsContainer);
                var texts = emptyText.GetComponentsInChildren<Text>(true);
                string msg = "No autosaves";
                foreach (var t in texts) t.text = msg;
                emptyText.GetComponent<Button>().interactable = false;
                return;
            }

            slots.Sort((a, b) => b.lastWriteTime.CompareTo(a.lastWriteTime));

            foreach (var slot in slots)
            {
                var buttonGO = Instantiate(_autoSaveButtonPrefab, _autoSaveButtonsContainer);
                var button = buttonGO.GetComponent<Button>();
                var texts = buttonGO.GetComponentsInChildren<Text>(true);

                string number = slot.slotName.Replace("AutoSave_", "");
                string dateTime = slot.lastWriteTime.ToString("HH:mm dd.MM.yyyy");
                string displayText = $"{number}: {dateTime}";

                foreach (var t in texts)
                    t.text = displayText;

                string slotName = slot.slotName;
                button.onClick.AddListener(() =>
                {
                    if (_gameSession != null)
                        _gameSession.LoadFromSlot(slotName);
                });
            }
        }
    }
}
