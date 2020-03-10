using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourResultOverrider : BehaviourNode
    {
        bool m_result;
        public BehaviourResultOverrider(bool result) : base("ResultOverrider", NodeType.DECORATOR)
        {
            m_result = result;
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        private void OnStarted_Listener()
        {
            Children[0].StartNode();
        }

        private void OnStopping_Listener()
        {
            Children[0].RequestStopNode();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            OnStopped.Invoke(m_result);
        }
    }
}
