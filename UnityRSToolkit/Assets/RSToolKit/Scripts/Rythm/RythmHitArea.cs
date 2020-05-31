using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Rythm
{

    [DisallowMultipleComponent]
    public class RythmHitArea : MonoBehaviour
    {

        public RectTransform RectTransformComponent { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            RectTransformComponent = GetComponent<RectTransform>();
        }


    }
}
