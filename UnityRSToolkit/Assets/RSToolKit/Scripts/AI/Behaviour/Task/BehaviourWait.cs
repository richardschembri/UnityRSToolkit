using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Task
{
    // Blackboard
    public class BehaviourWait : BehaviourNode
    {
        private System.Func<float> function = null;
        private float m_seconds = -1f;
        private float randomVariance;
        private NodeTimer m_waitTimout;

        public float RandomVariance
        {
            get
            {
                return randomVariance;
            }
            set
            {
                randomVariance = value;
            }
        }

        public void Init(float seconds, float randomVariance)
        {
            this.m_seconds = seconds;
            this.randomVariance = randomVariance;
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
        }
        #region Constructors

        /// <summary>
        /// Wait for given seconds with a random variance of 0.05 * seconds.
        /// </summary>
        /// <param name="seconds"></param>
        public BehaviourWait(float seconds) : base("Wait", NodeType.TASK)
        {
            Init(seconds, seconds * 0.05f);
        }

        /// <summary>
        /// Wait for given seconds with given random varaince.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="randomVariance"></param>
        public BehaviourWait(float seconds, float randomVariance) : base("Wait", NodeType.TASK)
        {
            Init(seconds, randomVariance);
        }

        #endregion Constructors

        #region Events

        private void OnStarted_Listener()
        {
            float seconds = m_seconds;
            if(seconds < 0)
            {
                if(this.function != null)
                {
                    seconds = this.function();
                }
            }
            if(seconds < 0)
            {
                seconds = 0;
            }
            if(randomVariance >= 0f)
            {
                m_waitTimout = AddTimer(seconds, randomVariance, 0, OnTimeOut);
            }
            else
            {
                m_waitTimout = AddTimer(seconds, 0, OnTimeOut);
            }
        }


        private void OnStopping_Listener()
        {
            OnTimeOut();
        }

        #endregion Events

        private void OnTimeOut()
        {
            RemoveTimer(m_waitTimout);
            OnStopped.Invoke(true);
        }
    }
}