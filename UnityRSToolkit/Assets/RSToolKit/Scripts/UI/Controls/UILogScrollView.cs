namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILogScrollView : ScrollRect
    {
        Text m_textComponent;
        Text TextComponent{
            get{
                if (m_textComponent == null){
                    m_textComponent = this.content.GetComponent<Text>();
                    // TODO: Add component if not there
                }
                return m_textComponent;
            }
        }

        public void AppendLog(StringBuilder sb){
            AppendLog(sb.ToString());
        }

        public void AppendLog(string text){
            m_textComponent.text = string.Format("{0} /n {1}", m_textComponent.text, text);
        }

        public void ClearLog(){
            m_textComponent.text = string.Empty;
        }
    }

}