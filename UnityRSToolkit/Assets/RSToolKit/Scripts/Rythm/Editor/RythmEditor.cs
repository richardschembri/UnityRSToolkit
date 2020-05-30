using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.Rythm
{
#if UNITY_EDITOR
    [CustomEditor(typeof(RythmManager), true)]
    public class RythmEditor : Editor
    {
        RythmManager m_targetRythmManager;

        

        public override void OnInspectorGUI()
        {
           base.OnInspectorGUI();

           m_targetRythmManager = (RythmManager)target;
           if (!Application.isPlaying)
           {
               return;
           }

           EditorGUILayout.LabelField("Rythm", EditorStyles.boldLabel);

           EditorGUI.BeginDisabledGroup(m_targetRythmManager.HasPrompts());

           if (GUILayout.Button("Spawn Prompts"))
           {
               m_targetRythmManager.SpawnPrompts();
           };

           EditorGUI.EndDisabledGroup();

           if (!m_targetRythmManager.HasStarted())
           {
               if (GUILayout.Button("Start Rythm"))
               {
                   m_targetRythmManager.StartRythm();
               }
           }
           else
           {
               if (GUILayout.Button("Stop Rythm"))
               {
                   m_targetRythmManager.StopRythm();
               }
           }

        }
    }
#endif
}
