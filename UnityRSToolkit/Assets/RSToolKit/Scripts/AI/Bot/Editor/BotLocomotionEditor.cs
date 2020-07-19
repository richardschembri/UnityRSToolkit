using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BotLocomotive), true)]
    public class BotLocomotionEditor : Editor
    {
        float m_currentSpeed = 0;
        BotLocomotive m_targetBotLocomotionManager;

        void OnEnable()
        {
            m_targetBotLocomotionManager = (BotLocomotive)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!m_targetBotLocomotionManager.DebugMode || !Application.isPlaying)
            {
                return;
            }
            m_currentSpeed = 0;

            if (m_targetBotLocomotionManager.enabled && m_targetBotLocomotionManager.CurrentLocomotionType != null)
            {
                m_currentSpeed = m_targetBotLocomotionManager.CurrentSpeed;
            }

            EditorGUILayout.LabelField($"Current Speed: {m_currentSpeed}", EditorStyles.boldLabel);
        }
    }
#endif
}