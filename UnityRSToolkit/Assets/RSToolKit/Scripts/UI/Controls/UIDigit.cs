namespace RSToolkit.UI.Controls{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using RSToolkit.Helpers;
    using UnityEngine.Serialization;

    [RequireComponent(typeof(Image))]
    public class UIDigit : MonoBehaviour
    {
        private LoadImageTools lit;
        [SerializeField]
        private Sprite[] _digitSprites = null;

        [FormerlySerializedAs("Digit")]
        [SerializeField]
        [Range(0, 9)]
        private uint m_digit = 0;

        public uint Digit{
            get{
                return m_digit;
            }set{
                if (m_digit != value){
                    m_digit = value;
                    RefreshImage();
                }
            }
        }

        public void RefreshImage(){
            if (_digitSprites.Length > Digit && _digitSprites[Digit] != null && _digitSprites[Digit] != ImageComponent.sprite){
                ImageComponent.sprite = null; // Set to null because of Unity Aspect bug
                ImageComponent.sprite = _digitSprites[Digit];
                ImageComponent.preserveAspect = true;
            }
        }

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
            RefreshImage();
        }

        private void OnInspectorGUI(){
            RefreshImage();
        }
    }
}