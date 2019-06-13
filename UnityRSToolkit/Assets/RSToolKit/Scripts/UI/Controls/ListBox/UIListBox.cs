namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using RSToolkit.Helpers;
    using System.Linq;

    [RequireComponent(typeof(ScrollRect))]
    public class UIListBox : MonoBehaviour
    {
        [SerializeField]
        public bool InitCullingByUser = true;

        private bool m_initCullingComplete = false;
        bool m_init = false;

        bool m_CullingOn = false;

        private ScrollRect m_scrollRectComponent = null;
        private ScrollRect m_ScrollRectComponent{
            get{
                if (m_scrollRectComponent == null)
                    m_scrollRectComponent = this.GetComponent<ScrollRect>();
                return m_scrollRectComponent;
            }
        }
       private RectTransform[] m_contentChildren ;
       private RectTransform[] m_ContentChildren{
           get{
            if (m_contentChildren == null)
                return m_ScrollRectComponent.content.GetComponent<RectTransform>().GetTopLevelChildren<RectTransform>().ToArray();
            return null;
           }
       } 
        int countdown = 20; // For some reason coroutine is not working. Need to refactor.
        void Awake(){
            if(!m_init){
                m_init = true;
                m_ScrollRectComponent.onValueChanged.AddListener(m_onValueChanged);
            }
        }
        void Update(){
            if (countdown > 0){
                countdown--;
            }else if(countdown == 0){
                countdown--;
                if(!m_initCullingComplete && !InitCullingByUser){
                    m_initCullingComplete = true;
                    TurnOnCulling();
                }
            }
        }

       void m_onValueChanged(Vector2 value){
            ViewportOcclusionCulling();
       }

       public void TurnOnCulling(){
           if(!m_CullingOn ){
            m_CullingOn = true; 
            ViewportOcclusionCulling();
           }
       }

       public void TurnOffCulling(){
           if(m_CullingOn ){
            m_CullingOn = false; 
            ViewportOcclusionCulling();
            for(int i = 0; i < m_ContentChildren.Length; i++){
                m_ContentChildren[i].gameObject.SetActive(true);
            }
           }
       }

       private VerticalLayoutGroup m_verticalLayoutGroupComponent = null;
       private VerticalLayoutGroup VerticalLayoutGroupComponent{
           get{
               if (m_verticalLayoutGroupComponent == null){
                   m_verticalLayoutGroupComponent = m_ScrollRectComponent.content.GetComponent<VerticalLayoutGroup>(); 
               }
               return m_verticalLayoutGroupComponent;
           }
       }
       
       private HorizontalLayoutGroup m_horizontalLayoutGroupComponent  = null;
       private HorizontalLayoutGroup HorizontalLayoutGroupComponent{
           get{
               if (m_horizontalLayoutGroupComponent == null){
                   m_horizontalLayoutGroupComponent = m_ScrollRectComponent.content.GetComponent<HorizontalLayoutGroup>(); 
               }
               return m_horizontalLayoutGroupComponent;
           }
       }

       void ViewportOcclusionCulling(){
            if (m_ScrollRectComponent.content == null){
                return;
            }

            if (VerticalLayoutGroupComponent != null){
                VerticalLayoutGroupComponent.enabled = !m_CullingOn;
            }

            if(HorizontalLayoutGroupComponent != null){
                HorizontalLayoutGroupComponent.enabled = !m_CullingOn;
            }

            m_ScrollRectComponent.content.GetComponent<ContentSizeFitter>().enabled = !m_CullingOn;
            if(m_CullingOn){
                for(int i = 0; i < m_ContentChildren.Length; i++){
                    m_ContentChildren[i].gameObject.SetActive(m_ScrollRectComponent.viewport.HasWithinBounds(m_ContentChildren[i]));
                }
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
            new_go.transform.SetParent(m_ScrollRectComponent.content.transform);
            return new_go;
        }

    }
}