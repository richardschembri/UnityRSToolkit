using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour
{
    /// <summary>
    /// Run the given decoratee. If the decoratee doesn't finish within the limit, the execution fails. 
    /// </summary>
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
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnStoppingSilent.AddListener(OnStoppingSilent_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnChildNodeStoppedSilent.AddListener(OnChildNodeStoppedSilent_Listener);

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

        /// <summary>
        /// If waitForChildButFailOnLimitReached is true, it will wait for the decoratee to finish but still fail.
        /// </summary>
        /// <param name="limit">The timeout limit</param>
        /// <param name="waitForChildButFailOnLimitReached">If is true, it will wait for the decoratee to finish but still fail.</param>
        public BehaviourBeforeTimeout(float limit, bool waitForChildButFailOnLimitReached) : base("BeforeTimeout", NodeType.DECORATOR)
        {
            Init(limit, null, waitForChildButFailOnLimitReached);
        }


        /// <summary>
        /// If waitForChildButFailOnLimitReached is true, it will wait for the decoratee to finish but still fail.
        /// </summary>
        /// <param name="limit">The timeout limit</param>
        /// <param name="randomVariation">The random variance to the timeout limit</param>
        /// <param name="waitForChildButFailOnLimitReached">If is true, it will wait for the decoratee to finish but still fail.</param>
        public BehaviourBeforeTimeout(float limit, float randomVariation, bool waitForChildButFailOnLimitReached) : base("BeforeTimeout", NodeType.DECORATOR)
        {
            Init(limit, randomVariation, waitForChildButFailOnLimitReached);
        }

        #endregion Constructors

        #region Events

        private void OnStarted_Common()
        {
            m_isLimitReached = false;
            AddTimer(m_limit, m_randomVariation, 0, OnTimeout);
        }

        private void OnStarted_Listener()
        {
            OnStarted_Common();
            Children[0].StartNode();
        }

        private void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }

        private void OnStopping_Common()
        {
            RemoveTimer(m_timeoutTimer);
        }
        private void OnStopping_Listener()
        {
            OnStopping_Common();
            if(Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }

        private void OnStoppingSilent_Listener(){
            OnStopping_Common();
        }

        private void OnChildNodeStopped_Common(BehaviourNode child, bool success)
        {
            RemoveTimer(m_timeoutTimer);
        }
        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            OnChildNodeStopped_Common(child, success);

            if (m_isLimitReached)
            {
                OnStopped.Invoke(false);
            }
            else
            {
                OnStopped.Invoke(success);
            }
        }

        private void OnChildNodeStoppedSilent_Listener(BehaviourNode child, bool success)
        {
            OnChildNodeStopped_Common(child, success);
        }

        #endregion Events

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

    }
}
