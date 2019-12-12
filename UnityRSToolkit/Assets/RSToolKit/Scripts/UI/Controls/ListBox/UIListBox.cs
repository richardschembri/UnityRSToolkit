namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine.Events;
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

        private bool m_initDelayedCullingComplete = false;
        bool m_init = false;

        bool m_CullingOn = false;

        public float manualScrollSpeed { get { return m_ManualScrollSpeed; } set { m_ManualScrollSpeed = value; }}

        [SerializeField]
        private float m_ManualScrollSpeed = 0.01f;

        public bool isVertical  { get { return m_IsVertical; } set { m_IsVertical = value; }}
        [SerializeField]
        private bool m_IsVertical = true;

        public bool occlusionCulling { get { return m_OcclusionCulling; } set { m_OcclusionCulling = value; CheckCulling(); }}
        [SerializeField]
        private bool m_OcclusionCulling = true;
        private ScrollRect m_scrollRectComponent = null;
        public ScrollRect ScrollRectComponent{
            get{
                if (m_scrollRectComponent == null)
                    m_scrollRectComponent = this.GetComponent<ScrollRect>();
                return m_scrollRectComponent;
            }
        }
       // private RectTransform m_contentRectTransform;
       public RectTransform ContentRectTransform{
           get{
               return ScrollRectComponent.content.GetComponent<RectTransform>();
           }
       }

       protected RectTransform[] m_contentChildren ;
       protected virtual RectTransform[] m_ContentChildren{
           get{
            if (m_contentChildren == null)
                m_contentChildren = ContentRectTransform.GetTopLevelChildren<RectTransform>().ToArray();
            return m_contentChildren;
           }
       } 


       private Spawner m_ListItemSpawner;
       public Spawner listItemSpawner { 
           get { 
               if(m_ListItemSpawner == null){
                   m_ListItemSpawner = ContentRectTransform.GetComponent<Spawner>();
               }
               return m_ListItemSpawner; 
            } 
       }

       public class OnShiftMostHorizontalEvent : UnityEvent<RectTransform, RectTransformHelpers.HorizontalPosition>{}
       public OnShiftMostHorizontalEvent OnShiftMostHorizontal = new OnShiftMostHorizontalEvent();
       
       public class OnShiftMostVerticalEvent : UnityEvent<RectTransform, RectTransformHelpers.VerticalPosition>{}
       public OnShiftMostVerticalEvent OnShiftMostVertical = new OnShiftMostVerticalEvent();
       public virtual GameObject AddListItem(){
            if(listItemSpawner == null){
                return null;
            }
            var result = listItemSpawner.SpawnAndGetGameObject();
            Refresh();
            return result;
       }

       public void ClearSpawnedListItems(){
            listItemSpawner.DestroyAllSpawns();
            Refresh();
       }

       public int VisibleSpawnedListItemCount(){
           return listItemSpawner.SpawnedGameObjects.Where(li => li.gameObject.activeSelf).Count();
       }

       public void Refresh(){
           m_contentChildren = null;
           m_scrollRectComponent = null;
           SetLayout();
           ScrollRectComponent.velocity = new Vector2(0f, 0f);
           ScrollRectComponent.content.anchoredPosition = new Vector2(0f, 0f);
           CheckCulling();
       }
        int m_culling_countdown = 5;
        protected void Start(){
            ScrollRectComponent.vertical = isVertical;
            ScrollRectComponent.horizontal = !isVertical;

            if(ScrollRectComponent.movementType == ScrollRect.MovementType.Unrestricted){
                bool removeScrollbar = false;
                if (ScrollRectComponent.vertical && ScrollRectComponent.verticalScrollbar != null)
                {
                    var vs = ScrollRectComponent.verticalScrollbar;
                    ScrollRectComponent.verticalScrollbar = null;
                    Destroy(vs.gameObject);
                    // vs.gameObject.SetActive(false);
                    removeScrollbar = true;
                }

                if (ScrollRectComponent.horizontal && ScrollRectComponent.horizontalScrollbar != null)
                {
                    var hs = ScrollRectComponent.horizontalScrollbar;
                    ScrollRectComponent.horizontalScrollbar = null;
                    Destroy(hs.gameObject);
                    // hs.gameObject.SetActive(false);
                    removeScrollbar = true;
                }
                if(removeScrollbar){
                    ScrollRectComponent.viewport.SetAnchor(RectTransformHelpers.AnchorPresets.StretchAll);
                }
            }

        }
        protected void Awake(){
            if(!m_init){
                m_init = true;


                ScrollRectComponent.onValueChanged.AddListener(m_onValueChanged);
                SetLayout();
            }
        }
        void Update(){
            if (m_culling_countdown > 0){
                m_culling_countdown--;
            }else if(m_culling_countdown == 0){
                m_culling_countdown--;
                if(!m_initDelayedCullingComplete){
                    m_initDelayedCullingComplete = true;
                    TurnOnCulling();
                }
            }
        }

       void m_onValueChanged(Vector2 value){
            ViewportOcclusionCulling();
            if (m_manualScroll == ManualScroll.NONE){
                InfiniteVerticalScroll(ScrollRectComponent.velocity.y < 0);
                InfiniteHorizontalScroll(ScrollRectComponent.velocity.x > 0);
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
           if(!m_CullingOn ){
            m_CullingOn = true; 
           }
           ViewportOcclusionCulling();
       }

       void DelayedTurnOnCulling(){
            m_initDelayedCullingComplete = false;
            m_culling_countdown = 5;
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

       private void CheckCulling(){
           if(occlusionCulling){
               TurnOnCulling();
           }else{
               TurnOffCulling();
           }
       }

       void ViewportOcclusionCulling(){
            if (ScrollRectComponent.content == null){
                return;
            }
            if(m_CullingOn){
                for(int i = 0; i < m_ContentChildren.Length; i++){
                    m_ContentChildren[i].gameObject.SetActive(ScrollRectComponent.viewport.HasWithinBounds(m_ContentChildren[i]));
                }
            }
       }

       public Vector2 ListItemsSize(){
           var x = m_ContentChildren.Sum(r => r.ScaledSize().x);
           var y = m_ContentChildren.Sum(r => r.ScaledSize().y);
           return new Vector2(x, y);
       }

       private void ShiftListItemAbove(RectTransform toshift){
            ShiftListItemAbove(toshift, toshift);
       }
       private void ShiftListItemAbove(RectTransform toshift, RectTransform target ){
            toshift.localPosition = target.ShiftUpLocalPosition(spacing);
       }
       private void ShiftListItemBelow(RectTransform toshift){
            ShiftListItemBelow(toshift, toshift);
       }
       private void ShiftListItemBelow(RectTransform toshift, RectTransform target){
            toshift.localPosition = target.ShiftDownLocalPosition(spacing);
       }
       private void ShiftListItemLeft(RectTransform toshift){
            ShiftListItemLeft(toshift, toshift);
       }
       private void ShiftListItemLeft(RectTransform toshift, RectTransform target){
            toshift.localPosition = target.ShiftLeftLocalPosition(spacing);
       }
       private void PlaceListItemLeftMost(RectTransform toplace, float padding){
            var toplacePos = GetPositionWithinViewPort(toplace, padding); 
            while(toplacePos.horizontalPostion != RectTransformHelpers.HorizontalPosition.LEFT){
                ShiftListItemLeft(toplace);                    
                toplacePos = ScrollRectComponent.viewport.PositionWithinBounds(toplace, new Vector2(padding, 0), Vector2.zero); 
            }
            toplace.SetAsFirstSibling();
            OnShiftMostHorizontal.Invoke(toplace, RectTransformHelpers.HorizontalPosition.LEFT); 
       }

       private void ShiftListItemRight(RectTransform toshift){
            ShiftListItemRight(toshift, toshift);
       }
       private void ShiftListItemRight(RectTransform toshift, RectTransform target){
            toshift.localPosition = target.ShiftRightLocalPosition(spacing);
       }
       private void ShiftListItemRightMost(RectTransform toplace, float padding){
            var toplacePos = GetPositionWithinViewPort(toplace, padding); 
            while(toplacePos.horizontalPostion != RectTransformHelpers.HorizontalPosition.RIGHT){
                ShiftListItemRight(toplace);
                toplacePos = ScrollRectComponent.viewport.PositionWithinBounds(toplace, new Vector2(padding, 0), Vector2.zero);
            }
            toplace.SetAsLastSibling();
            OnShiftMostHorizontal.Invoke(toplace, RectTransformHelpers.HorizontalPosition.RIGHT); 
       }

       private void ShiftListItemTopMost(RectTransform toplace, float padding){
            var toplacePos = GetPositionWithinViewPort(toplace, padding); 
            while(toplacePos.verticalPostion != RectTransformHelpers.VerticalPosition.ABOVE){
                ShiftListItemAbove(toplace);
                toplacePos = ScrollRectComponent.viewport.PositionWithinBounds(toplace, new Vector2(0, padding), Vector2.zero); 
            }
            toplace.SetAsFirstSibling();
            OnShiftMostVertical.Invoke(toplace, RectTransformHelpers.VerticalPosition.ABOVE); 
       }

       private void ShiftListItemBottomMost(RectTransform toplace, float padding){
            var toplacePos = GetPositionWithinViewPort(toplace, padding); 
            while(toplacePos.verticalPostion != RectTransformHelpers.VerticalPosition.BELOW){
                ShiftListItemBelow(toplace);
                toplacePos = ScrollRectComponent.viewport.PositionWithinBounds(toplace, new Vector2(0, padding), Vector2.zero);
            }
            toplace.SetAsLastSibling();
            OnShiftMostVertical.Invoke(toplace, RectTransformHelpers.VerticalPosition.BELOW); 
       }

       private RectTransformHelpers.RectTransformPosition GetPositionWithinViewPort(RectTransform toplace, float padding){
            return ScrollRectComponent.viewport.PositionWithinBounds(toplace, new Vector2(0, padding), Vector2.zero); 
       }

        public void InfiniteVerticalScroll(bool isScrollDown){
            if (!ScrollRectComponent.vertical || ScrollRectComponent.movementType != ScrollRect.MovementType.Unrestricted){
                return;
            } 
            var vertList = m_ContentChildren.OrderByDescending(rt => rt.GetComponent<RectTransform>().position.y).ToArray();
            if(!vertList.Any()){
                return;
            }
            var topLI = vertList[0];
            var bottomLI = vertList[vertList.Length - 1];
            float padding = -5f;
            if(!isScrollDown){
                padding = 5f;
            }
            var topPos = ScrollRectComponent.viewport.PositionWithinBounds(topLI, new Vector2(0, padding), Vector2.zero);
            var bottomPos = ScrollRectComponent.viewport.PositionWithinBounds(bottomLI, new Vector2(0, padding), Vector2.zero); 
            if(isScrollDown){
                if((vertList.Length < 2 && bottomPos.verticalPostion == RectTransformHelpers.VerticalPosition.BELOW) || (topPos.verticalPostion == RectTransformHelpers.VerticalPosition.WITHIN
                    && bottomPos.verticalPostion == RectTransformHelpers.VerticalPosition.BELOW)){
                    ShiftListItemTopMost(bottomLI, padding);
                }
            }else{
                if((vertList.Length < 2 && topPos.verticalPostion == RectTransformHelpers.VerticalPosition.ABOVE) || (bottomPos.verticalPostion == RectTransformHelpers.VerticalPosition.WITHIN
                    && topPos.verticalPostion == RectTransformHelpers.VerticalPosition.ABOVE)){
                    ShiftListItemBottomMost(topLI, padding);
                }

            }
        }

        public void InfiniteHorizontalScroll(bool isScrollRight){
            if (!ScrollRectComponent.horizontal || ScrollRectComponent.movementType != ScrollRect.MovementType.Unrestricted){
                return;
            } 
            var horizList = m_ContentChildren.OrderByDescending(rt => rt.GetComponent<RectTransform>().position.x).ToArray();
            if(!horizList.Any()){
                return;
            }
            var rightLI = horizList[0];
            var leftLI = horizList[horizList.Length - 1];
            float padding = -5f;
            if(!isScrollRight){
                padding = 5f;
            }
            var leftPos = ScrollRectComponent.viewport.PositionWithinBounds(leftLI, new Vector2(padding, 0), Vector2.zero);
            var rightPos = ScrollRectComponent.viewport.PositionWithinBounds(rightLI, new Vector2(padding, 0), Vector2.zero); 
            if(isScrollRight){
                if((horizList.Length < 2 && rightPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.RIGHT) || (leftPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.WITHIN
                    && rightPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.RIGHT)){
                    PlaceListItemLeftMost(rightLI, padding);
                }
            }else{
                if((horizList.Length < 2 && leftPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.LEFT) || (rightPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.WITHIN
                    && leftPos.horizontalPostion == RectTransformHelpers.HorizontalPosition.LEFT)){
                    ShiftListItemRightMost(leftLI, padding);
                }
            }
        }

        public void ScrollLeft(){
            m_manualScroll = ManualScroll.LEFT; 
            ScrollRectComponent.horizontalNormalizedPosition += m_ManualScrollSpeed;
        }

        public void ScrollRight(){
            m_manualScroll = ManualScroll.RIGHT; 
            ScrollRectComponent.horizontalNormalizedPosition -= m_ManualScrollSpeed;
        }

        public void ScrollDown(){
            m_manualScroll = ManualScroll.DOWN; 
            ScrollRectComponent.verticalNormalizedPosition += m_ManualScrollSpeed;
        }

        public void ScrollUp(){
            m_manualScroll = ManualScroll.UP; 
            ScrollRectComponent.verticalNormalizedPosition -= m_ManualScrollSpeed;
        }

        #region Content Size Fitter
        protected DrivenRectTransformTracker m_ContentSizeTracker; // m_Tracker

        private void HandleSelfFittingAlongAxis(int axis){
           m_ContentSizeTracker.Add(this, ContentRectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY)); 
           ContentRectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(ContentRectTransform, axis));
        }

        public virtual void SetLayout(){
            SetLayoutHorizontal();
            SetLayoutVertical();
        }

        public virtual void SetLayoutHorizontal(){
            m_ContentSizeTracker.Clear();

            SetChildrenAlongAxis(0, isVertical);
        }

        public virtual void SetLayoutVertical(){
            SetChildrenAlongAxis(1, isVertical);
        }

        protected void SetDirty(){
            if(!gameObject.activeSelf)
                return;
            
            if(!CanvasUpdateRegistry.IsRebuildingLayout())
                LayoutRebuilder.MarkLayoutForRebuild(ContentRectTransform);
            else
                StartCoroutine(DelayedSetDirty(ContentRectTransform));
                
        }

        IEnumerator DelayedSetDirty(RectTransform rectTransform){
            yield return null;
            LayoutRebuilder.MarkLayoutForRebuild(ContentRectTransform);
        }
        #endregion

        #region LayoutGroup code
        [SerializeField] protected TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;
        public TextAnchor childAlignment { get { return m_ChildAlignment; } set { SetProperty(ref m_ChildAlignment, value); }}
        [SerializeField] protected RectOffset m_Padding = new RectOffset();
        protected DrivenRectTransformTracker m_LayoutGroupTracker; // m_Tracker

        public RectOffset padding { get { return m_Padding; } set { SetProperty(ref m_Padding, value); }}

        private Vector2 m_TotalMinSize = Vector2.zero;
        private Vector2 m_TotalPreferredSize = Vector2.zero;

        [SerializeField] protected float m_Spacing = 0;
        public float spacing {get {return m_Spacing;} set { SetProperty(ref m_Spacing, value); }}

        /// <summary>
        /// Returns the alignment on the specified axis as a fraction where 0 is left/top, 0.5 is middle, and 1 is right/bottom.
        /// </summary>
        /// <param name="axis">The axis to get alignment along. 0 is horizontal and 1 is vertical.</param>
        /// <returns>The alignment as a fraction where 0 is left/top, 0.5 is middle, and 1 is right/bottom.</returns>
        protected float GetAlignmentOnAxis(int axis){
            if(axis == 0)
                return((int)childAlignment % 3) * 0.5f;
            return((int)childAlignment / 3) * 0.5f;
        }

        protected void SetProperty<T>(ref T currentValue, T newValue){
            if((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            // SetDirty
        }

        protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding){
            float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);
            float availableSpace = ContentRectTransform.rect.size[axis];
            float surplusSpace = availableSpace - requiredSpace;
            float alignmentOnAxis = GetAlignmentOnAxis(axis);
            return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
        }

        /// <summary>
        /// Set the position and size of a child layout element along the given axis.
        /// </summary>
        /// <param name="rect">The RectTransform of the child layout element.</param>
        /// <param name="axis">The axis to set the position and size along. 0 is horizontal and 1 is vertical.</param>
        /// <param name="pos">The position from the left side or top.</param>
        /// <param name="size">The size.</param>
        protected void SetChildrenAlongAxisWithScale(RectTransform rect, int axis, float pos, float size, float scaleFactor){
            if (rect == null)
                return;

            m_LayoutGroupTracker.Add(this, rect,
                DrivenTransformProperties.Anchors |
                (axis == 0 ?
                    (DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX) :
                    (DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY)
                )
            );

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            Vector2 sizeDelta = rect.sizeDelta;
            sizeDelta[axis] = size;
            rect.sizeDelta = sizeDelta;

            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition[axis] = (axis == 0) ? (pos + size * rect.pivot[axis] * scaleFactor) : (-pos - size * (1f - rect.pivot[axis] * scaleFactor));
            rect.anchoredPosition = anchoredPosition;
        }

        /// <summary>
        /// Set the position and size of a child layout element along the given axis.
        /// </summary>
        /// <param name="rect">The RectTransform of the child layout element.</param>
        /// <param name="axis">The axis to set the position and size along. 0 is horizontal and 1 is vertical.</param>
        /// <param name="pos">The position from the left side or top.</param>
        /// <param name="size">The size.</param>
        protected void SetChildrenAlongAxisWithScale(RectTransform rect, int axis, float pos, float scaleFactor){
            if(rect == null)
                return;
            
            m_LayoutGroupTracker.Add(this, rect,
                DrivenTransformProperties.Anchors | 
                (axis == 0 ? DrivenTransformProperties.AnchoredPositionX : DrivenTransformProperties.AnchoredPositionY));

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition[axis] = (axis == 0) ? (pos + rect.sizeDelta[axis] * rect.pivot[axis] * scaleFactor) : (-pos - rect.sizeDelta[axis] * (1f - rect.pivot[axis]) * scaleFactor);
            rect.anchoredPosition = anchoredPosition;
            rect.SetAsLastSibling();
        }

        private void SetChildrenAlongAxis(int axis, bool isVertical){
            float size = ContentRectTransform.rect.size[axis];
            bool controlSize = (axis == 0 ? isVertical : !isVertical);
            //bool useScale = false;
            float alignmentOnAxis = GetAlignmentOnAxis(axis);
            bool alongOtherAxis = (isVertical ^ (axis == 1));
            if(alongOtherAxis){
                float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
                for (int i = 0; i < m_ContentChildren.Length; i++){
                    RectTransform child = m_ContentChildren[i];
                    float scaleFactor = child.localScale[axis];
                    float requiredSpace = Mathf.Clamp(innerSize, 0, size);
                    float startOffset = GetStartOffset(axis, requiredSpace * scaleFactor);
                    if(controlSize){
                        SetChildrenAlongAxisWithScale(child, axis, startOffset, requiredSpace, scaleFactor);
                    } else {
                        float offsetInCell = (requiredSpace - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildrenAlongAxisWithScale(child, axis, startOffset + offsetInCell, scaleFactor);
                    }
                }

            } else {
                float pos = (axis == 0 ? padding.left : padding.top);
                for (int i = 0; i < m_ContentChildren.Length; i++){
                    RectTransform child = m_ContentChildren[i];
                    float scaleFactor = child.localScale[axis];

                    float childSize = axis == 0 ? child.rect.width : child.rect.height;
                    if(controlSize){
                        SetChildrenAlongAxisWithScale(child, axis, pos, childSize, scaleFactor);
                    } else {
                        float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildrenAlongAxisWithScale(child, axis, pos + offsetInCell, scaleFactor);
                    }
                    pos += childSize * scaleFactor + spacing;
                }
                var sizeDelta = ContentRectTransform.sizeDelta;
                sizeDelta[axis] = pos + (isVertical ? padding.bottom : padding.right);
                ContentRectTransform.sizeDelta = sizeDelta;
            }
        }


        #endregion

    }
}