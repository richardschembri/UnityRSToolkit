using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    /// <summary>
    /// Runs the child node with the given probability chance between 0 and 1
    /// </summary>
    public class BehaviourRandom : BehaviourParentNode
    {
        private float m_probability;

        /// <summary>
        /// Runs the child node with the given probability chance between 0 and 1
        /// </summary>
        /// <param name="probability">Between 0 and 1</param>
        public BehaviourRandom(float probability) : base("Random", NodeType.DECORATOR)
        {
            m_probability = probability;
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        private void OnStarted_Listener()
        {
            if (UnityEngine.Random.value <= m_probability)
            {
                // Children[0].StartNode();
                RunOnNextTick(()=>{ Children[0].StartNode();});
            }
            else
            {
                // OnStopped.Invoke(false);
                // StopNode(false);
                RunOnNextTick(()=>{ StopNode(false); });
            }
        }

        private void OnStopping_Listener()
        {
            // Children[0].RequestStopNode();
            RunOnNextTick(()=>{ Children[0].RequestStopNode(); });
        }

        /*
        public override bool StartNode(bool silent = false)
        {
            if (base.StartNode(silent))
            {
                if (UnityEngine.Random.value <= m_probability)
                {
                    Children[0].StartNode();
                }
                else
                {
                    // OnStopped.Invoke(false);
                    StopNode(false);
                }
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
        */

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            // OnStopped.Invoke(success);
            // StopNode(success);
            RunOnNextTick(()=>{StopNode(success);});
        }

    }
}