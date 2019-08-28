namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPreviewImage : MonoBehaviour
    {
        private Image m_imageComponent;
        public Image ImageComponent{
            get{
                if(m_imageComponent == null){
                    m_imageComponent = GetComponentInChildren<Image>();
                }
                return m_imageComponent;
            }
        }
        private RawImage m_rawImageComponent;
        public RawImage RawImageComponent{
            get{
                if(m_rawImageComponent == null){
                    m_rawImageComponent = GetComponentInChildren<RawImage>();
                }
                return m_rawImageComponent;
            }
        }
        private AspectRatioFitter m_aspectRatioFitterComponent; 
        private AspectRatioFitter m_AspectRatioFitterComponent{
            get{
                if(m_aspectRatioFitterComponent == null){
                    m_aspectRatioFitterComponent = GetComponentInChildren<AspectRatioFitter>();
                }
                return m_aspectRatioFitterComponent;
            }
        }

        public void SetImageSprite(Sprite sprite){
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.None;
            ImageComponent.sprite = null;
            ImageComponent.sprite = sprite;
            ImageComponent.SetNativeSize();
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

        }
        public void SetRawImageTexture2D(Texture2D texture){
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.None;
            RawImageComponent.texture = null;
            RawImageComponent.texture = texture;
            RawImageComponent.SetNativeSize();
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}