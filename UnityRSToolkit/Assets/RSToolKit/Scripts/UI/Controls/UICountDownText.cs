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
            OnStart.AddListener(onTick);
            OnTick.AddListener(onTick);
            OnReset.AddListener(onTick);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void onStart(){

        }
        void onTick(){
            this.GetComponent<Text>().text = Count.ToString();
        }
    }
}