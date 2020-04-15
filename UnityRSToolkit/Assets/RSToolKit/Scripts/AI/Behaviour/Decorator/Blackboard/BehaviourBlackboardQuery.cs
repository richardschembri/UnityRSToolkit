using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator.Blackboard
{
    public class BehaviourBlackboardQuery : BehaviourBlackboardObserver
    {
        private string[] m_observerkeys;
        private System.Func<bool> m_query;
        

        public BehaviourBlackboardQuery(BehaviourBlackboard blackboard, string[] observerkeys, StopRule stoprule, System.Func<bool> query) : base("BlackboardQuery", blackboard, stoprule)
        {
            m_observerkeys = observerkeys;
            m_query = query;
        }

        private void onValueChanged(BehaviourBlackboard.NotificationType notificationtype, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            return m_query();
        }

        protected override void StartObserving()
        {
            for (int i = 0; i < m_observerkeys.Length; i++)
            {
                this.blackboard.AddObserver(m_observerkeys[i], onValueChanged);
            }
            
        }

        protected override void StopObserving()
        {
            for(int i = 0; i < m_observerkeys.Length; i++)
            {
                this.blackboard.RemoveObserver(m_observerkeys[i], onValueChanged);
            }
        }

        public override string ToString()
        {
            var sbKeys = new System.Text.StringBuilder();
            sbKeys.Append(Name);
            for (int i = 0; i < m_observerkeys.Length; i++)
            {
                sbKeys.Append($" {m_observerkeys[i]}");
            }
            
            return sbKeys.ToString();
        }
    }
}