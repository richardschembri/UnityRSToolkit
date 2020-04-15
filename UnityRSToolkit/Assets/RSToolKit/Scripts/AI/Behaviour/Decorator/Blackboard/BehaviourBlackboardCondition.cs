using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator.Blackboard
{
    public class BehaviourBlackboardCondition : BehaviourBlackboardObserver
    {
        public string Key { get; private set; }
        public object ConditionValue { get; private set; }
        public Operator ConditionOperator { get; private set; }

        public BehaviourBlackboardCondition(BehaviourBlackboard blackboard, string key, Operator conditionoperator, object conditionvalue, StopRule stoprule) : base("BlackboardCondition", blackboard, stoprule)
        {
            ConditionOperator = conditionoperator;
            Key = key;            
            ConditionValue = conditionvalue; 
        }

        public BehaviourBlackboardCondition(BehaviourBlackboard blackboard, string key, Operator conditionoperator, StopRule stoprule) : base("BlackboardCondition", blackboard, stoprule)
        {
            ConditionOperator = conditionoperator;
            Key = key;
        }

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

        public override string ToString()
        {
            return $"({ConditionOperator}) {Key} ? {ConditionValue}";
        }
    }
}
