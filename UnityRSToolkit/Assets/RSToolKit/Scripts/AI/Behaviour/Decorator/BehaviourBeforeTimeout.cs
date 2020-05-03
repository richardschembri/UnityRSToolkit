using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour
{
    public class BehaviourBeforeTimeout : BehaviourParentNode
    {
        private float m_limit = 0.0f;
        private float m_randomVariation;
        private bool m_waitForChildButFailOnLimitReached = false;
        private bool m_isLimitReached = false;
        NodeTimer m_timeoutTimer;

        private void Init(float limit, float? randomVariation, bool waitForChildButFailOnLimitReached)
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
                m_randomVariation = m_limit * 0.05f;
            }
            m_waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        }
        #region Constructors
        public BehaviourBeforeTimeout(float limit, bool waitForChildButFailOnLimitReached) : base("BeforeTimeout", NodeType.DECORATOR)
        {
            Init(limit, null, waitForChildButFailOnLimitReached);
        }
        public BehaviourBeforeTimeout(float limit, float randomVariation, bool waitForChildButFailOnLimitReached) : base("BeforeTimeout", NodeType.DECORATOR)
        {
            Init(limit, randomVariation, waitForChildButFailOnLimitReached);
        }
        #endregion Constructors
        private void OnTimeout()
        {
            if (!m_waitForChildButFailOnLimitReached)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                m_isLimitReached = true;
            }
        }
        private void OnStarted_Listener()
        {
            m_isLimitReached = false;
            AddTimer(m_limit, m_randomVariation, 0, OnTimeout);
            Children[0].StartNode();
        }
        private void OnStopping_Listener()
        {
            RemoveTimer(m_timeoutTimer);
            if(Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }
        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            RemoveTimer(m_timeoutTimer);
            if (m_isLimitReached)
            {
                OnStopped.Invoke(false);
            }
            else
            {
                OnStopped.Invoke(success);
            }
        }
    }
}
