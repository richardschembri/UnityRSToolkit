using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{

    /// <summary>
    /// Inverts the child node's result
    /// </summary>
    public class BehaviourInverter : BehaviourParentNode
    {
        /// <summary>
        /// Inverts the child node result.
        /// </summary>
        /// <param name="child">The child node</param>
        public BehaviourInverter(BehaviourNode child) : base("Inverter", NodeType.DECORATOR)
        {
            AddChild(child);
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        #region Events

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

        #endregion Events

    }
}
