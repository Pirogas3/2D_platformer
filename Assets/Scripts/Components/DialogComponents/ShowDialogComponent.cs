using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using Assets.Scripts.UI.Hud.Dialogue;
using System;
using UnityEngine;

namespace Assets.Scripts.Components.DialogComponents
{
    public class ShowDialogComponent : MonoBehaviour
    {
        [SerializeField] private Mode _mode;
        [SerializeField] private DialogData _bound;
        [SerializeField] private DialogDef _external;

        private DialogBoxController _dialogBox;

        private void Start()
        {
            _dialogBox = FindObjectOfType<DialogBoxController>();
        }

        public void Show()
        {
            if (_dialogBox == null)
            {
                Debug.LogWarning("DialogBoxController not found!");
                return;
            }

            if (_dialogBox.IsOpen)
            {
                _dialogBox.CloseDialog();
            }
            else
            {
                _dialogBox.ShowDialog(Data);
            }
        }

        public DialogData Data
        {
            get
            {
                switch (_mode)
                {
                    case Mode.Bound:
                        return _bound;
                    case Mode.External:
                        return _external.Data;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public enum Mode
        {
            Bound,
            External
        }
    }
}
