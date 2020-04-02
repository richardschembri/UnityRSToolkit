namespace  RSToolkit.UI.Editor
{
    using UnityEngine;
    using UnityEditor;
    using RSToolkit.UI.Controls;

    [CustomEditor(typeof(UIListBox))]
    [CanEditMultipleObjects]
    public class UIListBoxEditor : Editor
    {
        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_ManualScrollSpeed;
        SerializedProperty m_IsVertical;

        SerializedProperty m_ChildAlignment;
        SerializedProperty m_OcclusionCulling;

        protected virtual void OnEnable(){
            m_Padding           = serializedObject.FindProperty("m_Padding");
            m_Spacing           = serializedObject.FindProperty("m_Spacing");
            m_ManualScrollSpeed = serializedObject.FindProperty("m_ManualScrollSpeed"); 
            m_IsVertical        = serializedObject.FindProperty("m_IsVertical");
            m_ChildAlignment    = serializedObject.FindProperty("m_ChildAlignment");
            m_OcclusionCulling  = serializedObject.FindProperty("m_OcclusionCulling"); 
        }

        public override void OnInspectorGUI(){
            serializedObject.Update();
            DrawInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInspectorGUI(){

            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_ManualScrollSpeed, true);

            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_IsVertical, EditorGUIUtility.TrTextContent("Is Vertical"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_OcclusionCulling, EditorGUIUtility.TrTextContent("Occlusion Culling"));
            EditorGUIUtility.labelWidth = 0;
        }

        protected void ToggleLeft(Rect position, SerializedProperty property, GUIContent label){
            bool toggle = property.boolValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            toggle = EditorGUI.ToggleLeft(position, label, toggle);
            EditorGUI.indentLevel = oldIndent;
            if(EditorGUI.EndChangeCheck()){
                property.boolValue = property.hasMultipleDifferentValues ? true : !property.boolValue;
            }
            EditorGUI.showMixedValue = false;
        }
    }
}