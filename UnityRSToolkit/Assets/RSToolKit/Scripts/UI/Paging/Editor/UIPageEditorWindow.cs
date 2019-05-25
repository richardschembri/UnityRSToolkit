using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UIPageEditorWindow : EditorWindow
{
    [MenuItem("RSToolKit/UI Page Editor Window")]
    public static void Init()
    {
        var window = GetWindow(typeof(UIPageEditorWindow), false, "PageChanger");
        window.minSize = new Vector2(450, 500);
        window.maxSize = new Vector2(450, 500);
    } 

    private void GenerateHeader()
    {
        
    }

}
