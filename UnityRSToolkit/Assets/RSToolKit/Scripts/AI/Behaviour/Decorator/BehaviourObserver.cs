using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour.Composite;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public abstract class BehaviourObserver : BehaviourNode
    {
        public enum StopRule
        {
            NONE,
            SELF,
            LOWER_PRIORITY,
            BOTH,
            LOWER_PRIORITY_RESTART,
            RESTART
        }

        private bool m_isObserving;
        private StopRule m_stoprule;
        public BehaviourObserver(string name, StopRule stoprule) : base(name, NodeType.DECORATOR)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);

            if(Parent.Type == NodeType.COMPOSITE)
            {
                Parent.OnStopped.AddListener(OnCompositeParentStopped_Listener);
            }

            m_stoprule = stoprule;
            m_isObserving = false;
        }
        protected abstract void StartObserving();

        protected abstract void StopObserving();

        protected abstract bool IsConditionMet();
        private void OnStarted_Listener()
        {
            if(m_stoprule != StopRule.NONE)
            {
                if (m_isObserving)
                {
                    m_isObserving = false;
                    StopObserving();
                }
            }
            if (!IsConditionMet())
            {
                OnStopped.Invoke(false);
            }
            else
            {
                Children[0].StartNode();
            }
        }

        private void OnStopping_Listener()
        {
            Children[0].StartNode();
        }
        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(m_stoprule == StopRule.NONE || m_stoprule == StopRule.SELF)
            {
                if (m_isObserving)
                {
                    m_isObserving = false;
                    StopObserving();
                }
            }
            OnStopped.Invoke(success);
        }
        private void OnCompositeParentStopped_Listener(bool success)
        {
            if (m_isObserving)
            {
                m_isObserving = false;
                StopObserving();
            }
        }

        protected void Evaluate()
        {
            if (m_stoprule == StopRule.LOWER_PRIORITY || m_stoprule == StopRule.BOTH || m_stoprule == StopRule.RESTART || m_stoprule == StopRule.LOWER_PRIORITY_RESTART)
            {
                BehaviourNode parentNode = this.Parent;
                BehaviourNode childNode = this;
                if(parentNode.Type == NodeType.COMPOSITE)
                {
                    childNode = Parent;
                    parentNode = parentNode.Parent;
                }
                if(m_stoprule == StopRule.RESTART || m_stoprule == StopRule.LOWER_PRIORITY_RESTART)
                {
                    if (m_isObserving)
                    {
                        m_isObserving = false;
                        StopObserving();
                    }
                }
                if(parentNode is BehaviourSequenceSelectBase)
                {
                    ((BehaviourSequenceSelectBase)parentNode).StopNextChildInPriorityTo(childNode, m_stoprule == StopRule.RESTART || m_stoprule == StopRule.LOWER_PRIORITY_RESTART);
                }else if(parentNode is BehaviourParallel)
                {
                    ((BehaviourParallel)parentNode).RestartChild(childNode);
                }
            }
        }
    }
}
