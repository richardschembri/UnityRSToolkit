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
        Object m_waypoint;
        bool m_fullspeed = false;

        void OnEnable()
        {
            m_targetBot = (Bot)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (!m_targetBot.DebugMode || !Application.isPlaying)
            {
                return;
            }
            
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
                if (m_targetBot.GetMovementState() != BotLocomotion.LocomotionState.NotMoving)
                {
                    if (GUILayout.Button("Stop Moving"))
                    {
                        m_targetBot.StopMoving();
                    }
                }
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
            if (m_targetBot.IsMoveable())
            {
                EditorGUILayout.LabelField("Move to Waypoint", EditorStyles.miniBoldLabel);
                GUILayout.BeginHorizontal();
                
                m_waypoint = EditorGUILayout.ObjectField(m_waypoint, typeof(Transform), true);
                EditorGUI.BeginDisabledGroup(m_waypoint == null);
                m_fullspeed = EditorGUILayout.Toggle("Full Speed", m_fullspeed);
                if (GUILayout.Button("Move to"))
                {
                    m_targetBot.FocusOnTransform((Transform)m_waypoint);
                    m_targetBot.MoveToTarget( BotLocomotion.StopMovementConditions.WITHIN_PERSONAL_SPACE, m_fullspeed);
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
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

            m_targetBotFlyable = (BotFlyable)target;
            if (!m_targetBotFlyable.DebugMode || !Application.isPlaying)
            {
                return;
            }

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