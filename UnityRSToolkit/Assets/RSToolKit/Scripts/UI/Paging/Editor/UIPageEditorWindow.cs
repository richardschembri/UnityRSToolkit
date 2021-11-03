namespace RSToolkit.UI.Paging.Editor {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using RSToolkit.UI.Paging;

    public class UIPageEditorWindow : EditorWindow
    {
        private int[] _displayPageIndexes;
        private UIPageManager[] _pageManagers;
        private UIPageManager[] _PageManagers
        {
            get
            {
                if(_pageManagers == null || _pageManagers.Length == 0)
                {
                    _pageManagers = FindObjectsOfType<UIPageManager>(true);
                    _displayPageIndexes = new int[_pageManagers.Length];
                    for(int i = 0; i < _displayPageIndexes.Length; i++){
                        _displayPageIndexes[i] = 0;
                    }
                Debug.Log($"asdadasdsad: {_displayPageIndexes.Length}");
                }
                    
                return _pageManagers;
            }
        }

        [MenuItem("Tools/RSToolkit/UI Page Editor Window")]
        public static void Init()
        {
            var window = GetWindow(typeof(UIPageEditorWindow), false, "UI Page Editor Window");
            window.minSize = new Vector2(450, 100);
            window.maxSize = new Vector2(450, 500);
            
        } 

        private void DisplayPages()
        {
            if(_PageManagers == null){
                return;
            }
            for(int pmi = 0; pmi < _PageManagers.Length; pmi++){
                if(_PageManagers[pmi] == null){
                    _pageManagers = null;
                    break;
                }
                Debug.Log($"Idndeded: {_displayPageIndexes.Length}");
                if (!_PageManagers[pmi].Initialized ) _PageManagers[pmi].Init();
                if (_PageManagers[pmi].Core == null ) _PageManagers[pmi].Init(true);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{_PageManagers[pmi].name} Pages :", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
                var pages = _PageManagers[pmi].Core.Pages;
                for (int i = 0; i < pages.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    var pi = pages[i];

                    for (int j = 0; j < pages.Length; j++)
                    {
                        var pj = pages[j];

                        pj.gameObject.SetActive(j == _displayPageIndexes[pmi]);
                    }

                    if (EditorGUILayout.Toggle(string.Format("- {0}", pi.GetHeader()), pi.gameObject.activeSelf))
                    {
                        _displayPageIndexes[pmi] = i;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Space(10);
            }
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                _pageManagers = null;
                EditorGUILayout.LabelField("Application is running", GUILayout.Height(300));
                return; 
            }
            DisplayPages();
        }

    }
}