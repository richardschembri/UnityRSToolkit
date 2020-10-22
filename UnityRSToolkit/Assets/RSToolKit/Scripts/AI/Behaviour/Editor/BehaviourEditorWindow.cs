#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RSToolkit.AI.Behaviour.Decorator.Blackboard;
using RSToolkit.AI.Behaviour.Decorator;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourEditorWindow : AITreeEditorWindow
    {
      
        public static BehaviourManager SelectedManager { get; set; }
           
        [MenuItem("Tools/RSToolkit/AI/BehaviourTree")]
        public static void ShowWindow()
        {
            BehaviourEditorWindow window = (BehaviourEditorWindow)EditorWindow.GetWindow(typeof(BehaviourEditorWindow), false, "Behaviour Tree");
            window.Init(); // Why doesnt this work
            window.Show();
        }
        protected override void DrawAdditionalStats(BehaviourDebugTools debugTools)
        {            
            if(SelectedManager.CurrentTree.LastResyncNodeIDs != null)
            {
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label("Last Resync Leaves:", EditorStyles.boldLabel);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {                        
                        for (int i = 0; i < SelectedManager.CurrentTree.LastResyncNodeIDs.Length; i++)
                        {
                            DrawKeyValue(SelectedManager.CurrentTree.GetNodeByID(SelectedManager.CurrentTree.LastResyncNodeIDs[i]).Name, 
                                            SelectedManager.CurrentTree.LastResyncNodeIDs[i]);
                        }
                    }
                    EditorGUILayout.EndVertical();                    
                }
                EditorGUILayout.EndVertical();
            }
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

        protected override void DrawNodeButtons(BehaviourNode node)
        {
            if (node.State == BehaviourNode.NodeState.ACTIVE)
            {
                if (GUILayout.Button("stop", EditorStyles.miniButton))
                {
                    node.RequestStopNode();
                }
            }
            else if (node is BehaviourRootNode && node.State == BehaviourNode.NodeState.INACTIVE)
            {
                GUI.color = new Color(1f, 1f, 1f, 1f);
                if (GUILayout.Button("start", EditorStyles.miniButton))
                {
                    node.StartNode();
                }
                GUI.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }

        public override void OnSelectionChange()
        {
            SelectedTransform = Selection.activeTransform;
            if (SelectedTransform != null) SelectedManager = SelectedTransform.GetComponentInChildren<BehaviourManager>();
            Init();
            Repaint();
        }

        public void OnGUI()
        {
            GUI.color = _defaultColor;
            GUILayout.Toggle(false, "Behaviour Debugger", GUI.skin.FindStyle("LODLevelNotifyText"));
            GUI.color = Color.white;

            var newManager = (BehaviourManager)EditorGUILayout.ObjectField("Selected Debugger:", SelectedManager, typeof(BehaviourManager), true);
            if (newManager != SelectedManager)
            {
                SelectedManager = newManager;
                if (newManager != null) SelectedTransform = SelectedManager.transform;
            }

            if (!IsDebuggable(SelectedManager))
            {
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            GUILayout.BeginHorizontal();
            DrawBlackboardKeyValues("Blackboard:", SelectedManager.CurrentBlackboard);
            DrawStats(SelectedManager.CurrentTree.DebugTools);
            GUILayout.EndHorizontal();
            DrawCommonGUI(SelectedManager.CurrentTree);
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
