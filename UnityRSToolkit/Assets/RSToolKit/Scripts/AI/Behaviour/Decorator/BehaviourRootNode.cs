using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourRootNode : BehaviourParentNode
    {
        public BehaviourRootNode(string name = "Root") : base(name, NodeType.DECORATOR)
        {
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (State != NodeState.STOPPING)
            {
                Children[0].StartNode();
            }
            else
            {
                //this.blackboard.Disable();
                OnStopped.Invoke(success);
            }
        }

        // To Refactor
        public override void SetParent(BehaviourParentNode parent)
        {
            throw new System.Exception("Root nodes cannot have parents");
        }
    }
}
