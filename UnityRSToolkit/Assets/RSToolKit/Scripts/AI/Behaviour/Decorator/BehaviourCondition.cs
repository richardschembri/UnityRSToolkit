using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourCondition : BehaviourObserver 
    {
        private Func<bool> m_isConditionMetFunc;
        private float m_checkInterval;
        private float m_checkVariance;
        private NodeTimer m_conditionTimer;

        public BehaviourCondition(Func<bool> isConditionMetFunc) : base("Condition", StopRule.NONE)
        {
            m_isConditionMetFunc = isConditionMetFunc ;
            m_checkInterval = 0.0f;
            m_checkVariance = 0.0f;
        }
        public BehaviourCondition(Func<bool> isConditionMetFunc, float checkInterval , float checkVariance ) : base("Condition", StopRule.NONE)
        {
            m_isConditionMetFunc = isConditionMetFunc ;
            m_checkInterval = checkInterval;
            m_checkVariance = checkVariance;
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
