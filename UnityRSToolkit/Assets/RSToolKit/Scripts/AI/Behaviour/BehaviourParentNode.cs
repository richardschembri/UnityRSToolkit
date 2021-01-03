using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Helpers;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourParentNode : BehaviourNode
    {
        private List<BehaviourNode> _children = new List<BehaviourNode>();

        public ReadOnlyCollection<BehaviourNode> Children
        {
            get
            {
                return _children.AsReadOnly();
            }
        }

        #region Events
        public class OnChildNodeStoppedEvent : UnityEvent<BehaviourNode, bool> { }
        public OnChildNodeStoppedEvent OnChildNodeStopped { get; private set; } = new OnChildNodeStoppedEvent();
        public OnChildNodeStoppedEvent OnChildNodeStoppedSilent { get; private set; } = new OnChildNodeStoppedEvent();
        public class OnChildNodeAddRemoveEvent : UnityEvent<BehaviourNode, BehaviourNode> { } // parent, child
        public OnChildNodeAddRemoveEvent OnChildNodeAdded = new OnChildNodeAddRemoveEvent();
        public OnChildNodeAddRemoveEvent OnChildNodeRemoved = new OnChildNodeAddRemoveEvent();
        #endregion Events


        public BehaviourParentNode(string name, NodeType type) : base(name, type)
        {

        }

        /// <summary>
        /// Set the parent of this BehaviourNode
        /// </summary>
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
                    throw new System.Exception($"Failed to add {child.Name}. Tasks don`t have children");
                case NodeType.DECORATOR:
                    if (Children.Count > 1)
                    {
                        throw new System.Exception($"Failed to add {child.Name} to {Name}. Too many children");
                    }
                    break;

            }

            if (!_children.Contains(child))
            {
                if (child.Parent != null)
                {
                    throw new System.Exception($"Adding child to {Name} failed. {child.Name} already has parent {child.Parent.Name}");
                }
                _children.Add(child);
                child.SetParent(this);
                OnChildNodeAdded.Invoke(this, child);
            }
        }

        public void RemoveChild(BehaviourNode child)
        {
            child.SetParent(null);
            _children.Remove(child);
            OnChildNodeRemoved.Invoke(this, child);
        }

        public bool HasChild(BehaviourNode node)
        {
            return Children.Contains(node);
        }

        public bool HasChildren(IEnumerable<BehaviourNode> children)
        {
            return Children.Any(c => children.Contains(c));
        }

        public bool StartFirstChildNode()
        {
            return Children[0].StartNode();
        }

        public NodeTimer StartFirstChildNodeOnNextTick()
        {
            return RunOnNextTick(() => { StartFirstChildNode(); });
        }

        protected void ShuffleChildren()
        {
            _children.Shuffle();
        }


        protected void StopChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].RequestStopNode();
            }
        }

        protected void StopChildrenSilent()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].RequestStopNode(true);
                Children[i].StopNode(true, true);
            }
        }

        public override bool StopNode(bool success, bool silent = false)
        {
            if (silent)
            {
                StopChildrenSilent();
            }

            return base.StopNode(success, silent);
        }

        /// <summary>
        /// Update descendants and then update self (including timers)
        /// </summary>
        public virtual bool UpdateRecursively(UpdateType updateType = UpdateType.DEFAULT)
        {
            UpdateTimers(updateType);

            if (State == NodeState.INACTIVE)
            {
                return false;
            }
            BehaviourParentNode nodeparent;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].UpdateTimers(updateType);
                if (Children[i].State != NodeState.INACTIVE)
                {
                    nodeparent = Children[i] as BehaviourParentNode;
                    if (nodeparent != null)
                    {
                        nodeparent.UpdateRecursively(updateType);
                    }
                    else
                    {
                        Children[i].Update(updateType);
                    }

                }
            }
            Update(updateType);
            return true;
        }

        public int GetIndexOfChild(BehaviourNode child)
        {
            return _children.IndexOf(child);
        }

        public bool IsAncestorOfOneOrMore(BehaviourNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].IsMyAncestor(this))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<BehaviourNode> GetLeavesFromChildren(NodeState? nodeState = null)
        {
            BehaviourParentNode parentChildNode;
            BehaviourNode[] descendantLeaves;
            for (int i = 0; i < _children.Count; i++)
            {
                if(nodeState == null || _children[i].State == nodeState)
                {
                    parentChildNode = _children[i] as BehaviourParentNode;

                    if (parentChildNode != null)
                    {
                        descendantLeaves = parentChildNode.GetLeaves(nodeState);

                        if (descendantLeaves.Length <= 0)
                        {
                            //Leaf
                            yield return parentChildNode;
                        }

                        for (int j = 0; j < descendantLeaves.Length; j++)
                        {
                            yield return descendantLeaves[j];
                        }
                    }
                    else
                    {
                        // Leaf
                        yield return _children[i];
                    }
                }
            }
        }

        /// <summary>
        /// Get the deepest nodes in children
        /// </summary>
        public BehaviourNode[] GetLeaves(NodeState? nodeState = null)
        {
            var result = GetLeavesFromChildren(nodeState).ToArray();
            if(result.Length <= 0 && (nodeState == null || State == nodeState)){
                result = new BehaviourNode[] { this };
            }

            return result;
        }

        /// <summary>
        /// Remove all BehaviourNode related event listens from descendants and self
        /// </summary>
        public void RemoveAllListenersRecursively()
        {
            RemoveAllListeners();
            BehaviourParentNode nodeparent;
            for (int i = 0; i < Children.Count; i++)
            {                
                if (Children[i].State != NodeState.INACTIVE)
                {
                    nodeparent = Children[i] as BehaviourParentNode;
                    if (nodeparent != null)
                    {
                        nodeparent.RemoveAllListenersRecursively();
                    }
                    else
                    {
                        Children[i].RemoveAllListeners();
                    }
                }
            }            
        }

    }
}
