using RSToolkit.AI.Behaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.AI
{
    public abstract class AITreeEditorWindow : EditorWindow
    {
        public static Transform SelectedTransform { get; set; }
        protected Vector2 _scrollPosition = Vector2.zero;

        protected GUIStyle _smallTextStyle, _nodeCapsuleGray, _nodeCapsuleFailed, _nodeCapsuleStopRequested;
        protected GUIStyle _nestedBoxStyle;
        protected const int NESTED_PADDING = 10;

        protected Color _activeColor = new Color(1f, 1f, 1f, 1f);
        protected Color _inactiveColor = new Color(1f, 1f, 1f, 0.3f);
        protected Color _lineColor = new Color(0f, 0f, 0f, 1f);

        protected Color _defaultColor;

        /*
        private Color m_compositeColor = new Color(0.3f, 1f, 0.1f);
        private Color m_decoratorColor = new Color(0.3f, 1f, 1f);
        private Color m_taskColor = new Color(0.5f, 0.1f, 0.5f);
        private Color m_observingDecorator = new Color(0.9f, 0.9f, 0.6f);
        */

        public virtual void Init()
        {
            _nestedBoxStyle = new GUIStyle();
            _nestedBoxStyle.margin = new RectOffset(NESTED_PADDING, 0, 0, 0);

            _smallTextStyle = new GUIStyle();
            _smallTextStyle.font = EditorStyles.miniFont;

            _nodeCapsuleGray = (GUIStyle)"helpbox";
            _nodeCapsuleGray.normal.textColor = Color.black;

            _nodeCapsuleFailed = new GUIStyle(_nodeCapsuleGray);
            _nodeCapsuleFailed.normal.textColor = Color.red;
            _nodeCapsuleStopRequested = new GUIStyle(_nodeCapsuleGray);
            _nodeCapsuleStopRequested.normal.textColor = new Color(0.7f, 0.7f, 0.0f);

            _defaultColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        protected void DrawLegends()
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("Legends:", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("O: Started, x: Stopping, X: Stopped, T: Timers", _smallTextStyle);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawAdditionalStats(BehaviourDebugTools debugTools)
        {

        }

        protected void DrawStats(BehaviourDebugTools debugTools)
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

                    DrawAdditionalStats(debugTools);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        protected void DrawKeyValue(string key, string value)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, _smallTextStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(value, _smallTextStyle);
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawSpacing()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawNodeButtons(BehaviourNode node)
        {

        }

        protected virtual void DrawNodeStats(BehaviourNode node)
        {
            GUILayout.Label((node.DebugTools.StoppedCallCount > 0 ? node.Result.ToString() : "")
                + $" [O:{node.DebugTools.StartCallCount}][x:{node.DebugTools.StopCallCount}][X:{node.DebugTools.StoppedCallCount}][T:{node.GetAllTimerCount()}]", _smallTextStyle);

        }

        //private void DrawNode(BehaviourNode node, int depth, bool connected, bool last = false)
        protected void DrawNode(BehaviourNode node, string asciiIndent, bool connected, bool last = false)
        {
            float stopRequestedTime = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugTools.StopRequestAt));
            float stoppedTime = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugTools.LastStoppedAt));
            float alpha = node.State == BehaviourNode.NodeState.INACTIVE ? Mathf.Max(0.35f, Mathf.Pow(stoppedTime, 1.5f)) : 1.0f;
            bool failed = (stoppedTime > 0.25f && stoppedTime < 1.0f && (node.Result == null || !node.Result.Value) && node.State == BehaviourNode.NodeState.INACTIVE);
            bool stopRequested = stopRequestedTime > 0.25f && stopRequestedTime < 0.1f
									&& node.State == BehaviourNode.NodeState.INACTIVE;

            EditorGUILayout.BeginHorizontal();
            GUI.color = new Color(1f, 1f, 1f, alpha);

            GUIStyle tagStyle = stopRequested ? _nodeCapsuleStopRequested : (failed ? _nodeCapsuleFailed : _nodeCapsuleGray);
            bool drawLabel = !string.IsNullOrEmpty(node.DebugTools.GUIlabel);
            string label = node.DebugTools.GUIlabel;

            GUI.backgroundColor = node.DebugTools.NodeColor;
            //tagStyle.padding.left = last ? depth * 10 : (depth - 1) * 10;
            if (!drawLabel)
            {
                GUILayout.Label($"{asciiIndent}+- {node.DebugTools.GUItag}", tagStyle);
                //GUILayout.Label($"{node.DebugTools.GUItag}", tagStyle);
            }
            else
            {
                GUILayout.Label($"{asciiIndent}+- ({node.DebugTools.GUItag}) {node.DebugTools.GUIlabel}", tagStyle);
                //GUILayout.Label($"({node.DebugTools.GUItag}) {node.DebugTools.GUIlabel}", tagStyle);

                // Reset background color
                GUI.backgroundColor = Color.white;
            }

            GUILayout.FlexibleSpace();

            // Draw Buttons
            DrawNodeButtons(node);

            // Draw Stats
            DrawNodeStats(node);
            EditorGUILayout.EndHorizontal();

			/*
            // Draw the lines
            if (connected)
            {
				DrawHorizontalLines();
            }
			*/
        }

