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
        private bool m_loopCountSkip = false;
        public int TotalLoops { get; private set; }  = -1;
        public int LoopCount { get; private set; } = 0;

        NodeTimer m_restartChildTimer;

        /// <summary>
        /// Repeated runs it's child node
        /// </summary>
        /// <param name="totalLoops">The amount of times the child node is looped. If -1 it will loop indefinately unless stopped manually.</param>
        public BehaviourRepeater(int totalLoops = -1) : base("Repeater", NodeType.DECORATOR)
        {
            TotalLoops = totalLoops;
            OnStarted.AddListener(OnStarted_Listener);
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnStoppingSilent.AddListener(OnStoppingSilent_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnChildNodeStoppedSilent.AddListener(OnChildNodeStoppedSilent_Listener);
        }

        public bool StartNode(int loopCount, bool silent = false)
        {
            LoopCount = loopCount;
            m_loopCountSkip = true;
            return StartNode(silent);
        }

        #region Events

        private void OnStarted_Common()
        {
            RemoveTimer(m_restartChildTimer);

            if (!m_loopCountSkip)
            {
                LoopCount = 0;
            }
            else
            {
                m_loopCountSkip = false;
            }
        }

        private void OnStarted_Listener()
        {
            OnStarted_Common();
            Children[0].StartNode();
        }

        private void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }

        private void OnStopping_Common(){
            RemoveTimer(m_restartChildTimer);
        }
        private void OnStopping_Listener()
        {
            OnStopping_Common();
            if(Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                OnStopped.Invoke(TotalLoops == -1);
            }
        }

        private void OnStoppingSilent_Listener(){
            OnStopping_Common();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (success)
            {
                if(State == NodeState.STOPPING || (TotalLoops >= 0 && ++LoopCount >= TotalLoops))
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

        private void OnChildNodeStoppedSilent_Listener(BehaviourNode child, bool success)
        {
            if (success 
                    && !(State == NodeState.STOPPING || (TotalLoops >= 0 && ++LoopCount >= TotalLoops)))
            {
                m_restartChildTimer = AddTimer(0, 0, 0, RestartChild);
            }
        }

        #endregion Events

        protected void RestartChild()
        {
            Children[0].StartNode();
        }

    }
}
