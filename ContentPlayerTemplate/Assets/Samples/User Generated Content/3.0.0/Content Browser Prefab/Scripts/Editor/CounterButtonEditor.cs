using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Unity.Services.Ugc.Samples.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(CounterButton))]
    public class CounterButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IconImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CountLabel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_NormalStateIconSprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ActiveStateIconSprite"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}