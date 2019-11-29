using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Controls;
using RSToolkit.UI.Controls;
using RSToolkit.UI.Paging;
using RSToolkit.Helpers;

public class ScrollListPage : UIPage
{
    public UIListBox VerticalListBox;

    private Spawner m_buttonSpawner;
    public Spawner ButtonSpawner{
        get{
            if(m_buttonSpawner == null){
                m_buttonSpawner = VerticalListBox.GetComponentInChildren<Spawner>();
            }
            return m_buttonSpawner;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       VerticalListBox.OnShiftMostVertical.AddListener(ShiftTest);
    }

    public void AddButtons(){
       VerticalListBox.Refresh();
       VerticalListBox.TurnOffCulling();
       for(int i = 0; i < 5; i++){
           var b = VerticalListBox.AddListItem().GetComponent<Button>();
           b.name = string.Format("Button {0}", VerticalListBox.ListItemSpawner.SpawnedGameObjects.Count);
           b.GetComponentInChildren<Text>().text = b.name;
       }
       VerticalListBox.TurnOnCulling();
       VerticalListBox.Refresh();
    }

    void ShiftTest(RectTransform li, RectTransformHelpers.VerticalPosition horpos){
        Debug.Log(li.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
