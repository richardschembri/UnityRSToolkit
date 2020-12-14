using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo.BehaviourTree.CTF{
    public class CTFCapturePoint : MonoBehaviour
    {
        
        public void OnTriggerEnter(Collider other)
            {
                if (other.gameObject.Equals(CTFGameManager.Instance.Flag)) {
                    // When the flag reaches the capture point the game is over
                    CTFGameManager.Instance.ResetGame();
                }
            }
    }

}