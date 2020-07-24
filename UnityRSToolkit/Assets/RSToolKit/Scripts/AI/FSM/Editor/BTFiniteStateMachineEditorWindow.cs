using RSToolkit.AI.Behaviour;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.AI.FSM
{
    public class BTFiniteStateMachineEditorWindow : AITreeEditorWindow
    {
        public static BTFiniteStateMachineManager SelectedManager { get; set; }

        [MenuItem("Tools/RSToolkit/AI/BTFiniteStateMachine")]
        public static void ShowWindow()
        {
            BTFiniteStateMachineEditorWindow window = (BTFiniteStateMachineEditorWindow)EditorWindow.GetWindow(typeof(BTFiniteStateMachineEditorWindow), false, "BT Finite State Machine");
            window.Init(); // Why doesnt this work
            window.Show();
        }

        public void OnSelectionChange()
        {
            SelectedTransform = Selection.activeTransform;
            if (SelectedTransform != null) SelectedManager = SelectedTransform.GetComponentInChildren<BTFiniteStateMachineManager>();
            Init();
            Repaint();
        }

        public void OnGUI()
        {
            GUI.color = _defaultColor;
            GUILayout.Toggle(false, "BT Finite State Machine Debugger",
								GUI.skin.FindStyle("LODLevelNotifyText"));
            GUI.color = Color.white;

            var newManager = (BTFiniteStateMachineManager)EditorGUILayout.ObjectField("Selected Debugger:", SelectedManager, typeof(BTFiniteStateMachineManager), true);

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
            DrawStats(SelectedManager.FSMBehaviourtree.DebugTools);
            GUILayout.EndHorizontal();
            DrawCommonGUI(SelectedManager.FSMBehaviourtree);
            EditorGUILayout.EndScrollView();

            Repaint();
        }

    }

    [CustomEditor(typeof(BTFiniteStateMachineManager))]
    public class BTFiniteStateMachineManagerEditor : Editor
    {
        BTFiniteStateMachineManager _targetBTFiniteStateMachine;
        ReadOnlyCollection<IBTFiniteStateMachine> _fsmList;

        public override void OnInspectorGUI()
        {
            GUILayout.Label("BT Finite State Machine Debugger", EditorStyles.centeredGreyMiniLabel);

            _targetBTFiniteStateMachine = (BTFiniteStateMachineManager)target;
            _fsmList = _targetBTFiniteStateMachine.FSMList;
            EditorGUILayout.LabelField("FSM States", EditorStyles.largeLabel);
            for (int i = 0; i < _fsmList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_fsmList[i].GetName(), EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(_fsmList[i].GetCurrentStateText());
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
            }


            if (GUILayout.Button("Open Debugger"))
            {
                BTFiniteStateMachineEditorWindow.SelectedManager = ((BTFiniteStateMachineManager)target);
                BTFiniteStateMachineEditorWindow.SelectedTransform = BTFiniteStateMachineEditorWindow.SelectedManager.transform;
                BTFiniteStateMachineEditorWindow.ShowWindow();
            }
        }
    }
}
