    using System.Collections;
    using System.Collections.Generic;
    using RSToolkit.Controls;
    using RSToolkit.Helpers;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

namespace RSToolkit.UI.Controls{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class UIDigits : Spawner<UIDigit>
    {

        [SerializeField]
        private uint _digits = 0;
        public uint Digits{
            get{
                return _digits;
            }set{
                if (_digits != value){
                    _digits = value;
                    SeperateDigits();
                }
            }
        }

        private void SeperateDigits(){
            var digitsArr = Digits.ToUIntArray();
            for(int i = 0; i < digitsArr.Length; i++){
                UIDigit uidigit;
                if (SpawnedGameObjects.Count > i){
                    uidigit = SpawnedGameObjects[i].GetComponent<UIDigit>();
                }else{
                    uidigit = SpawnAndGetGameObject().GetComponent<UIDigit>();
                }
                uidigit.Digit = digitsArr[i];
                uidigit.transform.SetAsLastSibling();
            }
            int extraIndex = digitsArr.Length;
            while ( SpawnedGameObjects.Count > digitsArr.Length ){
                DestroySpawnedGameObject(SpawnedGameObjects[extraIndex]);
                extraIndex++;
            }
            
        }

        private void OnInspectorGUI(){
            SeperateDigits();
        }

        #region MonoBehaviour Functions

        void OnEnable(){
            SeperateDigits();
        }

        #endregion MonoBehaviour Functions
    }
}