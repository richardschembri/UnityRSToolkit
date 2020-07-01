using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourRepeatUntil : BehaviourParentNode
    {
        private const string NODE_NAME = "Repeat Until";
        private Func<bool> m_isConditionMetFunc;
        NodeTimer m_restartChildTimer;

        public BehaviourRepeatUntil(Func<bool> isConditionMetFunc) : base(NODE_NAME, NodeType.DECORATOR)
        {
            m_isConditionMetFunc = isConditionMetFunc;
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnStoppingSilent.AddListener(OnStoppingSilent_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnChildNodeStoppedSilent.AddListener(OnChildNodeStoppedSilent_Listener);
        }

        #region Events
        private void OnStarted_Common()
        {
            RemoveTimer(m_restartChildTimer);
        }
        private void OnStarted_Listener()
        {
            OnStarted_Common();
            m_restartChildTimer = StartFirstChildNodeOnNextTick();
        }

        private void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }

        private void OnStopping_Common()
        {
            RemoveTimer(m_restartChildTimer);
        }

        private void OnStopping_Listener()
        {
            OnStopping_Common();
            if (Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNodeOnNextTick();
            }
            else
            {
                StopNodeOnNextTick(m_isConditionMetFunc());
            }
        }

        private void OnStoppingSilent_Listener()
        {
            OnStopping_Common();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (success)
            {
                if (State == NodeState.STOPPING || m_isConditionMetFunc())
                {
                    StopNodeOnNextTick(true);
                }
                else
                {
                    m_restartChildTimer = StartFirstChildNodeOnNextTick();
                }
            }
            else
            {
                // OnStopped.Invoke(false);
                // StopNode(false);
                StopNodeOnNextTick(false);
            }
        }

        private void OnChildNodeStoppedSilent_Listener(BehaviourNode child, bool success)
        {
            if (success
                    && !(State == NodeState.STOPPING || !m_isConditionMetFunc()))
            {
                m_restartChildTimer = StartFirstChildNodeOnNextTick();
            }
        }

        #endregion Events
    }
}