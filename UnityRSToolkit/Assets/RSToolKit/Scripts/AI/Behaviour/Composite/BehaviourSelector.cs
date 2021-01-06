using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Composite
{
    /// <summary>
    /// Run children sequentially until one succeeds (succeeds if one of the children succeeds).
    /// </summary>
    public class BehaviourSelector : BehaviourSequenceSelectBase
    {
        /// <summary>
        /// Run children sequentially until one succeeds and succeed (succeeds if one of the children succeeds).
        /// </summary>
        /// <param name="isRandom">Runs children in a pattern if true</param>
        public BehaviourSelector(bool isRandom) : base("Selector", isRandom)
        {
            
        }

        protected override void ProcessChildNodeSequence()
        {
            ProcessChildNodeSequence(false);
        }

        protected override void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(State == NodeState.INACTIVE)
            {
                return;
            }
            if (success)
            {
                // OnStopped.Invoke(true);
                // StopNode(true);
                
                StopNodeOnNextTick(true);
            }
            else
            {
                //ProcessChildNodeSequence();
                RunOnNextTick(ProcessChildNodeSequence);
            }
        }

#if UNITY_EDITOR
        protected override void InitDebugTools()
        {
            DebugTools = new BehaviourDebugTools(this, "?");
        }
#endif

    }
}
