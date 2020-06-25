using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Composite
{
    /// <summary>
    /// Run children sequentially until one fails and fail (succeeds if none of the children fail).
    /// </summary>
    public class BehaviourSequence : BehaviourSequenceSelectBase 
    {
        /// <summary>
        /// Run children sequentially until one fails and fail (succeeds if none of the children fail).
        /// </summary>
        /// <param name="isRandom">Runs children in a pattern if true</param>
        public BehaviourSequence(bool isRandom) : base("Sequence", isRandom)
        {
        }

        protected override void ProcessChildNodeSequence()
        {
            ProcessChildNodeSequence(true);
        }


        protected override void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(State == NodeState.INACTIVE)
            {
                return;
            }

            if (success)
            {
                //ProcessChildNodeSequence();
                RunOnNextTick(ProcessChildNodeSequence);
            }
            else
            {
                // OnStopped.Invoke(false);
                // StopNode(false);

                StopNodeOnNextTick(false);
            }
        }

#if UNITY_EDITOR
        protected override void InitDebugTools()
        {
            DebugTools = new BehaviourDebugTools(this, "->");
        }
#endif

    }
}
