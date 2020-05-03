using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourNode
    {
        public enum NodeState
        {
            INACTIVE,
            ACTIVE,
            STOPPING,
        }

        public enum NodeType
        {
            // Has one single child and is used to start or stop the whole behaviour tree
            ROOT, 
            // has multiple children and is used to control which of it's children are executed
            COMPOSITE,
            // always has one child and is used either to modify the result of the child or do something else whilst executing the child.
            DECORATOR,
            // the leafs of the tree that do the actual work.
            TASK
        }

        public enum Operator
        {
            IS_SET,
            IS_NOT_SET,
            IS_EQUAL,
            IS_NOT_EQUAL,
            IS_GREATER_OR_EQUAL,
            IS_GREATER,
            IS_SMALLER_OR_EQUAL,
            IS_SMALLER,
            ALWAYS_TRUE
        }

        public static class OperatorHelpers
        {
            public static string ToSymbolString(Operator op)
            {
                switch (op)
                {
                    case Operator.IS_SET:
                        return "?=";
                    case Operator.IS_NOT_SET:
                        return "?!=";
                    case Operator.IS_EQUAL:
                        return "==";
                    case Operator.IS_NOT_EQUAL:
                        return "!=";
                    case Operator.IS_GREATER_OR_EQUAL:
                        return ">=";
                    case Operator.IS_GREATER:
                        return ">";
                    case Operator.IS_SMALLER_OR_EQUAL:
                        return "<=";
                    case Operator.IS_SMALLER:
                        return "<";
                    default:
                        return op.ToString();
                }
            }
        }

        public static float ElapsedTime { get; private set; } = 0f;
        public static void UpdateTime(float deltaTime)
        {
            ElapsedTime += deltaTime;
        }

        public class NodeTimer
        {
            public double TimeOutIn { get; private set; }

            public double TimeOutAt { get; set; } = 0f;
            public int Repeat { get; set; } = 0;
            public int TimeOutCount { get; private set; } = 0;
            public bool IsFinished { get; set; } = false;
            private System.Action TimeoutAction { get; set; }

            public NodeTimer(float time, float randomVariance, int repeat, System.Action timeoutAction )
            {
                TimeoutAction = timeoutAction;
                SetTimeoutIn(time, randomVariance);
                Repeat = repeat;
                ResetTimeout();
            }
            public void SetTimeoutIn(float timeOutIn, float randomVariance)
            {
                    TimeOutIn = timeOutIn - randomVariance * 0.5f + randomVariance * UnityEngine.Random.value;
            }
            public void ResetTimeout()
            {
                TimeOutAt = BehaviourNode.ElapsedTime + TimeOutIn;
            }

            public bool IsActive
            {
                get
                {
                    return TimeOutAt < BehaviourNode.ElapsedTime;
                }
            }

            public bool Update()
            {
                if(TimeOutCount > Repeat)
                {
                    return false;
                }

                if(IsActive)
                {
                    TimeoutAction.Invoke();
                    TimeOutCount++;
                    ResetTimeout();
                }

                return true;
            }

        }

        public NodeState State { get; protected set; } = NodeState.INACTIVE;
        public bool? Result { get; private set; } = null;
        //public string Name { get; protected set; }
        public string Name { get; set; }
        //public BehaviourNode Root { get; set; }
        public NodeType Type { get; private set; }
        public BehaviourParentNode Parent { get; private set; }
        
        private List<NodeTimer> m_timers = new List<NodeTimer>();
        public ReadOnlyCollection<NodeTimer> Timers
        {
            get
            {
                return m_timers.AsReadOnly();
            }
        }
        public UnityEvent OnStarted { get; private set; } = new UnityEvent();
        public UnityEvent OnStopping { get; private set; } = new UnityEvent();
        public class OnStoppedEvent : UnityEvent<bool> { };
        public OnStoppedEvent OnStopped { get; private set; } = new OnStoppedEvent();
        


        public BehaviourNode GetRoot()
        {
            if(Type == NodeType.ROOT)
            {
                return this;
            }
            if (Parent == null)
            {
                return null;
            }else if(Parent.Type == NodeType.ROOT){
                return Parent;
            }else{
                return Parent.GetRoot();
            }
        }

        public virtual void SetParent(BehaviourParentNode parent)
        {
            if (parent != null)
            {
                if (Type == NodeType.ROOT)
                {
                    throw new System.Exception("Root nodes cannot have parents");
                }
                if (parent.Type == NodeType.TASK)
                {
                    throw new System.Exception("Tasks don`t have children");
                }
            }

            this.Parent = parent;
        }

        protected NodeTimer AddTimer(float time, float randomVariance, int repeat, System.Action timeoutAction)
        {
            var new_timer = new NodeTimer(time, randomVariance, repeat, timeoutAction);
            m_timers.Add(new_timer);
            return new_timer;
        }

        protected NodeTimer AddTimer(float time, int repeat, System.Action timeoutAction)
        {
            return AddTimer(time, 0f, repeat, timeoutAction);
        }

        protected void RemoveTimer(NodeTimer to_remove)
        {
            m_timers.Remove(to_remove);
        }

        public void StartNode()
        {
            this.Result = null;
            this.State = NodeState.ACTIVE;
            OnStarted.Invoke();
        }

        public bool RequestStopNode()
        {
            if (this.State == NodeState.ACTIVE)
            {
                this.State = NodeState.STOPPING;
                OnStopping.Invoke();
                return true;
            }
            return false;
        }

        public BehaviourNode(string name, NodeType type)
        {
            this.Name = name;
            this.Type = type;
            OnStopped.AddListener(OnStopped_Listener);
#if UNITY_EDITOR
            InitDebugTools();
#endif
        }

        private void OnStopped_Listener(bool success)
        {
            this.State = NodeState.INACTIVE;
            this.Result = success;
            Parent?.OnChildNodeStopped.Invoke(this, success);
        }

        public virtual void Update()
        {
            for(int i = 0; i < m_timers.Count; i++)
            {
                m_timers[i].Update();
            }
        }



#if UNITY_EDITOR
        public BehaviourDebugTools DebugTools { get; protected set; }

        protected virtual void InitDebugTools()
        {
            DebugTools = new BehaviourDebugTools(this);
        }
#endif
    }


}
