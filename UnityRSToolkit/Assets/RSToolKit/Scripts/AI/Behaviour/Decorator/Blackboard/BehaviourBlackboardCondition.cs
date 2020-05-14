using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator.Blackboard
{
    /// <summary>
    /// Execute the child node only if the Blackboard's key matches the op / value condition. 
    /// If stopsOnChange is not NONE, the node will observe the Blackboard for changes and stop 
    /// execution of running nodes based on the stopsOnChange stops rules.
    /// </summary>
    public class BehaviourBlackboardCondition : BehaviourBlackboardObserver
    {
        public string Key { get; private set; }
        public object ConditionValue { get; private set; }
        public Operator ConditionOperator { get; private set; }

        private void Init(string key, Operator conditionoperator, object conditionvalue = null)
        {
            ConditionOperator = conditionoperator;
            Key = key;
            ConditionValue = conditionvalue;
        }

        #region Constructors

        /// <summary>
        /// Execute the child node only if the Blackboard's key matches the op / value condition.
        /// </summary>
        /// <param name="blackboard">The blackboard that holds the value to check and checks the condition</param>
        /// <param name="key">The key of the value</param>
        /// <param name="conditionoperator">The operator used to check the value</param>
        /// <param name="conditionvalue"></param>
        /// <param name="decoratee">The child node</param>
        /// <param name="stoprule">Stop execution of running nodes based on the rule</param>
        public BehaviourBlackboardCondition(BehaviourBlackboard blackboard, string key, Operator conditionoperator, object conditionvalue, BehaviourNode decoratee, AbortRule stoprule) : base("BlackboardCondition", blackboard, decoratee, stoprule)
        {
            Init(key, conditionoperator, conditionvalue);
        }

        /// <summary>
        /// Execute the child node only if the Blackboard's key matches the op / value condition.
        /// </summary>
        /// <param name="blackboard">The blackboard that holds the value to check and checks the condition</param>
        /// <param name="key">The key of the value</param>
        /// <param name="conditionoperator">The operator used to check the value</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="stoprule"></param>
        public BehaviourBlackboardCondition(BehaviourBlackboard blackboard, string key, Operator conditionoperator, BehaviourNode decoratee, AbortRule stoprule) : base("BlackboardCondition", blackboard, decoratee, stoprule)
        {
            Init(key, conditionoperator);
        }

        #endregion Constructors

        private void onValueChanged(BehaviourBlackboard.NotificationType notificationtype, object newValue)
        {
            Evaluate();
        }

        protected override void StartObserving()
        {
            blackboard.AddObserver(Key, onValueChanged);
        }

        protected override void StopObserving()
        {
            blackboard.RemoveObserver(Key, onValueChanged);
        }

        protected override bool IsConditionMet()
        {
            if (ConditionOperator == Operator.ALWAYS_TRUE)
            {
                return true;
            }

            if (!blackboard.IsSet(Key))
            {
                return ConditionOperator == Operator.IS_NOT_SET;
            }

            object blackboard_value = blackboard.Get(Key);

            switch (ConditionOperator)
            {
                case Operator.IS_SET: return true;
                case Operator.IS_EQUAL: return object.Equals(blackboard_value, ConditionValue);
                case Operator.IS_NOT_EQUAL: return !object.Equals(blackboard_value, ConditionValue);

                case Operator.IS_GREATER_OR_EQUAL:
                    if (blackboard_value is float)
                    {
                        return (float)blackboard_value >= (float)ConditionValue;
                    }
                    else if (blackboard_value is int)
                    {
                        return (int)blackboard_value >= (int)ConditionValue;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + blackboard_value.GetType());
                        return false;
                    }

                case Operator.IS_GREATER:
                    if (blackboard_value is float)
                    {
                        return (float)blackboard_value > (float)ConditionValue;
                    }
                    else if (blackboard_value is int)
                    {
                        return (int)blackboard_value > (int)ConditionValue;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + blackboard_value.GetType());
                        return false;
                    }

                case Operator.IS_SMALLER_OR_EQUAL:
                    if (blackboard_value is float)
                    {
                        return (float)blackboard_value <= (float)ConditionValue;
                    }
                    else if (blackboard_value is int)
                    {
                        return (int)blackboard_value <= (int)ConditionValue;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + blackboard_value.GetType());
                        return false;
                    }

                case Operator.IS_SMALLER:
                    if (blackboard_value is float)
                    {
                        return (float)blackboard_value < (float)ConditionValue;
                    }
                    else if (blackboard_value is int)
                    {
                        return (int)blackboard_value < (int)ConditionValue;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + blackboard_value.GetType());
                        return false;
                    }

                default: return false;
            }
        }

#if UNITY_EDITOR
        protected override void InitDebugTools()
        {
            DebugTools = new BehaviourDebugTools(this, $"{Key} {OperatorHelpers.ToSymbolString(ConditionOperator)} {ConditionValue}");
        }
#endif

        public override string ToString()
        {
            return $"({ConditionOperator}) {Key} ? {ConditionValue}";
        }
    }
}
