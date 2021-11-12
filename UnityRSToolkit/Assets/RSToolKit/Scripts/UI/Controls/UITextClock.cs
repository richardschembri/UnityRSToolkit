using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

namespace RSToolkit.UI.Controls
{
    [RequireComponent(typeof(Text))]
    public class UITextClock : MonoBehaviour
    {
        Text _textComponent;

        [SerializeField]
        bool is24Hour = false;

        [SerializeField]
        int _ampmFontSize = -1;
        void Awake()
        {
            _textComponent = GetComponent<Text>();          
        }

        void Update()
        {

            if (is24Hour)
            {
                _textComponent.text = DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture);
            }
            else
            {
                if(_ampmFontSize <= 0)
                {
                    _textComponent.text = DateTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture);
                }
                else
                {
                    _textComponent.text = DateTime.Now.ToString("hh:mm", CultureInfo.InvariantCulture) + $" <size={_ampmFontSize}>" + DateTime.Now.ToString("tt", CultureInfo.InvariantCulture) + "</size>";
                }
            }
        }
    }
}
