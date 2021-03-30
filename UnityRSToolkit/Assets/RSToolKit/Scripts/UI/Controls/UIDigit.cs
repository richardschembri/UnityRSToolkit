using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Tools;
using UnityEngine.Serialization;

namespace RSToolkit.UI.Controls{
    [RequireComponent(typeof(Image))]
    public class UIDigit : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] _digitSprites = null;

        [FormerlySerializedAs("Digit")]
        [SerializeField]
        [Range(0, 9)]
        private uint _digit = 0;

        public uint Digit{
            get{
                return _digit;
            }set{
                if (_digit != value){
                    _digit = value;
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

        private Image _imageComponent;
        public Image ImageComponent{
            get{
                if (_imageComponent == null){
                    _imageComponent = this.GetComponent<Image>();
                }
                return _imageComponent;
            }
        }

        #region MonoBehaviour Functions
        private void OnGUI(){
            RefreshImage();
        }
        #endregion MonoBehaviour Functions

        private void OnInspectorGUI(){
            RefreshImage();
        }
    }
}