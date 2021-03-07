namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using RSToolkit.Controls;
    using UnityEngine.Events;

    [RequireComponent(typeof(Text))]
    public class UICountDownText : CountDown
    {

        private Text _textComponent;

        // Start is called before the first frame update
        protected sealed override void Start()
        {
            base.Start();
            OnStart.AddListener(updateText);
            OnTick.AddListener(updateText);
            OnReset.AddListener(updateText);
            ResetCountdown();
        }

        protected sealed override void Awake()
        {
            base.Awake();
            _textComponent = this.GetComponent<Text>();
        }

        void updateText(){
            _textComponent.text = Count.ToString();
        }
    }
}