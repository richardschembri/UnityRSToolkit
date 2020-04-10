using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.AI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Bot), true)]
    public class BotEditor : Editor
    {
        Bot m_targetBot;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (!Application.isPlaying)
            {
                return;
            }
            m_targetBot = (Bot)target;
            EditorGUILayout.LabelField("Bot States", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Interaction State");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(m_targetBot.CurrentInteractionState);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            if (m_targetBot.IsMoveable())
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Movement State");
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup(m_targetBot.GetMovementState());
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("Bot Controls", EditorStyles.boldLabel);
            if (m_targetBot.IsWandering())
            {
                if (GUILayout.Button("Stop Wandering"))
                {
                    m_targetBot.StopWandering();
                }
            }
            else
            {
                if (GUILayout.Button("Start Wandering"))
                {
                    m_targetBot.Wander();
                }
            }
        }
    }
    [CustomEditor(typeof(BotFlyable), true)]
    public class BotFlyableEditor : BotEditor
    {
        BotFlyable m_targetBotFlyable;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();    
            if (!Application.isPlaying)
            {
                return;
            }
            m_targetBotFlyable = (BotFlyable)target;

            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Flyable State");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(m_targetBotFlyable.CurrentState);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Bot Flyable Controls", EditorStyles.boldLabel);
            switch (m_targetBotFlyable.CurrentState)
            {
                case BotFlyable.FlyableStates.Flying:
                    if (GUILayout.Button("Land", EditorStyles.miniButton))
                    {
                        m_targetBotFlyable.Land();
                    }
                    break;
                case BotFlyable.FlyableStates.NotFlying:
                    if (GUILayout.Button("Take Off", EditorStyles.miniButton))
                    {
                        m_targetBotFlyable.TakeOff();
                    }
                    break;
                default:
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button(m_targetBotFlyable.CurrentState.ToString(), EditorStyles.miniButton);
                    EditorGUI.EndDisabledGroup();
                    break;
            }
            
        }
    }

#endif
}