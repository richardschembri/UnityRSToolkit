using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    /// <summary>
    /// Overrides the child node's result
    /// </summary>
    public class BehaviourResultOverrider : BehaviourParentNode
    {
        bool m_result;

        /// <summary>
        /// Overrides the child node's result
        /// </summary>
        /// <param name="result">The result to override the child node's result</param>
        public BehaviourResultOverrider(bool result) : base("ResultOverrider", NodeType.DECORATOR)
        {
            m_result = result;
            //OnStarted.AddListener(OnStarted_Listener);
            //OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        #region Events

        private void OnStarted_Listener()
        {
            // Children[0].StartNode();
            RunOnNextTick(() => { Children[0].StartNode(); });
        }

        private void OnStopping_Listener()
        {
            // Children[0].RequestStopNode();
            RunOnNextTick(() => { Children[0].RequestStopNode(); });
        }

        /*
        public override bool StartNode(bool silent = false)
        {
            if (base.StartNode(silent))
            {
                if (!silent)
                {
                    Children[0].StartNode();
                }
                return true;
            }
            return false;
        }

        public override bool RequestStopNode(bool silent = false)
        {
            if(base.RequestStopNode(silent))
            {
                if (!silent)
                {
                    Children[0].RequestStopNode();
                }
                return true;
            }
            return false;
        }
        */

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            // OnStopped.Invoke(m_result);
            // StopNode(m_result);
            RunOnNextTick(() => {StopNode(m_result);});
        }

        #endregion Events
    }
}
