using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourRepeater : BehaviourNode
    {
        uint m_totalLoops = 0;
        uint m_loopCount = 0;
        NodeTimer m_restartChildTimer;
        public BehaviourRepeater(BehaviourNode child, uint totalLoops): base("Repeater", NodeType.DECORATOR)
        {
            m_totalLoops = totalLoops;
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        private void OnStarted_Listener()
        {
            RemoveTimer(m_restartChildTimer);
            if(m_totalLoops > 0)
            {
                m_loopCount = 0;
                Children[0].StartNode();
            }
            else
            {
                OnStopped.Invoke(true);
            }
        }

        private void OnStopping_Listener()
        {
            RemoveTimer(m_restartChildTimer);
            
            if(Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (success)
            {
                if(State == NodeState.STOPPING || (m_totalLoops > 0 && ++m_loopCount >= m_totalLoops))
                {
            
                    OnStopped.Invoke(true);
                }
                else
                {
                    m_restartChildTimer = AddTimer(0, 0, 0, RestartChild);
                }
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }

        protected void RestartChild()
        {
            Children[0].StartNode();
        }

    }
}
