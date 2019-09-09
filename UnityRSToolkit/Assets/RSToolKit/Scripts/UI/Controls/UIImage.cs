namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class UIImage : MonoBehaviour
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

        private RectTransform m_rectTransformComponent;
        public RectTransform RectTransformComponent{
            get{
                if(m_rectTransformComponent == null){
                    m_rectTransformComponent = GetComponent<RectTransform>();
                }
                return m_rectTransformComponent;
            }
        }
        private AspectRatioFitter m_aspectRatioFitterComponent; 
        protected AspectRatioFitter m_AspectRatioFitterComponent{
            get{
                if(m_aspectRatioFitterComponent == null){
                    m_aspectRatioFitterComponent = GetComponentInChildren<AspectRatioFitter>();
                }
                return m_aspectRatioFitterComponent;
            }
        }

        public void CalculateAspectRatio(){
            m_AspectRatioFitterComponent.aspectRatio = ImageComponent.rectTransform.sizeDelta.x / ImageComponent.rectTransform.sizeDelta.y;
        }

        public virtual void SetImageSprite(Sprite sprite){
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.None;
            ImageComponent.sprite = null;
            if(sprite != null){
                ImageComponent.sprite = sprite;
                ImageComponent.preserveAspect = true;
                ImageComponent.SetNativeSize();
                CalculateAspectRatio();
            }else{
                m_AspectRatioFitterComponent.aspectRatio = 1;
            }
        }

        public virtual void SetRawImageTexture2D(Texture2D texture){
            m_AspectRatioFitterComponent.aspectMode = AspectRatioFitter.AspectMode.None;
            RawImageComponent.texture = null;
            if(texture == null){
                RawImageComponent.texture = texture;
                RawImageComponent.SetNativeSize();
                CalculateAspectRatio();
            }else{
                m_AspectRatioFitterComponent.aspectRatio = 1;
            }
        }
    }
}