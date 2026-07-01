using UnityEditor;
using UnityEditor.UI;

namespace Assets.Scripts.UI.Widgets.Editor
{
    [CustomEditor(typeof(CustonButton), true)]
    [CanEditMultipleObjects]
    public class CustonButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_normal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pressed"));
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
