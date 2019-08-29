namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    public class RawImageByScreenDimensions : AdjustByScreenDimensions{

        public RawImageForScreenDimensionsSettings[] settings; 
        public bool SetNativeSize = false;

        public override void Adjust(){
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();

            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    this.GetComponent<RawImage>().texture = sd.rawImageTexture;
                    m_adjusted = true;
                    if(SetNativeSize){
                        this.GetComponent<RawImage>().SetNativeSize();
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct RawImageForScreenDimensionsSettings {

        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;

        public Texture2D rawImageTexture;
    }
}