namespace  RSToolkit.UI.Editor
{
    using UnityEditor;
    using UnityEngine;
    using RSToolkit.UI.Controls;

    [CustomEditor(typeof(UIEditableListBox))]
    [CanEditMultipleObjects]
    public class UIEditableListBoxEditor : UIListBoxEditor  
    {
        SerializedProperty _OrderAscending;

        protected override void OnEnable(){
            base.OnEnable();
            _OrderAscending = serializedObject.FindProperty("_OrderAscending");
        }
        public override void OnInspectorGUI(){

            DrawDefaultInspector();
            serializedObject.Update();
            DrawInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }
        protected override void DrawInspectorGUI(){
            base.DrawInspectorGUI();
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _OrderAscending, EditorGUIUtility.TrTextContent("Order Ascending"));
            EditorGUIUtility.labelWidth = 0;
        }
    }
}