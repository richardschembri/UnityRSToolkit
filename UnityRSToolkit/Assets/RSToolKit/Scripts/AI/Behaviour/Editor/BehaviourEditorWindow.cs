using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourEditorWindow : EditorWindow
    {
        Dictionary<BehaviourNode.Operator, string> m_operatorSymbols;
        Dictionary<string, string> m_tagByName;

        private GUIStyle m_smallTextStyle, m_nodeCapsuleGray, m_nodeCapsuleFailed, m_nodeCapsuleStopRequested;
        private GUIStyle m_nestedBoxStyle;
        private const int NESTED_PADDING = 10;

        private Color m_defaultColor;

        public void Init()
        {
            m_operatorSymbols = new Dictionary<BehaviourNode.Operator, string>();
            m_operatorSymbols[BehaviourNode.Operator.IS_SET] = "?=";
            m_operatorSymbols[BehaviourNode.Operator.IS_NOT_SET] = "?!=";
            m_operatorSymbols[BehaviourNode.Operator.IS_EQUAL] = "==";
            m_operatorSymbols[BehaviourNode.Operator.IS_NOT_EQUAL] = "!=";
            m_operatorSymbols[BehaviourNode.Operator.IS_GREATER_OR_EQUAL] = ">=";
            m_operatorSymbols[BehaviourNode.Operator.IS_GREATER] = ">";
            m_operatorSymbols[BehaviourNode.Operator.IS_SMALLER_OR_EQUAL] = "<=";
            m_operatorSymbols[BehaviourNode.Operator.IS_SMALLER] = "<";
            m_operatorSymbols[BehaviourNode.Operator.ALWAYS_TRUE] = "ALWAYS_TRUE";

            m_tagByName = new Dictionary<string, string>();
            m_tagByName["Selector"] = "?";
            m_tagByName["Sequence"] = "->";

            m_nestedBoxStyle = new GUIStyle();
            m_nestedBoxStyle.margin = new RectOffset(NESTED_PADDING, 0, 0, 0);

            m_smallTextStyle = new GUIStyle();
            m_smallTextStyle.font = EditorStyles.miniFont;

            m_nodeCapsuleGray = (GUIStyle)"helpbox";
            m_nodeCapsuleGray.normal.textColor = Color.black;

            m_nodeCapsuleFailed = new GUIStyle(m_nodeCapsuleGray);
            m_nodeCapsuleFailed.normal.textColor = Color.red;
            m_nodeCapsuleStopRequested = new GUIStyle(m_nodeCapsuleGray);
            m_nodeCapsuleStopRequested.normal.textColor = new Color(0.7f, 0.7f, 0.0f);

            m_defaultColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        [MenuItem("Window/RSToolkit/BehaviourTree")]
        public static void ShowWindow()
        {
            BehaviourEditorWindow window = (BehaviourEditorWindow)EditorWindow.GetWindow(typeof(BehaviourEditorWindow), false, "Behaviour Tree");
            window.Show();
        }

        
    }
}
