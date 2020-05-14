using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    /// <summary>
    /// Delay execution of the child node until the condition is true.
    /// </summary>
    public class BehaviourWaitForCondition : BehaviourParentNode
    {
        private Func<bool> m_isConditionMetFunc;
        private float m_checkInterval;
        private float m_checkVariance;
        private NodeTimer m_conditionTimer;

        /// <summary>
        /// Delay execution of the child node until the condition is true
        /// </summary>
        /// <param name="isConditionMetFunc">The function used to check the condition</param>
        /// <param name="checkInterval">The interval at which the condition is checked</param>
        /// <param name="randomVariance">The interval variance</param>
        public BehaviourWaitForCondition(Func<bool> isConditionMetFunc, float checkInterval, float randomVariance) : base("WaitForCondition", NodeType.DECORATOR)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopped.AddListener(OnStopped_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);

            m_isConditionMetFunc = isConditionMetFunc ;

            m_checkInterval = checkInterval;
            m_checkVariance = randomVariance;

            
            // this.Label = "" + (checkInterval - randomVariance) + "..." + (checkInterval + randomVariance) + "s";
        }

        private void checkCondition()
        {
            if (m_isConditionMetFunc.Invoke())
            {
                RemoveTimer(m_conditionTimer);
                Children[0].StartNode();
            }
        }

        #region Events

        private void OnStarted_Listener()
        {
            if (!m_isConditionMetFunc.Invoke())
            {
                AddTimer(m_checkInterval, m_checkVariance, -1, checkCondition);
            }
            else
            {
                Children[0].StartNode();
            }
        }
        private void OnStopped_Listener(bool success)
        {
            RemoveTimer(m_conditionTimer);
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
            OnStopped.Invoke(success);
        }

        #endregion Events
    }
}
