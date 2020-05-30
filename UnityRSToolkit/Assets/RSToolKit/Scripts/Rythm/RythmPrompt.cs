using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Rythm
{
    [DisallowMultipleComponent]
    public class RythmPrompt : MonoBehaviour
    {
        
        public KeyCode promptKey;
        public RythmListBox ParentRythmListBox { get; private set; }
        public RectTransform RectTransformComponent { get; private set; }
        public RythmHitArea HitArea { get { return ParentRythmListBox.HitArea; } }

        void Awake()
        {
            ParentRythmListBox = this.GetComponentInParent<RythmListBox>();
            RectTransformComponent = GetComponent<RectTransform>();
        }

    }
}
