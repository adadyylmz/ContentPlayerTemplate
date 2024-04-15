using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Unity.Services.Ugc.Samples.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(HeaderTabButton))]
    public class HeaderTabButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ButtonLabel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NormalStateLabelColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SelectedStateLabelColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NormalStateLabelFont"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SelectedStateLabelFont"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SelectedStateOutlineImage"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
