using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour.Decorator
{
    /// <summary>
    /// Execute child node if the given condition returns true
    /// </summary>
    public class BehaviourCondition : BehaviourObserver 
    {
        private const string NODE_NAME = "Condition";
        private Func<bool> _isConditionMetFunc;
        private float _checkInterval;
        private float _checkVariance;
        private NodeTimer _conditionTimer;

        protected void Init(Func<bool> isConditionMetFunc, float checkInterval = 0.0f, float checkVariance = 0.0f)
        {
            _isConditionMetFunc = isConditionMetFunc;
            _checkInterval = checkInterval;
            _checkVariance = checkVariance;
        }

        #region Contructors

        protected BehaviourCondition(BehaviourNode decoratee, string nodeName = NODE_NAME, AbortRule abortRule = AbortRule.NONE) : base(NODE_NAME, decoratee, abortRule)
        {
        }
        /// <summary>
        /// Execute child node if the given condition returns true
        /// </summary>
        /// <param name="isConditionMetFunc">The function used to check the condition</param>
        /// <param name="decoratee">The child run to run if condition is true</param>
        /// <param name="abortRule">The parameter allows the Decorator to stop the execution of a running subtree within it's parent's Composite.</param>
        public BehaviourCondition(Func<bool> isConditionMetFunc, BehaviourNode decoratee, AbortRule abortRule = AbortRule.NONE) : base(NODE_NAME, decoratee, abortRule)
        {
            Init(isConditionMetFunc);
        }

        /// <summary>
        /// Execute child node if the given condition returns true
        /// </summary>
        /// <param name="isConditionMetFunc">The function used to check the condition</param>
        /// <param name="checkInterval">The interval at which the condition is checked</param>
        /// <param name="checkVariance">The interval variance</param>
        /// <param name="decoratee">The child run to run if condition is true</param>
        /// <param name="abortRule">The parameter allows the Decorator to stop the execution of a running subtree within it's parent's Composite.</param>
        public BehaviourCondition(Func<bool> isConditionMetFunc, float checkInterval, float checkVariance, BehaviourNode decoratee, AbortRule abortRule = AbortRule.NONE) : base(NODE_NAME, decoratee, abortRule)
        {
            Init(isConditionMetFunc, checkInterval, checkVariance);
        }

        #endregion Contructors

        protected override void StartObserving()
        {
            _conditionTimer = AddTimer(_checkInterval, _checkVariance, -1, Evaluate);
        }

        protected override void StopObserving()
        {
            RemoveTimer(_conditionTimer);
        }

        protected override bool IsConditionMet()
        {
            return _isConditionMetFunc();
        }
    }
}
