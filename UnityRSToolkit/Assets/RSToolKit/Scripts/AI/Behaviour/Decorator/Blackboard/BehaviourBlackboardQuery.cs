using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator.Blackboard
{
    /// <summary>
    /// BlackboardCondition allows to check only one key, BlackboardQuery will observe multiple blackboard keys and evaluate 
    /// the given query function as soon as one of the value's changes, allowing you to do arbitrary queries on the blackboard. 
    /// </summary>
    public class BehaviourBlackboardQuery : BehaviourBlackboardObserver
    {
        private string[] m_observerkeys;
        private System.Func<bool> m_query;

        /// <summary>
        /// BlackboardCondition allows to check only one key, this one will observe multiple blackboard keys and evaluate 
        /// the given query function as soon as one of the value's changes, allowing you to do arbitrary queries on the blackboard. 
        /// </summary>
        /// <param name="blackboard">The blackboard that holds the queries to check</param>
        /// <param name="observerkeys">Keys of the queries to check</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="stoprule">Stop execution of running nodes based on the rule</param>
        /// <param name="query">The query to execute</param>
        public BehaviourBlackboardQuery(BehaviourBlackboard blackboard, string[] observerkeys, BehaviourNode decoratee, AbortRule stoprule, System.Func<bool> query) : base("BlackboardQuery", blackboard, decoratee, stoprule)
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