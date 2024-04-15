using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Unity.Services.Ugc.Samples.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(PageButton))]
    public class PageButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PageIndexLabel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SelectedImage"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
