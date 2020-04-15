using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator.Blackboard
{
    public abstract class BehaviourBlackboardObserver : BehaviourObserver
    {
        protected BehaviourBlackboard blackboard;
        public BehaviourBlackboardObserver(string name, BehaviourBlackboard blackboard, StopRule stopRule) :base(name, stopRule)
        {
            this.blackboard = blackboard;
        }

        public override void Update()
        {
            base.Update();
            // blackboard.Update();
        }
    }
}
