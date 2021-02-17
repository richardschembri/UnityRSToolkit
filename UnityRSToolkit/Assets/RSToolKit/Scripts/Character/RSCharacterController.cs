using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Character
{
    public abstract class RSCharacterController : RSMonoBehaviour
    {
        public string DisplayName;
        public class RSCharacterEvent :UnityEvent<RSCharacterController> {}

        public override bool Init(bool force = false)
        {
            if(base.Init(force)){
                if(string.IsNullOrEmpty(DisplayName)){
                    DisplayName = gameObject.name;
                }
                return true;
            }
            return false;
        }
    }
}
