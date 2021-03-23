using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    [RequireComponent(typeof(Button))]
    public class UILinkButton : RSMonoBehaviour
    {
        public string LinkURL = "";
        Button _buttonComponent;

        public override bool Init(bool force = false)
        {
            if (!base.Init(force))
            {
                return false;
            }
            _buttonComponent = GetComponent<Button>();
            _buttonComponent.onClick.AddListener(OnLinkButtonClick_Listener);
            return true;
        }

        private void OnLinkButtonClick_Listener()
        {
            LogInDebugMode(LinkURL);
            Application.OpenURL(LinkURL);
        }
    }
}
