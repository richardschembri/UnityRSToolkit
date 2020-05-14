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

        public static Transform SelectedTransform { get; set; }
        public static BehaviourManager SelectedManager { get; set; }
        private Vector2 m_scrollPosition = Vector2.zero;

        private GUIStyle m_smallTextStyle, m_nodeCapsuleGray, m_nodeCapsuleFailed, m_nodeCapsuleStopRequested;
        private GUIStyle m_nestedBoxStyle;
        private const int NESTED_PADDING = 10;

        private Color m_defaultColor;
        private Color m_activeColor = new Color(1f, 1f, 1f, 1f);
        private Color m_inactiveColor = new Color(1f, 1f, 1f, 0.3f);
        private Color m_lineColor = new Color(0f, 0f, 0f, 1f);

        private Color m_compositeColor = new Color(0.3f, 1f, 0.1f);
        private Color m_decoratorColor = new Color(0.3f, 1f, 1f);
        private Color m_taskColor = new Color(0.5f, 0.1f, 0.5f);
        private Color m_observingDecorator = new Color(0.9f, 0.9f, 0.6f);

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

        private void DrawStats(BehaviourDebugTools debugTools)
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Stats:", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    DrawKeyValue("Total Starts:", debugTools.GetTotalStartCallCount().ToString());
                    DrawKeyValue("Total Stops:", debugTools.GetTotalStopCallCount().ToString());
                    DrawKeyValue("Total Stopped:", debugTools.GetTotalStoppedCallCount().ToString());
                    DrawKeyValue("Active Timers:  ", debugTools.GetTotalActiveTimers().ToString());
                    DrawKeyValue("Timer Pool Size:  ", debugTools.GetTotalTimers().ToString());
                    //DrawKeyValue("Active Update Observers:  ", behaviorTree.Clock.NumUpdateObservers.ToString());
                    //DrawKeyValue("Active Blackboard Observers:  ", behaviorTree.Blackboard.NumObservers.ToString());
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
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
            if (blackboard == null)
            {
                return;
            }
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

        private void DrawSpacing()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNode(BehaviourNode node, int depth, bool connected)
        {
            float stopRequestedTime = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugTools.StopRequestAt));
            float stoppedTime = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugTools.LastStoppedAt));
            float alpha = node.State == BehaviourNode.NodeState.INACTIVE ? Mathf.Max(0.35f, Mathf.Pow(stoppedTime, 1.5f)) : 1.0f;
            bool failed = (stoppedTime > 0.25f && stoppedTime < 1.0f && (node.Result == null || !node.Result.Value) && node.State == BehaviourNode.NodeState.INACTIVE);
            bool stopRequested = stopRequestedTime > 0.25f && stopRequestedTime < 0.1f && node.State == BehaviourNode.NodeState.INACTIVE;

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(1f, 1f, 1f, alpha);

            GUIStyle tagStyle = stopRequested ? m_nodeCapsuleStopRequested : (failed ? m_nodeCapsuleFailed : m_nodeCapsuleGray);
            bool drawLabel = !string.IsNullOrEmpty(node.DebugTools.GUIlabel);
            string label = node.DebugTools.GUIlabel;

            GUI.backgroundColor = node.DebugTools.NodeColor;
            tagStyle.padding.left = depth * 10;
            if (!drawLabel)
            {
                GUILayout.Label(node.DebugTools.GUItag, tagStyle);
            }
            else
            {
                GUILayout.Label($"({node.DebugTools.GUItag}) {node.DebugTools.GUIlabel}", tagStyle);
                // Reset background color
                GUI.backgroundColor = Color.white;
            }

            GUILayout.FlexibleSpace();

            // Draw Buttons
            if (node.State == BehaviourNode.NodeState.ACTIVE)
            {
                if (GUILayout.Button("stop", EditorStyles.miniButton))
                {
                    node.RequestStopNode();
                }
                else if (node is BehaviourRootNode)
                {
                    GUI.color = new Color(1f, 1f, 1f, 1f);
                    if (GUILayout.Button("start", EditorStyles.miniButton))
                    {
                        node.StartNode();
                    }
                    GUI.color = new Color(1f, 1f, 1f, 0.3f);
                }
              
            }
            // Draw Stats
            GUILayout.Label((node.DebugTools.StoppedCallCount > 0 ? node.Result.ToString() : "")
                + $" | O:{node.DebugTools.StartCallCount} , x:{node.DebugTools.StopCallCount} , X:{node.DebugTools.StoppedCallCount}", m_smallTextStyle);
            EditorGUILayout.EndHorizontal();

            // Draw the lines
            if (connected)
            {
                Rect rect = GUILayoutUtility.GetLastRect();

                Handles.color = new Color(0f, 0f, 0f, 1f);
                Handles.BeginGUI();
                float midY = 4 + (rect.yMin + rect.yMax) / 2f;
                Handles.DrawLine(new Vector2(rect.xMin - 5, midY), new Vector2(rect.xMin, midY));
                Handles.EndGUI();
            }
        }

        private void DrawNodeTree(BehaviourNode node, int depth = 0, bool firstNode = true, float lastYPos = 0f)
        {
            GUI.color = (node.State == BehaviourNode.NodeState.ACTIVE) ? m_activeColor : m_inactiveColor;
            if (node.Parent?.Type == BehaviourNode.NodeType.DECORATOR)
            {
                DrawSpacing();
            }

            bool isConnected = node.Type != BehaviourNode.NodeType.DECORATOR || (node.Type == BehaviourNode.NodeType.DECORATOR && node.DebugTools.GUIcollapse);

            DrawNode(node, depth, isConnected);
            var lastrect = GUILayoutUtility.GetLastRect();

            if (firstNode)
            {
                lastYPos = lastrect.yMin;
            }

            // Draw the lines
            Handles.BeginGUI();
            var interactionRect = new Rect(lastrect);
            interactionRect.width = 100;
            interactionRect.y += 8;

            var nodeparent = node as BehaviourParentNode;
            if (nodeparent != null && nodeparent.Children.Count > 0 && Event.current.type == EventType.MouseUp && Event.current.button == 0 && interactionRect.Contains(Event.current.mousePosition))
            {
                node.DebugTools.ToggleCollapse();
                Event.current.Use();
            }

            Handles.color = m_lineColor;
            float lineY = lastrect.yMax + 6;
            if (node.Type != BehaviourNode.NodeType.DECORATOR)
            {
                lineY = lastrect.yMax - 4;
            }

            Handles.DrawLine(new Vector2(lastrect.xMin - 5, lastYPos + 4), new Vector2(lastrect.xMin - 5, lineY));
            Handles.EndGUI();

            //if (node.Type == BehaviourNode.NodeType.DECORATOR) depth++;
            if (node.Type != BehaviourNode.NodeType.TASK) depth++;

            if (node.Type != BehaviourNode.NodeType.DECORATOR) EditorGUILayout.BeginVertical(m_nestedBoxStyle);
            if (nodeparent != null && nodeparent.Children.Count > 0 && !node.DebugTools.GUIcollapse)
            {
                lastYPos = lastrect.yMin + 16; // Set new Line position

                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    DrawNodeTree(nodeparent.Children[i], depth, i == 0, lastYPos);
                }
            }

            if (node.Type != BehaviourNode.NodeType.DECORATOR) EditorGUILayout.EndVertical();
        }


        public void OnSelectionChange()
        {
            SelectedTransform = Selection.activeTransform;
            if (SelectedTransform != null) SelectedManager = SelectedTransform.GetComponentInChildren<BehaviourManager>();

            Repaint();
        }
        public void OnGUI()
        {
            GUI.color = m_defaultColor;
            GUILayout.Toggle(false, "Behaviour Debugger", GUI.skin.FindStyle("LODLevelNotifyText"));
            GUI.color = Color.white;

            var newManager = (BehaviourManager)EditorGUILayout.ObjectField("Selected Debugger:", SelectedManager, typeof(BehaviourManager), true);
            if (newManager != SelectedManager)
            {
                SelectedManager = newManager;
                if (newManager != null) SelectedTransform = SelectedManager.transform;
            }

            if (SelectedManager == null)
            {
                EditorGUILayout.HelpBox("Please select an object", MessageType.Info);
                return;
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Cannot use this utility in Editor Mode", MessageType.Info);
                return;
            }
            /*
            else if (SelectedManager. == null)
            {
                EditorGUILayout.HelpBox("BehavorTree is null", MessageType.Info);
                return;
            }*/

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            GUILayout.BeginHorizontal();
            DrawBlackboardKeyValues("Blackboard:", SelectedManager.CurrentBlackboard);

            DrawStats(SelectedManager.CurrentTree.DebugTools);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (Time.timeScale <= 2.0f)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("TimeScale: ");
                Time.timeScale = EditorGUILayout.Slider(Time.timeScale, 0.0f, 2.0f);
                GUILayout.EndHorizontal();
            }

            DrawNodeTree(SelectedManager.CurrentTree);
            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();

            Repaint();
        }

    }

    [CustomEditor(typeof(BehaviourManager))]
    public class BehaviourManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Behaviour Debugger", EditorStyles.centeredGreyMiniLabel);
            if (GUILayout.Button("Open Debugger"))
            {
                BehaviourEditorWindow.SelectedManager = ((BehaviourManager)target);
                BehaviourEditorWindow.SelectedTransform = BehaviourEditorWindow.SelectedManager.transform;
                BehaviourEditorWindow.ShowWindow();
            }
        }
    }
}
#endif