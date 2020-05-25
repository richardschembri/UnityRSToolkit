using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RSToolkit.UI.Paging;
using RSToolkit.UI.Controls;
public class UIListBoxPage : UIPage
{
    public UIEditableListBox uiEditableListBox;
    public Toggle ToggleView;
    public Toggle ToggleSelect;
    public Toggle ToggleEdit;
    public Toggle ToggleDelete;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void Awake(){
        base.Awake();
    }

    protected override void onNavigatedTo(UIPage page, bool keepCache){
        ToggleView.isOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onToggleValueChanged(bool value){
        CheckToggles();
    }

    public void ButtonGenerate(){
        for(int i = 0; i < 5; i++){
           var vb = uiEditableListBox.AddEditableListItem();
           vb.name = string.Format("List Item {0}", uiEditableListBox.ListItemSpawner.SpawnedGameObjects.Count);
           vb.SetModeTextComponent(UIEditableListBoxItem.ListBoxItemMode.VIEW, "Text", string.Format( "View {0}", uiEditableListBox.ListItemSpawner.SpawnedGameObjects.Count));
           vb.SetModeTextComponent(UIEditableListBoxItem.ListBoxItemMode.SELECT, "Text", string.Format( "Select {0}", uiEditableListBox.ListItemSpawner.SpawnedGameObjects.Count));
           vb.SetModeTextComponent(UIEditableListBoxItem.ListBoxItemMode.EDIT, "Text", string.Format( "Edit {0}", uiEditableListBox.ListItemSpawner.SpawnedGameObjects.Count));
           vb.SetModeTextComponent(UIEditableListBoxItem.ListBoxItemMode.DELETE, "Text", string.Format( "Delete {0}", uiEditableListBox.ListItemSpawner.SpawnedGameObjects.Count));
        }
    }

    public void ButtonSort(){
        uiEditableListBox.orderAscending = !uiEditableListBox.orderAscending;
    }

    private void CheckToggles(){
        if(ToggleView.isOn){
            uiEditableListBox.SetMode(UIEditableListBoxItem.ListBoxItemMode.VIEW);
        }
        if(ToggleSelect.isOn){
            uiEditableListBox.SetMode(UIEditableListBoxItem.ListBoxItemMode.SELECT);
        }
        if(ToggleEdit.isOn){
            uiEditableListBox.SetMode(UIEditableListBoxItem.ListBoxItemMode.EDIT);
        }
        if(ToggleDelete.isOn){
            uiEditableListBox.SetMode(UIEditableListBoxItem.ListBoxItemMode.DELETE);
        }
    }
}
