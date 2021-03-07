namespace RSToolkit.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class CountDown : RSMonoBehaviour
    {
        public int CountFrom = 10;

        public bool IsRunning { get; private set; }

        public int Count { get; private set; }

        public float SecondsInterval = 1;

        public UnityEvent OnStart = new UnityEvent();
        public UnityEvent OnTick = new UnityEvent();
        public UnityEvent OnReset = new UnityEvent();
        public UnityEvent OnComplete = new UnityEvent();

        #region MonoBehaviour Functions
        protected virtual void Start(){
            ResetCounter();
        }
        #endregion MonoBehaviour Functions

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