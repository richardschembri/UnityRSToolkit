using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourConditionInverse : BehaviourCondition
    {
        private const string NODE_NAME = "Condition Inverse";

        /// <summary>
        /// Execute child node if the given condition inverse returns true
        /// </summary>
        /// <param name="isConditionMetFunc">The function used to check the condition</param>
        /// <param name="decoratee">The child run to run if condition is true</param>
        /// <param name="abortRule">The parameter allows the Decorator to stop the execution of a running subtree within it's parent's Composite.</param>
        public BehaviourConditionInverse(Func<bool> isConditionMetFunc, BehaviourNode decoratee, AbortRule abortRule = AbortRule.NONE) : base(isConditionMetFunc, decoratee, abortRule)
        {
            Name = NODE_NAME;
            
        }

        /// <summary>
        /// Execute child node if the given condition inverse returns true
        /// </summary>
        /// <param name="isConditionMetFunc">The function used to check the condition</param>
        /// <param name="checkInterval">The interval at which the condition is checked</param>
        /// <param name="checkVariance">The interval variance</param>
        /// <param name="decoratee">The child run to run if condition is true</param>
        /// <param name="abortRule">The parameter allows the Decorator to stop the execution of a running subtree within it's parent's Composite.</param>
        public BehaviourConditionInverse(Func<bool> isConditionMetFunc, float checkInterval, float checkVariance, BehaviourNode decoratee, AbortRule abortRule = AbortRule.NONE) : base(isConditionMetFunc, checkInterval, checkVariance, decoratee, abortRule)
        {            
            Name = NODE_NAME;
        }

        protected override bool IsConditionMet()
        {
            return !base.IsConditionMet();
        }
    }
}
