namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class FullscreenManager : MonoBehaviour
    {

        public KeyCode ToggleKey = KeyCode.F;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
           if (Input.GetKeyUp(ToggleKey)){
               ToggleFullscreen();
           }
        }
        public void ToggleFullscreen(){
            ToggleFullscreen(!Screen.fullScreen);
        }
        public void ToggleFullscreen(bool on){
            Debug.LogFormat("Toggle Fullscreen {0}", on ? "ON" : "OFF");
            Screen.fullScreen = on;
        }
    }
}