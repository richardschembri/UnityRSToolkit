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
    public UIListBox HorizontalListBox;

    private Spawner m_verticalButtonSpawner;
    public Spawner VerticalButtonSpawner{
        get{
            if(m_verticalButtonSpawner == null){
                m_verticalButtonSpawner = VerticalListBox.GetComponentInChildren<Spawner>();
            }
            return m_verticalButtonSpawner;
        }
    }
    private Spawner m_horizontalButtonSpawner;
    public Spawner HorizontalButtonSpawner{
        get{
            if(m_horizontalButtonSpawner == null){
                m_horizontalButtonSpawner = HorizontalListBox.GetComponentInChildren<Spawner>();
            }
            return m_horizontalButtonSpawner;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       VerticalListBox.OnShiftMostVertical.AddListener(ShiftTest);
    }

    public void AddButtonsVertical(){
       for(int i = 0; i < 5; i++){
           var vb = VerticalListBox.AddListItem().GetComponent<Button>();
           vb.name = string.Format("Button {0}", VerticalListBox.listItemSpawner.SpawnedGameObjects.Count);
           vb.GetComponentInChildren<Text>().text = vb.name;
       }
    }

    public void AddButtonsHorizontal()
    {
        for (int i = 0; i < 5; i++)
        {
            var hb = HorizontalListBox.AddListItem().GetComponent<Button>();
            hb.name = string.Format("Button {0}", HorizontalListBox.listItemSpawner.SpawnedGameObjects.Count);
            hb.GetComponentInChildren<Text>().text = hb.name;
        }
    }

    void ShiftTest(RectTransform li, RectTransformHelpers.VerticalPosition horpos){
        Debug.Log(li.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
