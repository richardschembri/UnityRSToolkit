using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.AI.Behaviour.Composite
{
    public abstract class BehaviourSequenceSelectBase : BehaviourParentNode
    {
        private int m_index = -1;
        //private NodeTimer m_processChildTimer;

        public bool IsRandom { get; private set; }
        public BehaviourSequenceSelectBase(string name, bool isRandom) : base(name, NodeType.COMPOSITE)
        {
            IsRandom = isRandom;
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        protected void ProcessChildNodeSequence(bool result_on_stop)
        {
            if (++m_index < Children.Count)
            {
                if (State == NodeState.STOPPING)
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

        private void OnStarted_Common()
        {
            ResetIndex();
            if (IsRandom)
            {
                ShuffleChildren();
            }
            AddTimer(0, 0, ProcessChildNodeSequence);
        }

        protected virtual void OnStarted_Listener()
        {
            OnStarted_Common();
            
        }

        protected virtual void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }

        private void OnStopping_Listener()
        {
            Children[m_index].RequestStopNode();
        }

        protected abstract void OnChildNodeStopped_Listener(BehaviourNode child, bool success);
        
        public virtual void StopNextChildInPriorityTo(BehaviourNode child, bool restart_child)
        {
            int next_child_index = Children.IndexOf(child) + 1;

            //while(next_child_index < Children.Count)
            for (int i = next_child_index; i < Children.Count; i++)
            {
                if(Children[i].State == NodeState.ACTIVE)
                {
                    Children[i].RequestStopNode();
                    m_index = restart_child ? i - 1 : Children.Count;
                    return;
                }
                
            }
        }
    }
}
