namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIDisplayImage : UIImage 
    {
        public override void SetImageSprite(Sprite sprite){
            base.SetImageSprite(sprite);
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

        }
        public override void SetRawImageTexture2D(Texture2D texture){
            base.SetRawImageTexture2D(texture);
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        }
    }
}