#region DrawLine
		protected void DrawVerticalLine(BehaviourNode node, Rect lastrect, float lastYPos){
            Handles.color = _lineColor;
            float lineY = lastrect.yMax + 6;
            if (node.Type != BehaviourNode.NodeType.DECORATOR)
            {
                lineY = lastrect.yMax - 4;
            }

            Handles.DrawLine(new Vector2(lastrect.xMin - 5, lastYPos + 4), new Vector2(lastrect.xMin - 5, lineY));
		}

		protected void DrawHorizontalLine(){
			Rect rect = GUILayoutUtility.GetLastRect();

			Handles.color = _lineColor;
			Handles.BeginGUI();
			float midY = 4 + (rect.yMin + rect.yMax) / 2f;
			Handles.DrawLine(new Vector2(rect.xMin - 5, midY), new Vector2(rect.xMin, midY));
			Handles.EndGUI();
		}
#endregion DrawLine

        protected void DrawNodeTree(BehaviourNode node, string asciiIndent,
										bool firstNode = true, float lastYPos = 0f,
										bool last = true)
        {

            GUI.color = (node.State == BehaviourNode.NodeState.ACTIVE) ? _activeColor : _inactiveColor;
			/*
            if (node.Parent?.Type == BehaviourNode.NodeType.DECORATOR)
            {
                DrawSpacing();
            }
			*/

            bool isConnected = node.Type != BehaviourNode.NodeType.DECORATOR
								|| (node.Type == BehaviourNode.NodeType.DECORATOR
										&& node.DebugTools.GUIcollapse);

            //DrawNode(node, depth, isConnected, last);
            DrawNode(node, asciiIndent, isConnected, last);
            asciiIndent += last ? "   " : "|  ";  // "--" : "|-"; //""; //
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
            if (nodeparent != null && nodeparent.Children.Count > 0
					&& Event.current.type == EventType.MouseUp
					&& Event.current.button == 0
					&& interactionRect.Contains(Event.current.mousePosition))
            {
                node.DebugTools.ToggleCollapse();
                Event.current.Use();
            }

			//DrawVerticalLine(node, lastrect, lastYPos);
            Handles.EndGUI();

            //if (node.Type != BehaviourNode.NodeType.TASK) depth++;

            //if (node.Type != BehaviourNode.NodeType.DECORATOR) EditorGUILayout.BeginVertical(_nestedBoxStyle);
            if (node.Type != BehaviourNode.NodeType.DECORATOR) EditorGUILayout.BeginVertical();

            if (nodeparent != null && nodeparent.Children.Count > 0 && !node.DebugTools.GUIcollapse)
            {
                lastYPos = lastrect.yMin + 16; // Set new Line position
                //string childasciiIndent = asciiIndent;
                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    //DrawNodeTree(nodeparent.Children[i], depth, i == 0, lastYPos, i == nodeparent.Children.Count - 1);
                    //DrawNodeTree(nodeparent.Children[i], ref childasciiIndent, i == 0, lastYPos, i == nodeparent.Children.Count - 1);
                    DrawNodeTree(nodeparent.Children[i], asciiIndent, i == 0, lastYPos, i == nodeparent.Children.Count - 1);
                    //childasciiIndent = asciiIndent;
                }

            }

            if (node.Type != BehaviourNode.NodeType.DECORATOR) EditorGUILayout.EndVertical();
        }

        protected void DrawTimeScale()
        {
            if (Time.timeScale <= 2.0f)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("TimeScale: ");
                Time.timeScale = EditorGUILayout.Slider(Time.timeScale, 0.0f, 2.0f);
                GUILayout.EndHorizontal();
            }
        }

        protected bool IsDebuggable(MonoBehaviour manager)
        {
            if (manager == null)
            {
                if (GUILayout.Button("Refresh"))
                {
                    OnSelectionChange();
                }
                EditorGUILayout.HelpBox("Please select an object", MessageType.Info);
                return false;
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Cannot use this utility in Editor Mode", MessageType.Info);
                return false;
            }
            /*
            else if (!manager.isActiveAndEnabled)
            {
                EditorGUILayout.HelpBox($"{manager.name} is disabled", MessageType.Info);
                return false;
            }
            */
            return true;
        }

        protected void DrawCommonGUI(BehaviourRootNode tree)
        {
            GUILayout.Space(10);
            DrawTimeScale();
            DrawNodeTree(tree, "");
            GUILayout.Space(10);
            DrawLegends();
            GUILayout.Space(10);
        }

        #region Mono Functions
        public virtual void OnSelectionChange()
        {

        }
        #endregion Mono Functions
        }
}
