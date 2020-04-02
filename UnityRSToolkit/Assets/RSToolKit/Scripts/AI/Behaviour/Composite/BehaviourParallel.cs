using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace RSToolkit.AI.Behaviour
{
    public class BehaviourParallel : BehaviourNode
    {
        public enum StopCondition // Policy
        {
            ONE_CHILD,
            ALL_CHILDREN
        }

        private StopCondition m_successStopCondition;
        private StopCondition m_failureStopCondition;
        public int RunningChildrenCount
        {
            get
            {
                return Children.Count(c => c.State == NodeState.ACTIVE);
            }
        }
        private int m_succeededCount = 0;
        private int m_failedCount = 0;
        private bool m_stoppedByChildren;
        bool m_success;

        public BehaviourParallel(StopCondition successStopCondition, StopCondition failureStopCondition) : base("Parallel", NodeType.COMPOSITE)
        {
            m_successStopCondition = successStopCondition;
            m_failureStopCondition = failureStopCondition;

            OnStarted.AddListener(OnStarted_Listener);
            OnStopped.AddListener(OnStopped_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        private void OnStarted_Listener()
        {
            m_stoppedByChildren = false;
            m_succeededCount = 0;
            m_failedCount = 0;
            StartChildren();
        }
        private void OnStopped_Listener(bool success)
        {

        }

        private void OnStopping_Listener()
        {
            StopChildren();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool child_success)
        {
            int child_count = Children.Count();
            if (child_success)
            {
                if (child_success) { m_succeededCount++; } else { m_failedCount++; }
                bool allChildrenStarted = RunningChildrenCount + m_succeededCount + m_failedCount == child_count;
                if (allChildrenStarted)
                {
                    if(RunningChildrenCount == 0)
                    {
                        if (m_failureStopCondition == StopCondition.ONE_CHILD && m_failedCount > 0)
                        {
                            m_success = false;
                        }
                        else if (m_successStopCondition == StopCondition.ONE_CHILD && m_succeededCount > 0)
                        {
                            m_success = true;
                        }
                        else if (m_successStopCondition == StopCondition.ALL_CHILDREN && m_succeededCount == child_count)
                        {
                            m_success = true;
                        }
                        else
                        {
                            m_success = false;
                        }
                        OnStopped.Invoke(m_success);
                    }else if (!m_stoppedByChildren)
                    {
                        if(m_failureStopCondition == StopCondition.ONE_CHILD && m_failedCount > 0)
                        {
                            m_success = false;
                            m_stoppedByChildren = true;
                            StopChildren();
                        }

                        
                    }
                }
            }
        }

        public void RestartChild(BehaviourNode child)
        {
            if (!Children.Contains(child))
            {
                throw new System.Exception("Not parent of this child!");
            }

            if (child.Result == true)
            {
                m_succeededCount--;
            }
            else
            {
                m_failedCount--;
            }
            child.StartNode();
        }
    }
}
