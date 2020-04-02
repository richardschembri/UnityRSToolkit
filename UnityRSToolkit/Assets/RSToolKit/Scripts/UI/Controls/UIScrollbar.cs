namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

       // Designwise only supports horizonal 
    public class UIScrollbar : MonoBehaviour
    {
        public UIButton ScrollDecButton;
        public UIButton ScrollIncButton;
         
        float m_value = 0f; // Is compensate for scrollbars with steps

        float ScrollSpeed = 0.05f;
        public Scrollbar ScrollBarComponent;
        // Start is called before the first frame update
        void Start()
        {
            m_value = ScrollBarComponent.value;
           if(ScrollDecButton != null){
            ScrollDecButton.onPressed.AddListener(ScrollDec);
           } 
           if(ScrollIncButton != null){
            ScrollIncButton.onPressed.AddListener(ScrollInc);
           } 
           ScrollBarComponent.onValueChanged.AddListener(onValueChanged);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void onValueChanged(float val){
            if(!ScrollDecButton.Pressed && !ScrollIncButton.Pressed){
                m_value = val;
            }
        }
        void ScrollDec(){
            m_value -= ScrollSpeed;
            m_value = (m_value < 0f) ? 0f : m_value;
            ScrollBarComponent.value = m_value;
        }
        void ScrollInc(){
            m_value += ScrollSpeed;
            m_value = (m_value > 1f) ? 1f : m_value;
            ScrollBarComponent.value = m_value;
        }
    }
}