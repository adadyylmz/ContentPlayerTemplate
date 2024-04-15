using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Unity.Services.Ugc.Samples.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(SubscriptionButton))]
    public class SubscriptionButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SubscriptionColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SubscribedColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SubscriptionIcon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SubscribedIcon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ButtonLabel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ButtonImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IconImage"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
