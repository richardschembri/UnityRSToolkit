using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Helpers;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourParentNode : BehaviourNode
    {
        private List<BehaviourNode> m_children = new List<BehaviourNode>();
        public class OnChildNodeStoppedEvent : UnityEvent<BehaviourNode, bool> { }
        public OnChildNodeStoppedEvent OnChildNodeStopped { get; private set; } = new OnChildNodeStoppedEvent();
        public class OnChildNodeAddRemoveEvent : UnityEvent<BehaviourNode, BehaviourNode> { } // parent, child
        public OnChildNodeAddRemoveEvent OnChildNodeAdded = new OnChildNodeAddRemoveEvent();
        public OnChildNodeAddRemoveEvent OnChildNodeRemoved = new OnChildNodeAddRemoveEvent();

        public ReadOnlyCollection<BehaviourNode> Children
        {
            get
            {
                return m_children.AsReadOnly();
            }
        }

        public override void SetParent(BehaviourParentNode parent)
        {
            base.SetParent(parent);
            this.Parent?.AddChild(this);
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
                    if (Children.Count > 1)
                    {
                        throw new System.Exception("Too many children");
                    }
                    break;

            }
            if (!m_children.Contains(child))
            {
                m_children.Add(child);
                child.SetParent(this);
                OnChildNodeAdded.Invoke(this, child);
            }
        }

        public void RemoveChild(BehaviourNode child)
        {
            child.SetParent(null);
            m_children.Remove(child);
            OnChildNodeRemoved.Invoke(this, child);
        }

        public BehaviourParentNode(string name, NodeType type) : base(name, type)
        {
      
        }


        protected void ShuffleChildren()
        {
            m_children.Shuffle();
        }


        protected void StopChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].RequestStopNode();
            }
        }

        public bool UpdateRecursively()
        {
            if (State == NodeState.INACTIVE)
            {
                return false;
            }
            BehaviourParentNode nodeparent;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].State != NodeState.INACTIVE)
                {
                    nodeparent = Children[i] as BehaviourParentNode;
                    if(nodeparent != null)
                    {
                        nodeparent.UpdateRecursively();
                    }
                    else
                    {
                        Children[i].Update();
                    }
                    
                }
            }
            Update();
            return true;
        }
    }
}
