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

        public NodeState CurrentState { get; protected set; } = NodeState.INACTIVE;
        public string Name { get; private set; }

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

    }
}
