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
                Children[0].StartNode();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }

        private void OnStopping_Listener()
        {
            Children[0].RequestStopNode();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {

            OnStopped.Invoke(success);
        }
    }
}