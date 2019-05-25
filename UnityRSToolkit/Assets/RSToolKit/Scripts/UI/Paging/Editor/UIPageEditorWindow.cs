namespace RSToolkit.UI.Paging.Editor {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using RSToolkit.UI.Paging;

    public class UIPageEditorWindow : EditorWindow
    {
        private int m_displayPageIndex = 0;
        private UIPageManager m_pageManager;
        private UIPageManager m_PageManager
        {
            get
            {
                if(m_pageManager == null)
                {
                    m_pageManager = FindObjectOfType<UIPageManager>();
                }
                    
                return m_pageManager;
            }
        }

        [MenuItem("RSToolKit/UI Page Editor Window")]
        public static void Init()
        {
            var window = GetWindow(typeof(UIPageEditorWindow), false, "UI Page Editor Window");
            window.minSize = new Vector2(450, 100);
            window.maxSize = new Vector2(450, 500);
            
        } 

        private void DisplayPages()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pages :");
            EditorGUILayout.EndHorizontal();
            var pages = m_PageManager.GetPages();
            for (int i = 0; i < pages.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var pi = pages[i];

                for (int j = 0; j < pages.Length; j++)
                {
                    var pj = pages[j];

                    pj.gameObject.SetActive(j == m_displayPageIndex);
                }

                if (EditorGUILayout.Toggle(string.Format("- {0}", pi.GetHeader()), pi.gameObject.activeSelf))
                {
                    m_displayPageIndex = i;
                }
                EditorGUILayout.EndHorizontal();
            }

        }

        private void OnGUI()
        {
            DisplayPages();
        }

    }
}