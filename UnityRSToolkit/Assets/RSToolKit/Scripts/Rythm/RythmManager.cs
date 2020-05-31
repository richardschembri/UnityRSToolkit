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

        [SerializeField]
        private bool AutoSpawn = true;
        [SerializeField]
        private bool AutoStart = true;

        // Start is called before the first frame update
        void Awake()
        {
            HitAreaComponent = GetComponentInChildren<RythmHitArea>();
            RythmScrollers = GetComponentsInChildren<RythmListBox>();
            if (AutoSpawn)
            {
                SpawnPrompts();
            }
            if (AutoStart)
            {
                StartRythm();
            }
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
            RythmPrompt[] rythmPrompts;
            for (int i = 0; i < RythmScrollers.Length; i++)
            {
                rythmPrompts = RythmScrollers[i].SpawnPrompts(clearItems);    
                
                for(int j = 0; j < rythmPrompts.Length; j++)
                {
                    rythmPrompts[j].OnRythmPrompted.AddListener(OnRythmPrompted_Listener);
                }
            }
        }

        private void OnRythmPrompted_Listener(RythmPrompt prompt, float overlap)
        {
            Debug.Log($"Overlap: {prompt.name} [{overlap}]");
        }

    }
}