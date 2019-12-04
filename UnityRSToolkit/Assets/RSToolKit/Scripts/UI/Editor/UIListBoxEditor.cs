namespace  RSToolkit.UI.Editor
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class UIListBoxEditor : Editor
    {
        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_ManualScrollSpeed;
        // SerializedProperty m_ListItemSpawner;
        SerializedProperty m_IsVertical;

        SerializedProperty m_ChildAlignment;
        protected virtual void OnEnable(){
            m_Padding           = serializedObject.FindProperty("m_Padding");
            m_Spacing           = serializedObject.FindProperty("m_Spacing");
            m_ManualScrollSpeed = serializedObject.FindProperty("m_ManualScrollSpeed"); 
            // m_ListItemSpawner   = serializedObject.FindProperty("m_ListItemSpawner");  
            m_IsVertical        = serializedObject.FindProperty("m_IsVertical");
            m_ChildAlignment    = serializedObject.FindProperty("m_ChildAlignment");
        }

        public override void OnInspectorGUI(){
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_ManualScrollSpeed, true);
            // EditorGUILayout.PropertyField(m_ListItemSpawner, true);
            EditorGUILayout.PropertyField(m_IsVertical, true);

            Rect rect = EditorGUILayout.GetControlRect();
            ToggleLeft(rect, m_IsVertical, EditorGUIUtility.TrTextContent("Is Vertical"));
            EditorGUIUtility.labelWidth = 0;

            serializedObject.ApplyModifiedProperties();
        }

        void ToggleLeft(Rect position, SerializedProperty property, GUIContent label){
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