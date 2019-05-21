namespace RSToolkit.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class CountDown : MonoBehaviour
    {
        public int CountFrom = 10;

        private bool m_isRunning = false;
        public bool IsRunning{
            get{
                return m_isRunning;
            }
            private set{
                m_isRunning = value;
            }
        }
        private int m_Count = 0;
        public int Count{
            get{ return m_Count; }
            private set { m_Count = value; }
        }
        public float SecondsInterval = 1;

        public UnityEvent OnStart = new UnityEvent();
        public UnityEvent OnTick = new UnityEvent();
        public UnityEvent OnReset = new UnityEvent();
        public UnityEvent OnComplete = new UnityEvent();

        void Start(){
            ResetCounter();
        }

        public void ResetCounter(){
            Count = CountFrom;
        }

        public void ResetCountdown(){
            StopCountdown();
            ResetCounter();
            OnReset.Invoke();
        }

        public void RestartCountdown(){
            ResetCounter();
            StopCountdown();
            StartCountdown();
        }
        public void StartCountdown(){
            if(IsRunning){
                return;
            }

            StopCountdown();
            IsRunning = true;
            OnStart.Invoke();
            InvokeRepeating("DecrementCounter", SecondsInterval, SecondsInterval);
        }
        public void StopCountdown(){
            CancelInvoke("DecrementCounter");
            IsRunning = false;
        }

        public bool IsComplete(){
            return Count <= 0;
        }

        void DecrementCounter(){
            Count--;
            OnTick.Invoke();
            if (IsComplete()){
                CancelInvoke("DecrementCounter");
                OnComplete.Invoke();
            }        
        }
        
    }

}