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
        Object _waypoint;
        bool _fullspeed = false;
        float _interactionCooldown = 0f;
        float _currentSpeed = 0;

        string _debugDistanceType = "";

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

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Network Type");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(_targetBot.NetworkType);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Bot States", EditorStyles.boldLabel);

            if (_targetBot.InteractableCooldown > 0f)
            {
                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField("Interaction Cooldown");
                _interactionCooldown = _targetBot.CanInteractFromTime - Time.time;
                if(_interactionCooldown < 0f)
                {
                    _interactionCooldown = 0f;
                }
                EditorGUILayout.Slider(_interactionCooldown, 0f, _targetBot.InteractableCooldown);
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
            }
            
            EditorGUILayout.LabelField("Noticed Transforms", EditorStyles.boldLabel);
            foreach (var nt in _targetBot.NoticedTransforms)
            {                             
                if (GUILayout.Button(nt.name, GUILayout.Width(200)))
                {
                    Selection.objects = new Object[] { nt.gameObject };
                }

            }
            
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

                _currentSpeed = 0;

                if (_targetBotLocomotive.enabled && _targetBotLocomotive.CurrentLocomotionType != null)
                {
                    _currentSpeed = _targetBotLocomotive.CurrentSpeed;
                }

                EditorGUILayout.LabelField($"Current Speed: {_currentSpeed}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"Close to surface by [{_targetBotLocomotive.IsCloseToSurface()}]", EditorStyles.boldLabel);
                


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
                            _targetBotLocomotive.BotWanderManagerComponent.Wander(false);
                        }
                    }
                }
                EditorGUILayout.LabelField("Move to Waypoint", EditorStyles.miniBoldLabel);
                GUILayout.BeginHorizontal();
                
                _waypoint = EditorGUILayout.ObjectField(_waypoint, typeof(Transform), true);
                EditorGUI.BeginDisabledGroup(_waypoint == null);
                _fullspeed = EditorGUILayout.Toggle("Full Speed", _fullspeed);
                if (GUILayout.Button("Move to"))
                {
                    _targetBotLocomotive.FocusOnTransform((Transform)_waypoint);
                    // _targetBotLocomotive.MoveToTarget( BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, _fullspeed);
                    _targetBotLocomotive.MoveToTarget(Bot.DistanceType.PERSONAL_SPACE, _fullspeed);
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();

                Bot.DistanceType? dt = _targetBotLocomotive.GetDistanceTypeFromFocusedPosition();
                if (dt != null)
                {
                    float sqrMagnitude = 0f;
                    switch (dt)
                    {
                        case Bot.DistanceType.AT_POSITION:
                            sqrMagnitude = _targetBotLocomotive.SqrAtPositionErrorMargin;
                            break;
                        case Bot.DistanceType.PERSONAL_SPACE:
                            sqrMagnitude = _targetBotLocomotive.SqrPersonalSpaceMagnitude;
                            break;
                        case Bot.DistanceType.INTERACTION:
                            sqrMagnitude = _targetBotLocomotive.SqrPersonalSpaceMagnitude;
                            break;
                        case Bot.DistanceType.AWARENESS:
                            sqrMagnitude = _targetBotLocomotive.SqrAwarenessMagnitude;
                            break;
                    }
                    _debugDistanceType = $"Move to : {dt.ToString()}|{Vector3.SqrMagnitude(_targetBotLocomotive.FocusedOnPosition.Value - _targetBotLocomotive.transform.position)}/{sqrMagnitude}|{Vector3.Distance(_targetBotLocomotive.FocusedOnPosition.Value, _targetBotLocomotive.transform.position)}/{sqrMagnitude}";
                    
                }
                EditorGUILayout.LabelField(_debugDistanceType, EditorStyles.boldLabel);
            }
        }
    }

    [CustomEditor(typeof(BotGround), true)]
    public class BotGroundEditor : BotEditor{
        BotGround _targetBotGround ;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _targetBotGround = (BotGround)target;
            if (!Application.isPlaying || !_targetBotGround.DebugMode  )
            {
                return;
            }
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ground State");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(_targetBotGround.CurrentStatesGround);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
    }

    [CustomEditor(typeof(BotFlyable), true)]
    public class BotFlyableEditor : BotEditor
    {
        BotFlyable _targetBotFlyable;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _targetBotFlyable = (BotFlyable)target;
            if (!Application.isPlaying || !_targetBotFlyable.DebugMode)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Flyable State");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(_targetBotFlyable.CurrentFlyableState);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Bot Flyable Controls", EditorStyles.boldLabel);
            switch (_targetBotFlyable.CurrentFlyableState)
            {
                case BotFlyable.FStatesFlyable.Flying:
                    if (GUILayout.Button("Land", EditorStyles.miniButton))
                    {
                        _targetBotFlyable.Land();
                    }
                    break;
                case BotFlyable.FStatesFlyable.NotFlying:
                    if (GUILayout.Button("Take Off", EditorStyles.miniButton))
                    {
                        _targetBotFlyable.TakeOff();
                    }
                    break;
                default:
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button(_targetBotFlyable.CurrentFlyableState.ToString(), EditorStyles.miniButton);
                    EditorGUI.EndDisabledGroup();
                    break;
            }
            
        }
    }

#endif
}