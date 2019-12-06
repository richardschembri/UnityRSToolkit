namespace  RSToolkit.UI.Editor
{
    using UnityEditor;
    using UnityEngine;
    using RSToolkit.UI.Controls;

    [CustomEditor(typeof(UIEditableListBox))]
    [CanEditMultipleObjects]
    public class UIEditableListBoxEditor : UIListBoxEditor  
    {
        SerializedProperty m_OrderAscending;

        protected override void OnEnable(){
            base.OnEnable();
            m_OrderAscending = serializedObject.FindProperty("m_OrderAscending");
        }
        public override void OnInspectorGUI(){
            serializedObject.Update();
            DrawInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }
        protected override void DrawInspectorGUI(){
            base.DrawInspectorGUI();
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_OrderAscending, EditorGUIUtility.TrTextContent("Order Ascending"));
            EditorGUIUtility.labelWidth = 0;
        }
    }
}