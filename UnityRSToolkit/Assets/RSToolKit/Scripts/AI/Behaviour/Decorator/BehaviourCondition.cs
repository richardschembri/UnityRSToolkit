using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourCondition : BehaviourObserver 
    {
        private const string NODE_NAME = "Condition";
        private Func<bool> m_isConditionMetFunc;
        private float m_checkInterval;
        private float m_checkVariance;
        private NodeTimer m_conditionTimer;

        private void Init(Func<bool> isConditionMetFunc, float checkInterval = 0.0f, float checkVariance = 0.0f)
        {
            m_isConditionMetFunc = isConditionMetFunc;
            m_checkInterval = checkInterval;
            m_checkVariance = checkVariance;
        }

        public BehaviourCondition(Func<bool> isConditionMetFunc, BehaviourNode decoratee, AbortRule abortRule = AbortRule.NONE) : base(NODE_NAME, decoratee, abortRule)
        {
            Init(isConditionMetFunc);
        }
        
        public BehaviourCondition(Func<bool> isConditionMetFunc, float checkInterval, float checkVariance, BehaviourNode decoratee, AbortRule abortRule = AbortRule.NONE) : base(NODE_NAME, decoratee, abortRule)
        {
            Init(isConditionMetFunc, checkInterval, checkVariance);
        }

        protected override void StartObserving()
        {
            m_conditionTimer = AddTimer(m_checkInterval, m_checkVariance, -1, Evaluate);
        }

        protected override void StopObserving()
        {
            RemoveTimer(m_conditionTimer);
        }

        protected override bool IsConditionMet()
        {
            return m_isConditionMetFunc();
        }
    }
}
