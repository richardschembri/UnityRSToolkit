using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Character
{
    public abstract class RSCharacterController : RSMonoBehaviour
    {
        public class RSCharacterEvent :UnityEvent<RSCharacterController> {}
    }
}
