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
    public UIListBox<Button> VerticalListBox;
    public UIListBox<Button> HorizontalListBox;

    private Spawner<Button> _verticalButtonSpawner;
    public Spawner<Button> VerticalButtonSpawner{
        get{
            if(_verticalButtonSpawner == null){
                _verticalButtonSpawner = VerticalListBox.GetComponentInChildren<Spawner<Button>>();
            }
            return _verticalButtonSpawner;
        }
    }

    private Spawner<Button> _horizontalButtonSpawner;
    public Spawner<Button> HorizontalButtonSpawner{
        get{
            if(_horizontalButtonSpawner == null){
                _horizontalButtonSpawner = HorizontalListBox.GetComponentInChildren<Spawner<Button>>();
            }
            return _horizontalButtonSpawner;
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
           vb.name = string.Format("Button {0}", VerticalListBox.SpawnedGameObjects.Count);
           vb.GetComponentInChildren<Text>().text = vb.name;
       }
    }

    public void AddButtonsHorizontal()
    {
        for (int i = 0; i < 5; i++)
        {
            var hb = HorizontalListBox.AddListItem().GetComponent<Button>();
            hb.name = string.Format("Button {0}", HorizontalListBox.SpawnedGameObjects.Count);
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
