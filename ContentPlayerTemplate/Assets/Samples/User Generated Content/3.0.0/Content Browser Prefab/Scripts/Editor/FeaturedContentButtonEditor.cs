using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Unity.Services.Ugc.Samples.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(FeaturedContentButton))]
    public class FeaturedContentButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ContentRawImage"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
