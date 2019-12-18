namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class AdjustByScreenDimensions : MonoBehaviour
    {
        public bool AdjustOnStart = true;
        protected bool m_adjusted = false;

        public enum ResolutionAspectType{
            iPhonePremium,
            iPhoneBudget,
            iPad,
            Other
        }
#region MonoBehaviour Funcitons
        protected virtual void Start () {
            if(AdjustOnStart){
                Adjust();		
            }
        }

        protected virtual void Awake(){
            if(AdjustOnStart){
                Adjust();		
            }
        }

        protected virtual void Update () {
            
        }
#endregion MonoBehaviour Funcitons

        public List<ScreenDimensions> GetPresetScreenDimensions(ResolutionAspectType rat, ScreenDimensions other){
            var result = new List<ScreenDimensions>();

            switch(rat){
                case ResolutionAspectType.iPhonePremium:
                result.Add(new ScreenDimensions(false, 90, 185));
                result.Add(new ScreenDimensions(true, 1125, 2436));
                result.Add(new ScreenDimensions(true, 1242, 2688));
                break;
                case ResolutionAspectType.iPhoneBudget:
                result.Add(new ScreenDimensions(true, 828, 1792));
                break;
                case ResolutionAspectType.iPad:
                result.Add(new ScreenDimensions(false, 3, 4));
                result.Add(new ScreenDimensions(false, 512, 683));
                result.Add(new ScreenDimensions(true, 1668, 2388));
                break;
                case ResolutionAspectType.Other:
                result.Add(other);
                break;
            }

            return result;
        }
        public bool IsDimensions(ScreenDimensions sd){
            return IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height);
        }

        public bool IsDimensions(bool resolutionOrAspectRatio, float width, float height){
            if(resolutionOrAspectRatio){
                return CheckResolution(width, height);
            }else{
                return CheckAspectRatio(width, height);
            }
        }

        public bool CheckResolution(float reswidth, float resheight){
            return (reswidth == Screen.width && resheight == Screen.height); 
        }

        public bool CheckAspectRatio(float ratiowidth, float ratioheight){
            return (((float)Screen.width / (float)Screen.height) == (ratiowidth / ratioheight));
        }

        public virtual void Adjust(){
            m_adjusted = true;
        }

        public bool IsAdjusted{
            get{
                return m_adjusted;
            }
        }

    }

    [System.Serializable]
    public struct ScreenDimensions {
        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;
        public ScreenDimensions(bool resolutionOrAspectRatio, float width, float height){
            ResolutionOrAspectRatio = resolutionOrAspectRatio;
            Width = width;
            Height = height;
        }
    }
}