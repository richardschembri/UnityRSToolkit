namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class SpriteByScreenDimensions : AdjustByScreenDimensions {
        public Sprite defaultsprite;
        private Sprite m_sprite;
        public SpriteByScreenDimensionsSettings[] settings; 

        // Use this for initialization
        public override void Adjust(){
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();

            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    m_sprite = sd.sprite;
                    m_adjusted = true;
                }
            }
        }

        public Sprite GetSprite(){

            if(!m_adjusted){
                Adjust();
            }
            if(m_sprite == null){
                m_sprite = defaultsprite;
            }
            return m_sprite;
        }
    }

    [System.Serializable]
    public struct SpriteByScreenDimensionsSettings {

        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;

        public Sprite sprite;
    }
}