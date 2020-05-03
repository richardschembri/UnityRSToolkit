using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourRandom : BehaviourParentNode
    {
        private float m_probability;

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