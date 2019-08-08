using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Controls;
using RSToolkit.UI.Controls;
using RSToolkit.UI.Paging;

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
    }

    public void AddButtons(){
       VerticalListBox.Refresh();
       VerticalListBox.TurnOffCulling();
       for(int i = 0; i < 5; i++){
           var b = VerticalListBox.AddListItem().GetComponent<Button>();
           b.GetComponentInChildren<Text>().text = string.Format("Button {0}", VerticalListBox.ListItemSpawner.SpawnedGameObjects.Count);
       }
       VerticalListBox.TurnOnCulling();
       VerticalListBox.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
