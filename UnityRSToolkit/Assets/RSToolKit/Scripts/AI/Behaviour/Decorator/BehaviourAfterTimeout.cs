using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour
{
    /// <summary>
    ///  Run the given Child Node. If the decoratee finishes sucessfully before the limit time is reached the decorator will wait until the limit is reached 
    ///  and then stop the execution with the result of the Child Node. If the Child Node stops with fail before the limit time is reached, 
    ///  the decorator will immediately stop.
    /// </summary>
    public class BehaviourAfterTimeout : BehaviourParentNode
    {
        private float m_limit = 0.0f;
        private float m_randomVariation;
        private bool m_waitOnFailure = false;
        private bool m_isLimitReached = false;
        NodeTimer m_timeoutTimer;

        private void Init(float limit, float? randomVariation = null, bool waitOnFailure = false)
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
                m_randomVariation = limit * 0.05f;
            }
            m_waitOnFailure = waitOnFailure;
        }
        #region Constructors

        /// <summary>
        ///  Run the given Child Node. If the decoratee finishes sucessfully before the limit time is reached the decorator will wait until the limit is reached 
        ///  and then stop the execution with the result of the Child Node. If the Child Node finishes failing before the limit time is reached, 
        ///  the decorator will immediately stop.
        /// </summary>
        /// <param name="limit">The timeout limit</param>
        public BehaviourAfterTimeout(float limit) : base("AfterTimeout", NodeType.DECORATOR)
        {
            Init(limit);
        }

        /// <summary>
        ///  Run the given Child Node. If the decoratee finishes sucessfully before the limit time is reached the decorator will wait until the limit is reached 
        ///  and then stop the execution with the result of the Child Node. If the Child Node finishes failing before the limit time is reached, 
        ///  the decorator will immediately stop.
        /// </summary>
        /// <param name="limit">The timeout limit</param>
        /// <param name="waitOnFailure">Wait for timeout before returning a result if child fails</param>
        public BehaviourAfterTimeout(float limit, bool waitOnFailure) : base("AfterTimeout", NodeType.DECORATOR)
        {
            Init(limit, null, waitOnFailure);
        }

        /// <summary>
        ///  Run the given Child Node. If the decoratee finishes sucessfully before the limit time is reached the decorator will wait until the limit is reached 
        ///  and then stop the execution with the result of the Child Node. If the Child Node finishes failing before the limit time is reached, 
        ///  the decorator will immediately stop.
        /// </summary>
        /// <param name="limit">The timeout limit</param>
        /// <param name="randomVariation">The random variance of the time limit</param>
        /// <param name="waitOnFailure">Wait for timeout before returning a result if child fails</param>
        public BehaviourAfterTimeout(float limit, float randomVariation, bool waitOnFailure) : base("AfterTimeout", NodeType.DECORATOR)
        {
            Init(limit, randomVariation, waitOnFailure);
        }

        #endregion Constructors
        
        #region Events

        private void OnStarted_Common()
        {
            RemoveTimer(m_timeoutTimer);
            m_timeoutTimer = AddTimer(m_limit, m_randomVariation, 0, OnTimeout);
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
                m_isLimitReached = true;
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

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(m_isLimitReached || (!success && !m_waitOnFailure))
            {
                RemoveTimer(m_timeoutTimer);
                OnStopped.Invoke(success);
            }
        }
        private void OnChildNodeStoppedSilent_Listener(BehaviourNode child, bool success)
        {
            if(m_isLimitReached || (!success && !m_waitOnFailure))
            {
                RemoveTimer(m_timeoutTimer);
            }
        }

        #endregion Events

        private void OnTimeout()
        {
            m_isLimitReached = true;
            if (Children[0].Result != null)
            {
                OnStopped.Invoke(Children[0].Result.Value);
            }
        }
    }
}
