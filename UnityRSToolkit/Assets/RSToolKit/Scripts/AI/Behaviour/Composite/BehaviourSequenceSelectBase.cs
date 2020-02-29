﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Composite
{
    public abstract class BehaviourSequenceSelectBase : BehaviourNode
    {
        private int m_index = -1;

        public BehaviourSequenceSelectBase(string name) : base(name, NodeType.COMPOSITE)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        protected void ProcessChildNodeSequence(bool result_on_stop)
        {
            if (++m_index < Children.Count)
            {
                if (CurrentState == NodeState.STOPPING)
                {

                    // Stopped manually
                    OnStopped.Invoke(false);
                }
                else
                {
                    // Run next child in sequence
                    Children[m_index].StartNode();
                }
            }
            else
            {
                // Finished running all children
                OnStopped.Invoke(result_on_stop);
            }
        }

        protected abstract void ProcessChildNodeSequence();

        private void ResetIndex()
        {
            m_index = -1;
        }
        protected virtual void OnStarted_Listener()
        {
            ResetIndex();
            ProcessChildNodeSequence();
        }

        private void OnStopping_Listener()
        {
            Children[m_index].RequestStopNode();
        }

        protected abstract void OnChildNodeStopped_Listener(BehaviourNode child, bool success);
        
        public virtual void StopNextChildInPriorityTo(BehaviourNode child, bool restart_child)
        {
            int next_child_index = Children.IndexOf(child) + 1;
            if(next_child_index < Children.Count)
            {
                if(Children[next_child_index].CurrentState == NodeState.ACTIVE)
                {
                    Children[next_child_index].RequestStopNode();
                    m_index = restart_child ? next_child_index - 1 : Children.Count;
                }
            }
        }
    }
}
