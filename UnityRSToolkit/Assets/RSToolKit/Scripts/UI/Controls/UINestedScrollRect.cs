// Taken from https://forum.unity.com/threads/nested-scrollrect.268551/
namespace RSToolkit.UI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UINestedScrollRect : ScrollRect {
 
        private bool m_dragParents = false;
    
        /// <summary>
        /// Do action for all parents
        /// </summary>
        private void DoForParents<T>(Action<T> action) where T:IEventSystemHandler
        {
            Transform parent = transform.parent;
            while(parent != null) {
                foreach(var component in parent.GetComponents<Component>()) {
                    if(component is T)
                        action((T)(IEventSystemHandler)component);
                }
                parent = parent.parent;
            }
        }
    
        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public override void OnInitializePotentialDrag (PointerEventData eventData)
        {
            DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
            base.OnInitializePotentialDrag (eventData);
        }
    
        /// <summary>
        /// Drag event
        /// </summary>
        public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(m_dragParents)
                DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
            else
                base.OnDrag (eventData);
        }
    
        /// <summary>
        /// Begin drag event
        /// </summary>
        public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(!horizontal && Math.Abs (eventData.delta.x) > Math.Abs (eventData.delta.y))
                m_dragParents = true;
            else if(!vertical && Math.Abs (eventData.delta.x) < Math.Abs (eventData.delta.y))
                m_dragParents = true;
            else
                m_dragParents = false;
    
            if(m_dragParents)
                DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
            else
                base.OnBeginDrag (eventData);
        }
    
        /// <summary>
        /// End drag event
        /// </summary>
        public override void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(m_dragParents)
                DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
            else
                base.OnEndDrag (eventData);
            m_dragParents = false;
        }
    }
}