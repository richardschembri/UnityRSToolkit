namespace RSToolkit.UI.Controls{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using RSToolkit.Helpers;

    [RequireComponent(typeof(Image))]
    public class UIDigit : MonoBehaviour
    {
        private LoadImageTools lit;
        [SerializeField]
        private Sprite[] m_digitSprites;

        [Range(0, 9)]
        public int digit =0;

        private Image m_imageComponent;
        public Image ImageComponent{
            get{
                if (m_imageComponent == null){
                    m_imageComponent = this.GetComponent<Image>();
                }
                return m_imageComponent;
            }
        }

    // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnGUI(){
            if (m_digitSprites.Length > digit && m_digitSprites[digit] != null && m_digitSprites[digit] != ImageComponent.sprite){
                ImageComponent.sprite = m_digitSprites[digit];
                ImageComponent.preserveAspect = true;
            }
        }
    }
}