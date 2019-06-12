namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using RSToolkit.Helpers;
    using System.Linq;

    public class UIListBox : ScrollRect
    {
        public bool InitCullingByUser = true;

        bool m_CullingOn = false;
       private RectTransform[] m_contentChildren ;
       private RectTransform[] m_ContentChildren{
           get{
            if (m_contentChildren == null)
                return content.GetComponent<RectTransform>().GetTopLevelChildren<RectTransform>().ToArray();
            return null;
           }
       } 
        protected override void Start(){
            base.Start();
            onValueChanged.AddListener(m_onValueChanged);  
        }

        void Awake(){
            if(!InitCullingByUser){
                TurnOnCulling();
            }
        }

       void m_onValueChanged(Vector2 value){
            ViewportOcclusionCulling();
       }

       public void TurnOnCulling(){
          m_CullingOn = true; 
          ViewportOcclusionCulling();
       }

       public void TurnOffCulling(){
          m_CullingOn = false; 
          ViewportOcclusionCulling();
       }

       private VerticalLayoutGroup m_verticalLayoutGroupComponent = null;
       private VerticalLayoutGroup VerticalLayoutGroupComponent{
           get{
               if (m_verticalLayoutGroupComponent == null){
                   m_verticalLayoutGroupComponent = content.GetComponent<VerticalLayoutGroup>(); 
               }
               return m_verticalLayoutGroupComponent;
           }
       }
       
       private HorizontalLayoutGroup m_horizontalLayoutGroupComponent  = null;
       private HorizontalLayoutGroup HorizontalLayoutGroupComponent{
           get{
               if (m_horizontalLayoutGroupComponent == null){
                   m_horizontalLayoutGroupComponent = content.GetComponent<HorizontalLayoutGroup>(); 
               }
               return m_horizontalLayoutGroupComponent;
           }
       }

       void ViewportOcclusionCulling(){
            if (content == null){
                return;
            }
            content.GetComponent<ContentSizeFitter>().enabled = !m_CullingOn;

            if (VerticalLayoutGroupComponent != null){
                VerticalLayoutGroupComponent.enabled = !m_CullingOn;
            }

            if(HorizontalLayoutGroupComponent != null){
                HorizontalLayoutGroupComponent.enabled = !m_CullingOn;
            }

            for(int i = 0; i < m_ContentChildren.Length; i++){
                m_ContentChildren[i].gameObject.SetActive(m_CullingOn && viewport.HasWithinBounds(m_ContentChildren[i]));
            }
       }
        /// <summary>
        /// Adds a gameobject in 
        /// </summary>
        /// <param name=""></param>
        public GameObject GeneratedGameObject(GameObject go)
        {
            var new_go = Instantiate(go);
            new_go.transform.CopyScaleAndRotation(go.transform);
            new_go.transform.SetParent(content.transform);
            return new_go;
        }

    }
}