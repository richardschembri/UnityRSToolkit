namespace  RSToolkit.UI.Editor
{
    using UnityEngine;
    using UnityEditor;
    using RSToolkit.UI.Controls;

    [CustomEditor(typeof(UIListBox<MonoBehaviour>))]
    [CanEditMultipleObjects]
    public class UIListBoxEditor : Editor
    {
        SerializedProperty _Padding;
        SerializedProperty _Spacing;
        SerializedProperty _ManualScrollSpeed;
        SerializedProperty _IsVertical;

        SerializedProperty _ChildAlignment;
        SerializedProperty _OcclusionCulling;

        protected virtual void OnEnable(){
            _Padding           = serializedObject.FindProperty("_Padding");
            _Spacing           = serializedObject.FindProperty("_Spacing");
            _ManualScrollSpeed = serializedObject.FindProperty("_ManualScrollSpeed"); 
            _IsVertical        = serializedObject.FindProperty("_IsVertical");
            _ChildAlignment    = serializedObject.FindProperty("_ChildAlignment");
            _OcclusionCulling  = serializedObject.FindProperty("_OcclusionCulling"); 
        }

        public override void OnInspectorGUI(){
            serializedObject.Update();
            DrawInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInspectorGUI(){

            EditorGUILayout.PropertyField(_Padding, true);
            EditorGUILayout.PropertyField(_Spacing, true);
            EditorGUILayout.PropertyField(_ManualScrollSpeed, true);

            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _IsVertical, EditorGUIUtility.TrTextContent("Is Vertical"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _OcclusionCulling, EditorGUIUtility.TrTextContent("Occlusion Culling"));
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