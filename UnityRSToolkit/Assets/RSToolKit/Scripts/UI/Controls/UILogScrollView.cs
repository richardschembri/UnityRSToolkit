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

        public void AppendLog(string text){
            var sbLog = new StringBuilder();
            sbLog.AppendLine(TextComponent.text);
            sbLog.AppendLine(text);
            TextComponent.text = sbLog.ToString();
        }

        public void ClearLog(){
            TextComponent.text = string.Empty;
        }
    }

}