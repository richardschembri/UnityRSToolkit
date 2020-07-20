using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Bot), true)]
    public class BotEditor : Editor
    {
        Bot _targetBot;
        BotLocomotive _targetBotLocomotive ;
        Object m_waypoint;
        bool m_fullspeed = false;

        void OnEnable()
        {
            _targetBot = (Bot)target;
            if(target is BotLocomotive){
                _targetBotLocomotive = (BotLocomotive)target;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (!_targetBot.DebugMode || !Application.isPlaying)
            {
                return;
            }
            
            EditorGUILayout.LabelField("Bot States", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Interaction State");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(_targetBot.CurrentInteractionState);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            if (_targetBotLocomotive != null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Movement State");
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup(_targetBotLocomotive.CurrentFState);  
                EditorGUI.EndDisabledGroup();
                if (_targetBotLocomotive.CurrentFState != BotLocomotive.FStatesLocomotion.NotMoving)
                {
                    if (GUILayout.Button("Stop Moving"))
                    {
                        _targetBotLocomotive.StopMoving();
                    }
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Bot Controls", EditorStyles.boldLabel);
                if(_targetBotLocomotive.BotWanderManagerComponent != null){
                    if (_targetBotLocomotive.BotWanderManagerComponent.IsWandering())
                    {
                        if (GUILayout.Button("Stop Wandering"))
                        {
                            _targetBotLocomotive.BotWanderManagerComponent.StopWandering();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Start Wandering"))
                        {
                            _targetBotLocomotive.BotWanderManagerComponent.Wander();
                        }
                    }
                }
                EditorGUILayout.LabelField("Move to Waypoint", EditorStyles.miniBoldLabel);
                GUILayout.BeginHorizontal();
                
                m_waypoint = EditorGUILayout.ObjectField(m_waypoint, typeof(Transform), true);
                EditorGUI.BeginDisabledGroup(m_waypoint == null);
                m_fullspeed = EditorGUILayout.Toggle("Full Speed", m_fullspeed);
                if (GUILayout.Button("Move to"))
                {
                    _targetBotLocomotive.FocusOnTransform((Transform)m_waypoint);
                    _targetBotLocomotive.MoveToTarget( BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, m_fullspeed);
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
            EditorGUILayout.EnumPopup(m_targetBotFlyable.CurrentFlyableState);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Bot Flyable Controls", EditorStyles.boldLabel);
            switch (m_targetBotFlyable.CurrentFlyableState)
            {
                case BotFlyable.FStatesFlyable.Flying:
                    if (GUILayout.Button("Land", EditorStyles.miniButton))
                    {
                        m_targetBotFlyable.Land();
                    }
                    break;
                case BotFlyable.FStatesFlyable.NotFlying:
                    if (GUILayout.Button("Take Off", EditorStyles.miniButton))
                    {
                        m_targetBotFlyable.TakeOff();
                    }
                    break;
                default:
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button(m_targetBotFlyable.CurrentFlyableState.ToString(), EditorStyles.miniButton);
                    EditorGUI.EndDisabledGroup();
                    break;
            }
            
        }
    }

#endif
}