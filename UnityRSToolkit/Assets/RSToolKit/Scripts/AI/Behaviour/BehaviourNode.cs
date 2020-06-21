﻿using System.Collections;
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
            //ROOT, 
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

        public static void OverrideElapsedTime(float elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }

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
                    return Repeat == -1 || TimeOutCount <= Repeat;
                }
            }

            public bool TimeElapsed()
            {
                return BehaviourNode.ElapsedTime >= TimeOutAt;
            }

            public bool Update()
            {
                if(!IsActive)
                {
                    return false;
                }

                if(TimeElapsed())
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
        public UnityEvent OnStartedSilent { get; private set; } = new UnityEvent();
        public UnityEvent OnStopping { get; private set; } = new UnityEvent();
        public UnityEvent OnStoppingSilent { get; private set; } = new UnityEvent();
        public class OnStoppedEvent : UnityEvent<bool> { };
        public OnStoppedEvent OnStopped { get; private set; } = new OnStoppedEvent();
        public OnStoppedEvent OnStoppedSilent { get; private set; } = new OnStoppedEvent();
        /*
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
        */
        public BehaviourRootNode GetRoot()
        {
            if (this is BehaviourRootNode)
            {
                return this as BehaviourRootNode;
            }
            if (Parent == null)
            {
                return null;
            }
            else if (Parent is BehaviourRootNode)
            {
                return Parent as BehaviourRootNode;
            }
            else
            {
                return Parent.GetRoot();
            }
        }

        public virtual void SetParent(BehaviourParentNode parent)
        {
            if (parent != null)
            {
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

        /*
        public bool StartNode()
        {
            if(this.State != NodeState.INACTIVE)
            {
                return false;
            }
            this.Result = null;
            this.State = NodeState.ACTIVE;
            OnStarted.Invoke();
            return true;
        }
        */

        /// <summary>
        /// Starts the node
        /// </summary>
        /// <param name="silent">If true will not invoke the OnStarted event</param>
        /// <returns>If node successfully started</returns>
        public bool StartNode(bool silent = false)
        {
            if (this.State != NodeState.INACTIVE || (Parent != null && Parent.State != NodeState.ACTIVE))
            {
                return false;
            }
            this.Result = null;
            this.State = NodeState.ACTIVE;
            if (!silent)
            {
                OnStarted.Invoke();
            }
            else
            {
                OnStartedSilent.Invoke();
            }
            return true;
        }

        /// <summary>
        /// Initiates the stopping process
        /// </summary>
        /// <returns>If it successfully initiated the stopping process</returns>
        public bool RequestStopNode(bool silent = false)
        {
            if (this.State == NodeState.ACTIVE)
            {
                this.State = NodeState.STOPPING;
                if(!silent){
                    OnStopping.Invoke();
                }else{
                    OnStoppingSilent.Invoke();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Forces the node to be INACTIVE, bypassing the STOPPING state.
        /// This is a very dangerous method that can cause unexpected behaviour!
        /// </summary>
        /// <param name="success">If the node was successful</param>
        /// <param name="silent">If true will not invoke the OnStarted event</param>
        /// <returns></returns>
        public bool ForceStopNode(bool success, bool silent = false)
        {
            if (this.State != NodeState.INACTIVE)
            {
                if (!silent)
                {
                    OnStopped.Invoke(success);
                }
                else
                {
                    OnStoppedSilent.Invoke(success);
                }
                return true;
            }
            return false;
        }

        public BehaviourNode(string name, NodeType type)
        {
            this.Name = name;
            this.Type = type;
            OnStopped.AddListener(OnStopped_Listener);
            OnStoppedSilent.AddListener(OnStoppedSilent_Listener);
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

        private void OnStoppedSilent_Listener(bool success)
        {
            this.State = NodeState.INACTIVE;
            this.Result = success;
            Parent?.OnChildNodeStoppedSilent.Invoke(this, success);
        }

        public void UpdateTimers()
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                m_timers[i].Update();
            }
        }

        public virtual void Update()
        {
            
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
