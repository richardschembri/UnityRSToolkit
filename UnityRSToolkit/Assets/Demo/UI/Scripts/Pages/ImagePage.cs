using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.UI.Paging;
using RSToolkit.Tools;
using RSToolkit.UI.Controls;

public class ImagePage : UIPage
{
    public UIPreviewImage previewImage;
    public UIDisplayImage displayImage;
    public RectTransform scaleCompare;
    // Start is called before the first frame update
    async void Start()
    {
        var pi = await LoadImageTools.LoadSprite("Demo/UI/Images/sample/caferacer.jpg", true);
        
        previewImage.SetImageSprite(pi);
        displayImage.SetImageSprite(pi);
       //imageRect.ResizeToParentAndKeepAspect(); 
    }

    protected override void Awake(){
        base.Awake();
        Debug.Log("Awake");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
