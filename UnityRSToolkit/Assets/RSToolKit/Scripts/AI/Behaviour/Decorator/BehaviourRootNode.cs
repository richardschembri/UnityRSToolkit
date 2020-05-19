using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{

    /// <summary>
    /// The root node of a tree
    /// </summary>
    public class BehaviourRootNode : BehaviourParentNode
    {
        private NodeTimer m_rootTimer;
        /// <summary>
        /// The root node of a tree
        /// </summary>
        /// <param name="name"></param>
        public BehaviourRootNode(string name = "Root") : base(name, NodeType.DECORATOR)
        {
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            //OnStopped.AddListener(OnStopped_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnStarted.AddListener(OnStarted_Listener);
        }

        #region Events

        private void OnStarted_Listener()
        {
            StartChildNode();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (State != NodeState.STOPPING)
            {
                //Children[0].StartNode();
                // wait one tick, to prevent endless recursions
                m_rootTimer = AddTimer(0, 0, StartChildNode);
            }
            else
            {
                //this.blackboard.Disable();
                OnStopped.Invoke(success);
            }
        }

        //private void OnStopped_Listener(bool success)
        private void OnStopping_Listener()
        {
            if (this.Children[0].State == NodeState.ACTIVE)
            {
                this.Children[0].RequestStopNode();
            }
            else
            {
                RemoveTimer(m_rootTimer);
            }
            OnStopped.Invoke(true);
        }

        #endregion Events

        // To Refactor
        public override void SetParent(BehaviourParentNode parent)
        {
            throw new System.Exception("Root nodes cannot have parents");
        }

        private void StartChildNode()
        {
            Children[0].StartNode();
        }
    }
}
