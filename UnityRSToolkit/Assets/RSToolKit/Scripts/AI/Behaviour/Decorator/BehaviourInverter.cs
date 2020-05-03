using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourInverter : BehaviourParentNode
    {
        public BehaviourInverter(BehaviourNode child) : base("Inverter", NodeType.DECORATOR)
        {
            AddChild(child);
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
            OnStopped.Invoke(!success);
        }
    }
}
