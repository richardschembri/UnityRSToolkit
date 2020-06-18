using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.AI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BotLocomotion), true)]
    public class BotLocomotionEditor : Editor
    {
        float m_currentSpeed = 0;
        BotLocomotion m_targetBotLocomotion;

        void OnEnable()
        {
            m_targetBotLocomotion = (BotLocomotion)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (!m_targetBotLocomotion.DebugMode || !Application.isPlaying)
            {
                return;
            }
            m_currentSpeed = 0;

            if (m_targetBotLocomotion.enabled)
            {
                m_currentSpeed = m_targetBotLocomotion.CurrentSpeed;
            }

            EditorGUILayout.LabelField($"Current Speed: {m_currentSpeed}", EditorStyles.boldLabel);
        }
    }
#endif
}