using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.UI.Paging;
using RSToolkit.UI.Controls;

public class PopupPage : UIPage
{
    public Toggle isStaticOpen;
    public UIPopup staticPopup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       isStaticOpen.isOn = staticPopup.IsOpen(); 
    }
}
