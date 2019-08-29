namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class PositionByScreenDimensions : AdjustByScreenDimensions
    {
        public PositionByScreenDimensionsSettings[] settings; 

        public override void Adjust(){
            var ordPosByScreenDimensions = settings.OrderBy(psd => psd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordPosByScreenDimensions.Count(); i++){
                var psd = ordPosByScreenDimensions[i];

                if(IsDimensions(psd.ResolutionOrAspectRatio, psd.Width, psd.Height)){
                    this.GetComponent<RectTransform>().anchoredPosition = new Vector2(psd.PosX, psd.PosY);
                    m_adjusted = true;
                }
            }
        }
    }

    [System.Serializable]
    public struct PositionByScreenDimensionsSettings {
        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;
        public float PosX;
        public float PosY;
    }
}