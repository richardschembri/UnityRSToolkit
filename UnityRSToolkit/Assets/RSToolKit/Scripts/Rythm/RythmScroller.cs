using RSToolkit.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.Rythm
{
    public class RythmScroller : MonoBehaviour
    {

        public float tempo = 5f;

        public float BeatsPerSecond
        {
            get
            {
                return (tempo / 60f); //* Time.deltaTime;
            }
        }

        public bool AutoStart = false;
        public bool HasStarted { get; private set; }
        
        ScrollRect m_scrollRectComponent;

        ScrollRect ScrollRectComponent
        {
            get
            {
                if (m_scrollRectComponent == null)
                {
                    m_scrollRectComponent = this.GetComponent<ScrollRect>();
                }
                return m_scrollRectComponent;
            }
        }

        private Spawner m_rythmPromptSpawner;
        public Spawner RythmPromptSpawner
        {
            get
            {
                if(m_rythmPromptSpawner == null)
                {
                    m_rythmPromptSpawner = GetComponentInChildren<Spawner>();
                }
                return m_rythmPromptSpawner;
            }
        }

        public void StartScrolling()
        {
            HasStarted = true;
        }

        public void StopScrolling()
        {
            HasStarted = false;
        }

        public void SpawnPrompts()
        {
            RythmPromptSpawner.DestroyAllSpawns();
            
            for (int i = 0; i < 10; i++)
            {
                RythmPromptSpawner.SpawnGameObject();
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (AutoStart)
            {
                StartScrolling();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (HasStarted)
            {
                ScrollRectComponent.content.anchoredPosition = new Vector2(0f, ScrollRectComponent.content.anchoredPosition.y +   BeatsPerSecond);
            }
        }
    }
}
