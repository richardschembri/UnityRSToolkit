using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.UI.Paging;
using RSToolkit.Helpers;
using RSToolkit.UI.Controls;

public class ImagePage : UIPage
{
    public UIPreviewImage previewImage;
    // Start is called before the first frame update
    void Start()
    {
        var pi = LoadImageTools.LoadSprite("Images/sample/caferacer.jpg", true);
        previewImage.SetImageSprite(pi);
       //imageRect.ResizeToParentAndKeepAspect(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
