using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RSToolkit.Character
{
#if UNITY_EDITOR
    [CustomEditor(typeof(RSCharacterController), true)]
    public class RSCharacterControllerEditor : Editor
    {
        RSCharacterController targetController;
        void OnEnable()
        {
            // _targetBot = (Bot)target;
            targetController = (RSCharacterController)target;

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            //if (!_targetBot.DebugMode || !Application.isPlaying)
            if (!targetController.DebugMode || !Application.isPlaying)
            {
                return;
            }
            EditorGUILayout.LabelField($"Debug Values:" , EditorStyles.largeLabel);
            EditorGUILayout.LabelField($"Current Speed Horizontal: {targetController.CurrentSpeedHorizontal}/{targetController.SettingsLocomotion.MaxSpeedHorizontal}({targetController.CurrentSpeedPercentHorizontal}%)", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Direction Axis: {targetController.DirectionAxis }", EditorStyles.boldLabel);
        }
    }
#endif
}
