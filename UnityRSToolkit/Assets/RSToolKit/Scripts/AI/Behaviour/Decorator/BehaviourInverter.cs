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
            // OnStarted.AddListener(OnStarted_Listener);
            // OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        #region Events
        /*
        private void OnStarted_Listener()
        {
            Children[0].StartNode();
            
        }

        private void OnStopping_Listener()
        {
            Children[0].RequestStopNode();
        }
        */

        public override bool StartNode(bool silent = false)
        {
            if (base.StartNode(silent))
            {
                Children[0].StartNode();
                return true;
            }
            return false;
        }

        public override bool RequestStopNode(bool silent = false)
        {
            if (base.RequestStopNode(silent))
            {
                Children[0].RequestStopNode();
                return true;
            }
            return false;
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            // OnStopped.Invoke(!success);
            //StopNode(!success);
            RunOnNextTick(ProcessChildStopped);
        }


        private void ProcessChildStopped()
        {
            StopNode(!Children[0].Result.Value);
        }

        #endregion Events

    }
}
