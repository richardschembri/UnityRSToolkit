namespace RSToolkit.UI.Controls{
    using System.Collections;
    using System.Collections.Generic;
    using RSToolkit.Controls;
    using RSToolkit.Helpers;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [RequireComponent(typeof(Spawner))]
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class UIDigits : MonoBehaviour
    {
        private Spawner m_spawnerComponent;
        private Spawner SpawnerComponent{
            get{
                if (m_spawnerComponent == null){
                    m_spawnerComponent = this.GetComponent<Spawner>();
                }
                return m_spawnerComponent;
            }
        }

        [FormerlySerializedAs("Digits")]
        [SerializeField]
        private uint m_digits = 0;
        public uint Digits{
            get{
                return m_digits;
            }set{
                if (m_digits != value){
                    m_digits = value;
                    SeperateDigits();
                }
            }
        }
        private List<UIDigit> m_UIDigits = new List<UIDigit>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnEnable(){
            SeperateDigits();
        }

        private void SeperateDigits(){
            var digitsArr = Digits.ToUIntArray();
            for(int i = 0; i < digitsArr.Length; i++){
                UIDigit uidigit;
                if (SpawnerComponent.SpawnedGameObjects.Count > i){
                    uidigit = SpawnerComponent.SpawnedGameObjects[i].GetComponent<UIDigit>();
                }else{
                    uidigit = SpawnerComponent.SpawnAndGetGameObject().GetComponent<UIDigit>();
                }
                uidigit.Digit = digitsArr[i];
                uidigit.transform.SetAsLastSibling();
            }
            int extraIndex = digitsArr.Length;
            while ( SpawnerComponent.SpawnedGameObjects.Count > digitsArr.Length ){
                SpawnerComponent.DestroySpawnedGameObject(SpawnerComponent.SpawnedGameObjects[extraIndex]);
                extraIndex++;
            }
            
        }

        private void OnInspectorGUI(){
            SeperateDigits();
        }
    }
}