using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RSToolkit.Controls
{
    public class InputTimeout : MonoBehaviour
    {
        public float TimeoutSeconds = 60f;

        private float m_elapsedTime = 0f;
        public UnityEvent OnTimeout = new UnityEvent();
        public bool DebugMode = false;


        void Awake()
        {
            ResetTimer();
        }

        void OnEnable()
        {
            ResetTimer();
        }

        private void ResetTimer()
        {
            m_elapsedTime = 0;
        }

        // Update is called once per frame
        void Update()
        {        
            if(Input.anyKey)
            {
                ResetTimer();
            }

            m_elapsedTime += Time.deltaTime;

            if (m_elapsedTime > TimeoutSeconds)
            {
                if (DebugMode)
                {
                    Debug.Log("Input Timeout");
                }
                OnTimeout.Invoke();
                ResetTimer();
            }
        }

    }
}
