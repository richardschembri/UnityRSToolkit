namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class AdjustByScreenDimensions : MonoBehaviour
    {
        public bool AdjustOnStart = true;
        protected bool m_adjusted = false;
        // Use this for initialization
        void Start () {
            if(AdjustOnStart){
                Adjust();		
            }
        }

        void Awake(){
            if(AdjustOnStart){
                Adjust();		
            }
        }
        
        // Update is called once per frame
        void Update () {
            
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
        public string Name;
        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;
    }
}