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
        // Start is called before the first frame update
        void Start()
        {
            OnStart.AddListener(updateText);
            OnTick.AddListener(updateText);
            OnReset.AddListener(updateText);
            ResetCountdown();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void Awake(){
        }

        void updateText(){
            this.GetComponent<Text>().text = Count.ToString();
        }
    }
}