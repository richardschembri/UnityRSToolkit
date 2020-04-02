namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        bool m_pressed = false;
        public bool Pressed{
            get{
                return m_pressed;
            }
        }
        public UnityEvent onPressed = new UnityEvent();
        public void OnPointerDown(PointerEventData eventData)
        {
            m_pressed = true;
        }
     
        public void OnPointerUp(PointerEventData eventData)
        {
            m_pressed = false;
        }
     
        void Update()
        {
            if (!m_pressed)
                return;
            onPressed.Invoke(); 
            // DO SOMETHING HERE
        }
    }

}