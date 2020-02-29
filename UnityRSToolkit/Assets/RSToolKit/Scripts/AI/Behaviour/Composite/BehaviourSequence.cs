﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Composite
{
    public class BehaviourSequence : BehaviourSequenceSelectBase 
    {
        public BehaviourSequence() : base("Sequence")
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
                OnStopped.Invoke(true);
            }
            else
            {
                ProcessChildNodeSequence();
            }
        }

    }
}
