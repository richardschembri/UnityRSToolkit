#if UNITY_EDITOR
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Locomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSToolkit.AI
{
    public class BotsEditorWindow : EditorWindow
    {
        private Bot[] _bots;

        [MenuItem("Tools/RSToolkit/AI/Bot")]
        public static void ShowWindow()
        {
            BotsEditorWindow window = (BotsEditorWindow)GetWindow(typeof(BotsEditorWindow), false, "Bot Manager");
            window.Show();
        }

        public void OnGUI()
        {
            _bots = FindObjectsOfType<Bot>();
            for(int i = 0; i < _bots.Length; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button($"{_bots[i].name}", GUILayout.Width(200)))
                    {
                        Selection.objects = new Object[] { _bots[i].gameObject };
                    }
                    _bots[i].DebugMode = EditorGUILayout.Toggle("Debug Mode", _bots[i].DebugMode, GUILayout.Width(170));
                    if (Application.isPlaying)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.EnumPopup(_bots[i].CurrentInteractionState);                   
                        if(_bots[i] is BotLocomotive){
                            
                            EditorGUILayout.EnumPopup(((BotLocomotive)_bots[i]).CurrentFState);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    
                    
                }
                GUILayout.EndHorizontal();
            }
        }

    }
}
#endif