using Assets.Scripts.Model.Data;
using UnityEngine;

namespace Assets.Scripts.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/Dialog", fileName = "DialogDef")]
    public class DialogDef : ScriptableObject
    {
        [SerializeField] private DialogData _data;
        public DialogData Data => _data;
    }
}
