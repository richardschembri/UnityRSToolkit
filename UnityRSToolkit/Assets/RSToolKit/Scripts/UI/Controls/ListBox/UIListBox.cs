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
       private RectTransform[] m_contentChildren ;
       private RectTransform[] m_ContentChildren{
           get{
            if (m_contentChildren == null)
                return content.GetComponent<RectTransform>().GetTopLevelChildren().ToArray();
            return null;
           }
       } 
        protected override void Start(){
            base.Start();
            onValueChanged.AddListener(m_onValueChanged);  
        }

       void m_onValueChanged(Vector2 value){

           ViewportOcclusionCulling();
       }

       void ViewportOcclusionCulling(){
            if (content == null){
                return;
            }
            content.GetComponent<ContentSizeFitter>().enabled = false;
            content.GetComponent<VerticalLayoutGroup>().enabled = false;

            for(int i = 0; i < m_ContentChildren.Length; i++){
                m_ContentChildren[i].gameObject.SetActive(viewport.HasWithinBounds(m_ContentChildren[i]));
            }
       }
        /// <summary>
        /// Adds a gameobject in 
        /// </summary>
        /// <param name=""></param>
        public void GeneratedGameObject(GameObject go)
        {
            var new_go = Instantiate(go);
            new_go.transform.CopyScaleAndRotation(go.transform);
            new_go.transform.SetParent(content.transform);
        }

    }
}