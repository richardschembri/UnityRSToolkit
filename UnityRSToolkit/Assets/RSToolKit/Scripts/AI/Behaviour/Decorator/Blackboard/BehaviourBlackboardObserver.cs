﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator.Blackboard
{
    public abstract class BehaviourBlackboardObserver : BehaviourObserver
    {
        protected BehaviourBlackboard blackboard;
        public BehaviourBlackboardObserver(string name, BehaviourBlackboard blackboard, BehaviourNode decoratee, AbortRule stopRule) :base(name, decoratee, stopRule)
        {
            this.blackboard = blackboard;
        }

        public override void Update(UpdateType updateType = UpdateType.DEFAULT)
        {
            base.Update(updateType);
            // blackboard.Update();
        }
    }
}
