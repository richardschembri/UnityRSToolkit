#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RSToolkit.AI.Behaviour.Decorator.Blackboard;
using RSToolkit.AI.Behaviour.Decorator;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourEditorWindow : EditorWindow
    {
        private GUIStyle m_smallTextStyle, m_nodeCapsuleGray, m_nodeCapsuleFailed, m_nodeCapsuleStopRequested;
        private GUIStyle m_nestedBoxStyle;
        private const int NESTED_PADDING = 10;

        private Color m_defaultColor;
        private Color m_activeColor = new Color(1f, 1f, 1f, 1f);
        private Color m_inactiveColor = new Color(1f, 1f, 1f, 0.3f);

        public void Init()
        {       
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

        private void DrawKeyValue(string key, string value)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, m_smallTextStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(value, m_smallTextStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBlackboardKeyValues(string label, BehaviourBlackboard blackboard)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label(label, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            List<string> keys = blackboard.GetDataKeys();
            foreach (string key in keys)
            {
                DrawKeyValue(key, blackboard.Get(key).ToString());
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        /*
        private void DrawBehaviourTree(Debugger debugger)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Behaviour Tree:", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(nestedBoxStyle);
                DrawNodeTree(debugger.BehaviorTree, 0);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        */

        private void DrawNode(BehaviourNode node, int depth, bool connected)
        {
            float stopRequestedTime = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugTools.StopRequestAt));
            float stoppedTime = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugTools.LastStoppedAt));
            float alpha = node.State == BehaviourNode.NodeState.INACTIVE ? Mathf.Max(0.35f, Mathf.Pow(stoppedTime, 1.5f)) : 1.0f;
            bool failed = (stoppedTime > 0.25f && stoppedTime < 1.0f && (node.Result == null || !node.Result.Value) && node.State == BehaviourNode.NodeState.INACTIVE);
            bool stopRequested = stopRequestedTime > 0.25f && stopRequestedTime < 0.1f && node.State == BehaviourNode.NodeState.INACTIVE;

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(1f, 1f, 1f, alpha);

            string tagName;
            GUIStyle tagStyle = stopRequested ? m_nodeCapsuleStopRequested : (failed ? m_nodeCapsuleFailed : m_nodeCapsuleGray);
            bool drawLabel = !string.IsNullOrEmpty(node.DebugTools.GUIlabel);
            string label = node.DebugTools.GUIlabel;

            GUI.backgroundColor = node.DebugTools.NodeColor;

            tagName = node.DebugTools.GUItag;
        }

        private void DrawNodeTree(BehaviourNode node, int depth = 0, bool firstNode = true, float lastYPos = 0f)
        {
            GUI.color = (node.State == BehaviourNode.NodeState.ACTIVE) ? m_activeColor : m_inactiveColor;
            if (node.Parent?.Type == BehaviourNode.NodeType.DECORATOR)
            {
                // DrawSpacing();
            }

            bool drawConnected = node.Type != BehaviourNode.NodeType.DECORATOR || (node.Type == BehaviourNode.NodeType.DECORATOR && node.DebugTools.GUIcollapse);
            
        }


    }
}
#endif