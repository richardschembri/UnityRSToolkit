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

        public BehaviourWait(float seconds, float randomVariance) : base("Wait", NodeType.TASK)
        {
            Init(seconds, randomVariance);
        }

        public BehaviourWait(float seconds) : base("Wait", NodeType.TASK)
        {
            Init(seconds, seconds * 0.05f);
        }

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

        private void OnTimeOut()
        {
            RemoveTimer(m_waitTimout);
            OnStopped.Invoke(true);
        }
    }
}