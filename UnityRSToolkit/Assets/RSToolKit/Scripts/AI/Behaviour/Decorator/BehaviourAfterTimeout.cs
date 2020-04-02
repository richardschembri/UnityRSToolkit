using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour
{
    public class BehaviourAfterTimeout : BehaviourNode
    {
        private float m_limit = 0.0f;
        private float m_randomVariation;
        private bool m_waitOnFailure = false;
        private bool m_isLimitReached = false;
        NodeTimer m_timeoutTimer;

        private void Init(float limit, float? randomVariation = null, bool waitOnFailure = false)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);

            m_limit = limit;
            if(randomVariation != null)
            {
                m_randomVariation = randomVariation.Value;
            }
            else
            {
                m_randomVariation = limit * 0.05f;
            }
            m_waitOnFailure = waitOnFailure;
        }
        #region Constructors
        public BehaviourAfterTimeout(float limit) : base("AfterTimeout", NodeType.DECORATOR)
        {
            Init(limit);
        }

        public BehaviourAfterTimeout(float limit, bool waitOnFailure) : base("AfterTimeout", NodeType.DECORATOR)
        {
            Init(limit, null, waitOnFailure);
        }

        public BehaviourAfterTimeout(float limit, float randomVariation, bool waitOnFailure) : base("AfterTimeout", NodeType.DECORATOR)
        {
            Init(limit, randomVariation, waitOnFailure);
        }
        #endregion Constructors
        private void OnTimeout()
        {
            m_isLimitReached = true;
            if (Children[0].Result != null)
            {
                OnStopped.Invoke(Children[0].Result.Value);
            }
        }
        private void OnStarted_Listener()
        {
            AddTimer(m_limit, m_randomVariation, 0, OnTimeout);
            Children[0].StartNode();
        }
        private void OnStopping_Listener()
        {
            RemoveTimer(m_timeoutTimer);
            if(Children[0].State == NodeState.ACTIVE)
            {
                m_isLimitReached = true;
                Children[0].RequestStopNode();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }
        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(m_isLimitReached || (!success && !m_waitOnFailure))
            {
                RemoveTimer(m_timeoutTimer);
                OnStopped.Invoke(success);
            }
        }
    }
}
