using Assets.Scripts.Utils.Editor;
using UnityEditor;

namespace Assets.Scripts.Components.DialogComponents.Editor
{
    [CustomEditor(typeof(ShowDialogComponent))]
    public class ShowDialogComnponentEditor : UnityEditor.Editor
    {
        private SerializedProperty _modeProperty;

        private void OnEnable()
        {
            _modeProperty = serializedObject.FindProperty("_mode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_modeProperty);
            ShowDialogComponent.Mode mode;

            if (_modeProperty.GetEnum(out mode))
            {
                switch (mode)
                {
                    case ShowDialogComponent.Mode.Bound:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_bound"));
                        break;
                    case ShowDialogComponent.Mode.External:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_external"));
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
