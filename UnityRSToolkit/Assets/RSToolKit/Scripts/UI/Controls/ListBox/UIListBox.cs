namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using RSToolkit.Helpers;
    using RSToolkit.Controls;
    using System.Linq;

    [RequireComponent(typeof(ScrollRect))]
    public class UIListBox : MonoBehaviour
    {
        private enum ManualScroll{
            NONE, LEFT, RIGHT, UP, DOWN
        }
        ManualScroll m_manualScroll = ManualScroll.NONE;

        [SerializeField]
        public bool InitCullingByUser = true;

        private bool m_initCullingComplete = false;
        bool m_init = false;

        bool m_CullingOn = false;

        public float ManualScrollSpeed = 0.01f;

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

       public Spawner ListItemSpawner;
       
       public GameObject AddListItem(){
            if(ListItemSpawner == null){
                return null;
            }

            return ListItemSpawner.SpawnAndGetGameObject();
       }

       public void Refresh(){
           m_contentChildren = null;
           m_scrollRectComponent = null;
           TurnOffCulling();
           TurnOnCulling();
           m_ScrollRectComponent.velocity = new Vector2(0f, 0f);
           //m_ScrollRectComponent.verticalNormalizedPosition = 0f;
           m_ScrollRectComponent.content.anchoredPosition = new Vector2(m_ScrollRectComponent.content.anchoredPosition.x, 0);
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
            if (m_manualScroll == ManualScroll.NONE){
                InfiniteVerticalScroll(m_ScrollRectComponent.velocity.y < 0);
                InfiniteHorizontalScroll(m_ScrollRectComponent.velocity.x > 0);
            }else{
                switch(m_manualScroll){
                    case ManualScroll.DOWN:
                    case ManualScroll.UP:
                    InfiniteVerticalScroll(m_manualScroll == ManualScroll.DOWN);
                    break;
                    case ManualScroll.LEFT:
                    case ManualScroll.RIGHT:
                    InfiniteHorizontalScroll(m_manualScroll == ManualScroll.RIGHT);
                    break;
                }
            }
            m_manualScroll = ManualScroll.NONE;
       }

       public void TurnOnCulling(){
           StartCoroutine(DelayedTurnOnCulling());
       }

       IEnumerator DelayedTurnOnCulling(){
           yield return new WaitForEndOfFrame();
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

       public Vector2 ListItemsSize(){
           var x = m_ContentChildren.Sum(r => r.ScaledSize().x);
           var y = m_ContentChildren.Sum(r => r.ScaledSize().y);
           return new Vector2(x, y);
       }

        public void InfiniteVerticalScroll(bool isScrollDown){
            if (!m_ScrollRectComponent.vertical || m_ScrollRectComponent.movementType != ScrollRect.MovementType.Unrestricted || ListItemsSize().y < m_ScrollRectComponent.viewport.ScaledSize().y){
                return;
            } 
            var vertList = m_ContentChildren.OrderByDescending(rt => rt.GetComponent<RectTransform>().position.y).ToArray();
            var topLI = vertList[0];
            var bottomLI = vertList[vertList.Length - 1];
            float padding = -5f;
            if(!isScrollDown){
                padding = 5f;
            }
            var topPos = m_ScrollRectComponent.viewport.PositionWithinBounds(topLI, new Vector2(0, padding), Vector2.zero);
            var bottomPos = m_ScrollRectComponent.viewport.PositionWithinBounds(bottomLI, new Vector2(0, padding), Vector2.zero); 
            if(isScrollDown){
                if(topPos.verticalPostion == RectTransformHelpers.VerticalPosition.WITHIN
                    && bottomPos.verticalPostion == RectTransformHelpers.VerticalPosition.BELOW){
                    bottomLI.position = topLI.ShiftUpPosition();
                    bottomLI.SetAsFirstSibling();
                }
            }else{
                if(bottomPos.verticalPostion == RectTransformHelpers.VerticalPosition.WITHIN
                    && topPos.verticalPostion == RectTransformHelpers.VerticalPosition.ABOVE){
                    topLI.position = bottomLI.ShiftDownPosition();
                    topLI.SetAsLastSibling();
                }
            }
        }

        public void InfiniteHorizontalScroll(bool isScrollRight){
            if (!m_ScrollRectComponent.horizontal || m_ScrollRectComponent.movementType != ScrollRect.MovementType.Unrestricted || ListItemsSize().x < m_ScrollRectComponent.viewport.ScaledSize().x){
                return;
            } 
            var horizList = m_ContentChildren.OrderByDescending(rt => rt.GetComponent<RectTransform>().position.x).ToArray();
            var rightLI = horizList[0];
            var leftLI = horizList[horizList.Length - 1];
            float padding = -5f;
            if(!isScrollRight){
                padding = 5f;
            }
            var leftPos = m_ScrollRectComponent.viewport.PositionWithinBounds(leftLI, new Vector2(padding, 0), Vector2.zero);
            var rightPos = m_ScrollRectComponent.viewport.PositionWithinBounds(rightLI, new Vector2(padding, 0), Vector2.zero); 
            if(isScrollRight){
                if(leftPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.WITHIN
                    && rightPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.RIGHT){
                    rightLI.position = leftLI.ShiftLeftPosition();
                    rightLI.SetAsFirstSibling();
                }
            }else{
                if(rightPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.WITHIN
                    && leftPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.LEFT){
                    leftLI.position = rightLI.ShiftRightPosition();
                    leftLI.SetAsLastSibling();
                }
            }
        }

        public void ScrollLeft(){
            m_manualScroll = ManualScroll.LEFT; 
            m_ScrollRectComponent.horizontalNormalizedPosition += ManualScrollSpeed;
        }

        public void ScrollRight(){
            m_manualScroll = ManualScroll.RIGHT; 
            m_ScrollRectComponent.horizontalNormalizedPosition -= ManualScrollSpeed;
        }

        public void ScrollDown(){
            m_manualScroll = ManualScroll.DOWN; 
            m_ScrollRectComponent.verticalNormalizedPosition += ManualScrollSpeed;
        }

        public void ScrollUp(){
            m_manualScroll = ManualScroll.UP; 
            m_ScrollRectComponent.verticalNormalizedPosition -= ManualScrollSpeed;
        }

    }
}