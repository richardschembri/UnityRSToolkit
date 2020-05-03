using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Composite
{
    public class BehaviourSequence : BehaviourSequenceSelectBase 
    {
        public BehaviourSequence(bool isRandom) : base("Sequence", isRandom)
        {
        }

        protected override void ProcessChildNodeSequence()
        {
            ProcessChildNodeSequence(true);
        }


        protected override void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (success)
            {
                ProcessChildNodeSequence(); 
            }
            else
            {
                OnStopped.Invoke(false);
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
