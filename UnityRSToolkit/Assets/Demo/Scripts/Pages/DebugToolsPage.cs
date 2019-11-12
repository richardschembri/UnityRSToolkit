using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.UI.Paging;
using RSToolkit.Debug.UI.Controls;

public class DebugToolsPage : UIPage
{
    public DebugLog DebugLogGO; 

    public void AddRandomLog(){
        DebugLogGO.AppendLog("Log!");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
