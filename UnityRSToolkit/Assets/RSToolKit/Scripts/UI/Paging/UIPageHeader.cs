namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPageHeader : MonoBehaviour
    {
        private Text m_headerText;
        private Text m_HeaderText{
            get{
                if(m_headerText == null){
                    m_headerText = this.gameObject.GetComponentInChildren<Text>();
                }
                return m_headerText;
            }
        }

        public void SetHeaderText(UIPage page)
        {
            if(m_HeaderText != null){
               m_HeaderText.text = page.GetHeader();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
