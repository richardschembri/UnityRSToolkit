using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.AI.Behaviour
{
    public abstract class BehaviourNode
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

        public static float ElipsedTime { get; private set; } = 0f;
        public static void UpdateTime(float deltaTime)
        {
            ElipsedTime += deltaTime;
        }

        protected class NodeTimer
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
                TimeOutAt = BehaviourNode.ElipsedTime + TimeOutIn;
            }

            public bool Update()
            {
                if(TimeOutCount > Repeat)
                {
                    return false;
                }

                if(TimeOutAt < BehaviourNode.ElipsedTime)
                {
                    TimeoutAction.Invoke();
                    TimeOutCount++;
                    ResetTimeout();
                }

                return true;
            }

        }

        public NodeState CurrentState { get; protected set; } = NodeState.INACTIVE;
        public string Name { get; protected set; }

        //public BehaviourNode Root { get; set; }
        public NodeType Type { get; private set; }
        public BehaviourNode Parent { get; private set; }
        private List<BehaviourNode> m_children = new List<BehaviourNode>();
        public ReadOnlyCollection<BehaviourNode> Children
        {
            get
            {
                return m_children.AsReadOnly();
            }
        }
        private List<NodeTimer> m_timers = new List<NodeTimer>();
        public UnityEvent OnStarted { get; private set; } = new UnityEvent();
        public UnityEvent OnStopping { get; private set; } = new UnityEvent();
        public class OnStoppedEvent : UnityEvent<bool> { };
        public OnStoppedEvent OnStopped { get; private set; } = new OnStoppedEvent();
        public class OnChildNodeStoppedEvent : UnityEvent<BehaviourNode, bool> { }
        public OnChildNodeStoppedEvent OnChildNodeStopped { get; private set; } = new OnChildNodeStoppedEvent();
        public class OnChildNodeAddRemoveEvent : UnityEvent<BehaviourNode, BehaviourNode> { } // parent, child
        public OnChildNodeAddRemoveEvent OnChildNodeAdded = new OnChildNodeAddRemoveEvent();
        public OnChildNodeAddRemoveEvent OnChildNodeRemoved = new OnChildNodeAddRemoveEvent();


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

        public void SetParent(BehaviourNode parent)
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

        public void AddChild(BehaviourNode child)
        {
            switch (Type)
            {
                case NodeType.TASK:
                    throw new System.Exception("Tasks don`t have children");
                    break;
                case NodeType.ROOT:
                case NodeType.DECORATOR:
                    if(Children.Count > 0)
                    {
                        throw new System.Exception("Too many children");
                    }
                    break;

            }
            
            child.SetParent(this);
            m_children.Add(child);
            OnChildNodeAdded.Invoke(this, child);
        }

        protected NodeTimer AddTimer(float time, float randomVariance, int repeat, System.Action timeoutAction)
        {
            var new_timer = new NodeTimer(time, randomVariance, repeat, timeoutAction);
            m_timers.Add(new_timer);
            return new_timer;
        }

        protected void RemoveTimer(NodeTimer to_remove)
        {
            m_timers.Remove(to_remove);
        }

        public void RemoveChild(BehaviourNode child)
        {
            child.SetParent(null);
            m_children.Remove(child);
            OnChildNodeRemoved.Invoke(this, child);
        }

        public void StartNode()
        {
            this.CurrentState = NodeState.ACTIVE;
            OnStarted.Invoke();
        }

        public void RequestStopNode()
        {
            this.CurrentState = NodeState.STOPPING;
            OnStopping.Invoke();
        }

        public BehaviourNode(string name, NodeType type)
        {
            this.Name = name;
            this.Type = type;
            OnStopped.AddListener(OnStopped_Listener);
        }

        private void OnStopped_Listener(bool success)
        {
            this.CurrentState = NodeState.INACTIVE;
            Parent?.OnChildNodeStopped.Invoke(this, success);
        }

        public virtual void Update()
        {
            for(int i = 0; i < m_timers.Count; i++)
            {
                m_timers[i].Update();
            }
        }

        public void UpdateRecursively()
        {
            if (CurrentState == NodeState.INACTIVE)
            {
                return;
            }
            for(int i = 0; i < Children.Count; i++)
            {
                if(Children[i].CurrentState != NodeState.INACTIVE)
                {
                    Children[i].UpdateRecursively();
                }
            }
            Update();
        }
    }
}
