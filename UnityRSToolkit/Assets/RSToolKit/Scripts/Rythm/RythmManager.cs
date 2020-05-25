using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Rythm
{
    public class RythmManager : MonoBehaviour
    {

        public RectTransform HitArea;
        public RythmListBox[] RythmScrollers;

        // Start is called before the first frame update
        void Awake()
        {
            for(int i = 0; i < RythmScrollers.Length; i++)
            {
                RythmScrollers[i].SpawnPrompts();
                RythmScrollers[i].StartScrolling();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}