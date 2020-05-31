using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Helpers;
using UnityEngine.UI;

namespace RSToolkit.Rythm
{
    [DisallowMultipleComponent]
    public class RythmPrompt : MonoBehaviour
    {
        
        private KeyCode m_promptKey;
        private RythmListBox m_parentRythmListBox;
        public RythmListBox ParentRythmListBox {
            get
            {
                if (m_parentRythmListBox == null)
                {
                    m_parentRythmListBox = GetComponentInParent<RythmListBox>();
                }
                return m_parentRythmListBox;
            }
        }
        public RectTransform RectTransformComponent { get; private set; }
        public RythmHitArea HitArea { get { return ParentRythmListBox.HitArea; } }
        public bool Prompted { get; private set; }
        public class OnRythmPromptedEvent : UnityEvent<RythmPrompt, float> { }
        public OnRythmPromptedEvent OnRythmPrompted { get; private set; } = new OnRythmPromptedEvent();
        float m_overlap;
        public Text m_textComponent { get; private set; }

        void Awake()
        {    
            RectTransformComponent = GetComponent<RectTransform>();
            m_textComponent = GetComponentInChildren<Text>();
            Prompted = false;
        }

        public void SetPrompt(KeyCode promptKey)
        {
            this.GetComponent<Button>().interactable = true;
            m_promptKey = promptKey;
            SetPromptText(m_promptKey.ToString());           
            Prompted = false;
        }

        public void SetPromptText(string text)
        {
            m_textComponent.text = text;
        }

        private void Update()
        {
            if (Input.GetKeyUp(m_promptKey) && !Prompted)
            {               
                m_overlap = HitArea.RectTransformComponent.OverlapPercent(RectTransformComponent);
                if(m_overlap > 0 && m_overlap <= 1f)
                {
                    this.GetComponent<Button>().interactable = false;
                    Prompted = true;
                    OnRythmPrompted.Invoke(this, m_overlap);
                }               
            }
        }

    }
}
