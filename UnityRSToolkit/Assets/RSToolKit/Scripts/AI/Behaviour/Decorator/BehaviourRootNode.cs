using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourRootNode : BehaviourParentNode
    {
        private NodeTimer m_rootTimer;
        public BehaviourRootNode(string name = "Root") : base(name, NodeType.DECORATOR)
        {
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnStopped.AddListener(OnStopped_Listener);
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (State != NodeState.STOPPING)
            {
                //Children[0].StartNode();
                // wait one tick, to prevent endless recursions
                m_rootTimer = AddTimer(0, 0, Children[0].StartNode);
            }
            else
            {
                //this.blackboard.Disable();
                OnStopped.Invoke(success);
            }
        }

        private void OnStopped_Listener(bool success)
        {
            if (this.Children[0].State == NodeState.ACTIVE)
            {
                this.Children[0].RequestStopNode();
            }
            else
            {
                RemoveTimer(m_rootTimer);
            }
        }

        // To Refactor
        public override void SetParent(BehaviourParentNode parent)
        {
            throw new System.Exception("Root nodes cannot have parents");
        }
    }
}
