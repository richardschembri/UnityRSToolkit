using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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

        public enum UpdateType
        {
            DEFAULT,
            FIXED,
            LATE
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

        public class NodeTimer
        {
            public double TimeOutIn { get; private set; }

            public double TimeOutAt { get; set; } = 0f;
            public int Repeat { get; set; } = 0;
            public int TimeOutCount { get; private set; } = 0;

            public bool AutoRemove { get; private set; } = false;
            private System.Action TimeoutAction { get; set; }

            public NodeTimer(float time, float randomVariance, int repeat, System.Action timeoutAction, bool autoRemove = false )
            {
                AutoRemove = autoRemove;
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

            public bool IsFinished {
                get {
                    return Repeat > -1 && TimeOutCount > Repeat;
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
        public string Name { get; set; }
        public NodeType Type { get; private set; }
        public BehaviourParentNode Parent { get; private set; }

        private Dictionary<UpdateType, List<NodeTimer>> _timers = new Dictionary<UpdateType, List<NodeTimer>>();
      
        public ReadOnlyCollection<NodeTimer> GetTimers(UpdateType updateType)
        {
            return _timers[updateType].AsReadOnly();           
        }

        public int GetTimerCount(UpdateType updateType)
        {
            return _timers[updateType].Count;
        }

        public int GetAllTimerCount()
        {
            return _timers[UpdateType.DEFAULT].Count + _timers[UpdateType.FIXED].Count + _timers[UpdateType.LATE].Count;
        }

        public int GetAllTimerCount(bool active)
        {
            return _timers[UpdateType.DEFAULT].Count(t => t.IsActive == active) + _timers[UpdateType.FIXED].Count(t => t.IsActive == active) + _timers[UpdateType.LATE].Count(t => t.IsActive == active);
        }

        string _uniqueID = "";
        public string GetUniqueID(bool refresh = false)
        {
            if(!string.IsNullOrEmpty(_uniqueID) && !refresh)
            {
                return _uniqueID;
            }

            if (Parent != null)
            {
                _uniqueID = $"{Parent.GetUniqueID()}-{Parent.GetIndexOfChild(this)}";
                return _uniqueID;
            }

            _uniqueID = "0";
            return _uniqueID;
        }

        public string GetNamePath()
        {
            if (Parent != null)
            {
                return $"{Parent.GetNamePath()} -> {Name}";
            }

            return Name;
        }

        public UnityEvent OnStarted { get; private set; } = new UnityEvent();
        public UnityEvent OnStartedSilent { get; private set; } = new UnityEvent();
        public UnityEvent OnStopping { get; private set; } = new UnityEvent();
        public UnityEvent OnStoppingSilent { get; private set; } = new UnityEvent();
        public class OnStoppedEvent : UnityEvent<bool> { };
        public OnStoppedEvent OnStopped { get; private set; } = new OnStoppedEvent();
        public OnStoppedEvent OnStoppedSilent { get; private set; } = new OnStoppedEvent();        

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

        protected NodeTimer AddTimer(float time, float randomVariance, int repeat, System.Action timeoutAction, bool autoRemove = false, UpdateType updateType = UpdateType.DEFAULT)
        {
            var new_timer = new NodeTimer(time, randomVariance, repeat, timeoutAction, autoRemove);
            //m_timers.Add(new_timer);
            _timers[updateType].Add(new_timer);
            return new_timer;
        }

        protected NodeTimer AddTimer(float time, int repeat, System.Action timeoutAction, bool autoRemove = false, UpdateType updateType = UpdateType.DEFAULT)
        {
            return AddTimer(time, 0f, repeat, timeoutAction, autoRemove);
        }

        protected NodeTimer RunOnNextTick(System.Action timeoutAction, UpdateType updateType = UpdateType.DEFAULT)
        {
            return AddTimer(0, 0, timeoutAction, true);
        }

        protected bool HasTimer(NodeTimer timer, UpdateType updateType = UpdateType.DEFAULT)
        {
            return _timers[updateType].Contains(timer); //m_timers.Contains(timer);
        }

        protected void RemoveTimer(NodeTimer to_remove, UpdateType updateType = UpdateType.DEFAULT)
        {
            _timers[updateType].Remove(to_remove); //m_timers.Remove(to_remove);
        }

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

        public bool StartNodePath(bool silent = true)
        {
            if(Parent != null && Parent.State != NodeState.ACTIVE)
            {
                if(Parent.State == NodeState.STOPPING)
                {
                    if (!Parent.StopNode(true))
                    {
                        return false;
                    }
                }
                if (!Parent.StartNodePath(true))
                {
                    return false;
                }
            }

            return StartNode(silent);
        }

        public NodeTimer StartNodeOnNextTick(bool silent = false)
        {
            return RunOnNextTick(()=> { StartNode(silent); });
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

        public NodeTimer RequestStopNodeOnNextTick(bool silent = false)
        {
            return RunOnNextTick(() => { RequestStopNode(silent); });
        }

        /// <summary>
        /// Forces the node to be INACTIVE, bypassing the STOPPING state.
        /// This is a very dangerous method that can cause unexpected behaviour!
        /// </summary>
        /// <param name="success">If the node was successful</param>
        /// <param name="silent">If true will not invoke the OnStarted event</param>
        /// <returns></returns>
        public virtual bool StopNode(bool success, bool silent = false)
        {
            if (this.State != NodeState.INACTIVE)
            {
                this.State = NodeState.INACTIVE;
                this.Result = success;
                if (!silent)
                {
                    OnStopped.Invoke(success);
                    Parent?.OnChildNodeStopped.Invoke(this, success);
                }
                else
                {
                    OnStoppedSilent.Invoke(success);
                    Parent?.OnChildNodeStoppedSilent.Invoke(this, success);
                }
                return true;
            }
            return false;
        }

        public NodeTimer StopNodeOnNextTick(bool success, bool silent = false)
        {
            return RunOnNextTick(() => { StopNode(success, silent); });
        }

        public bool IsMyAncestor(BehaviourNode node)
        {
            if(this.Parent == null)
            {
                return false;
            }else if(node == this.Parent)
            {
                return true;
            }

            return this.Parent.IsMyAncestor(node);
        }


        public BehaviourNode(string name, NodeType type)
        {
            this.Name = name;
            this.Type = type;
            _timers.Add(UpdateType.DEFAULT, new List<NodeTimer>());
            _timers.Add(UpdateType.FIXED, new List<NodeTimer>());
            _timers.Add(UpdateType.LATE, new List<NodeTimer>());
#if UNITY_EDITOR
            InitDebugTools();
#endif
        }

        int m_timerCount;

        public void UpdateTimers(UpdateType updateType = UpdateType.DEFAULT)
        {
            m_timerCount = _timers[updateType].Count;
            for (int i = 0; i < _timers[updateType].Count; i++)
            {
                if (_timers[updateType][i].IsFinished && _timers[updateType][i].AutoRemove)
                {
                    _timers[updateType].Remove(_timers[updateType][i]);
                    i--;
                    m_timerCount--;
                }
                else
                {
                    _timers[updateType][i].Update();
                }
            }
        }

        public virtual void Update(UpdateType updateType = UpdateType.DEFAULT)
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
