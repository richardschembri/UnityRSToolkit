using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Task
{
    // Blackboard
    public class BehaviourWait : BehaviourNode
    {

        public float WaitSeconds { get; private set; } = 0f;
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
            this.WaitSeconds = seconds;
            this.randomVariance = randomVariance;
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnStoppingSilent.AddListener(OnStoppingSilent_Listener);
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

        private void OnStarted_Common()
        {
            float seconds = WaitSeconds;

            if (seconds < 0)
            {
                seconds = 0;
            }
            if (randomVariance >= 0f)
            {
                m_waitTimout = AddTimer(seconds, randomVariance, 0, OnTimeOut);
            }
            else
            {
                m_waitTimout = AddTimer(seconds, 0, OnTimeOut);
            }
        }

        private void OnStarted_Listener()
        {
            OnStarted_Common();
        }

        private void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }

        private void OnStopping_Listener()
        {
            OnTimeOut();
        }

        private void OnStoppingSilent_Listener(){
            RemoveTimer(m_waitTimout);
        }

        #endregion Events

        private void OnTimeOut()
        {
            RemoveTimer(m_waitTimout);
            OnStopped.Invoke(true);
        }
    }
}