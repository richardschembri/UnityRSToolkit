using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Composite
{
    public class BehaviourSequence : BehaviourNode
    {
        private int m_index = -1;

        public BehaviourSequence(string name) : base(name, NodeType.COMPOSITE)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        private void ProcessChildNodes()
        {
            if (++m_index < Children.Count)
            {
                if (CurrentState == NodeState.STOPPING)
                {
                    OnStopped.Invoke(false);
                }
                else
                {
                    Children[m_index].StartNode();
                }
            }
            else
            {
                OnStopped.Invoke(true);
            }
        }

        private void ResetIndex()
        {
            m_index = -1;
        }
        private void OnStarted_Listener()
        {
            ResetIndex();
            ProcessChildNodes();
        }

        private void OnStopping_Listener()
        {
            Children[m_index].RequestStopNode();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (success)
            {
                ProcessChildNodes();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }
    }
}
