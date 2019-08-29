namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    public class ImageByScreenDimensions : AdjustByScreenDimensions
    {
        public ImageByScreenDimensionsSettings[] settings;
        public override void Adjust(){
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    this.GetComponent<Image>().sprite = sd.imageSprite;
                    this.GetComponent<Image>().preserveAspect = sd.preserveAspect;
                }
            }
        }
    }

    [System.Serializable]
    public struct ImageByScreenDimensionsSettings {

        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;

        public Sprite imageSprite;

        public bool preserveAspect;
    }
}