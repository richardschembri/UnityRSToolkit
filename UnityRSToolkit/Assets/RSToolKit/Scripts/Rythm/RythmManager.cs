using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RSToolkit.Controls;

namespace RSToolkit.Rythm
{
    public class RythmManager : MonoBehaviour
    {
        public RythmHitArea HitAreaComponent { get; private set; }

        public RythmListBox[] RythmScrollers { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            RythmScrollers = GetComponentsInChildren<RythmListBox>();
            HitAreaComponent = GetComponentInChildren<RythmHitArea>();
        }

        public bool HasStarted()
        {
            return RythmScrollers.Any(rs => rs.HasStarted);
        }

        public bool HasPrompts()
        {
            return RythmScrollers.Any(rs => rs.HasPrompts());
        }

        public void StartRythm()
        {
            for (int i = 0; i < RythmScrollers.Length; i++)
            {
                RythmScrollers[i].StartScrolling();
            }
        }

        public void StopRythm()
        {
            for (int i = 0; i < RythmScrollers.Length; i++)
            {
                RythmScrollers[i].StopScrolling();
            }
        }

        public void SpawnPrompts(bool clearItems = true)
        {
            for (int i = 0; i < RythmScrollers.Length; i++)
            {
                RythmScrollers[i].SpawnPrompts(clearItems);             
            }
        }

    }
}