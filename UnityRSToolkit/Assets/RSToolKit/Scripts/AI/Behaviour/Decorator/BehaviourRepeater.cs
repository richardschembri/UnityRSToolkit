using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    /// <summary>
    /// Repeatedly runs it's child node
    /// </summary>
    public class BehaviourRepeater : BehaviourParentNode
    {
        int m_totalLoops = -1;
        int m_loopCount = 0;

        NodeTimer m_restartChildTimer;

        /// <summary>
        /// Repeated runs it's child node
        /// </summary>
        /// <param name="totalLoops">The amount of times the child node is looped. If -1 it will loop indefinately unless stopped manually.</param>
        public BehaviourRepeater(int totalLoops = -1) : base("Repeater", NodeType.DECORATOR)
        {
            m_totalLoops = totalLoops;
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        #region Events

        private void OnStarted_Listener()
        {
            RemoveTimer(m_restartChildTimer);
            m_loopCount = 0;
            Children[0].StartNode();
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
                OnStopped.Invoke(m_totalLoops == -1);
            }
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (success)
            {
                if(State == NodeState.STOPPING || (m_totalLoops >= 0 && ++m_loopCount >= m_totalLoops))
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

        #endregion Events

        protected void RestartChild()
        {
            Children[0].StartNode();
        }

    }
}
