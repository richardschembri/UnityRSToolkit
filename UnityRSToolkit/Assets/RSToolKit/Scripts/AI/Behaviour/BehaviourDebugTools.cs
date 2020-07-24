using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
namespace RSToolkit.AI.Behaviour
{
    public class BehaviourDebugTools
    {
        BehaviourNode m_node;
        public static readonly Color COLLAPSE_COLOR = new Color(0.4f, 0.4f, 0.4f);
        public static readonly Color COMPOSITE_COLOR = new Color(0.3f, 1f, 0.1f);
        public static readonly Color DECORATOR_COLOR = new Color(0.3f, 1f, 1f);
        public static readonly Color TASK_COLOR = new Color(0.5f, 0.1f, 0.5f);
        public static readonly Color OBSERVER_COLOR = new Color(0.9f, 0.9f, 0.6f);
        public static readonly Color ROOT_COLOR = new Color(1f, 1f, 1f, 1f);

        public float StopRequestAt { get; private set; }
        public float LastStoppedAt { get; private set; }
        public int StartCallCount { get; private set; }
        public int StopCallCount { get; private set; }
        public int StoppedCallCount { get; private set; }
        public string GUIlabel;
        private string m_GUItag;

        public int GetTotalStartCallCount()
        {
            int result = StartCallCount;
            var nodeparent = m_node as BehaviourParentNode;
            if (nodeparent != null)
            {
                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    result += nodeparent.Children[i].DebugTools.GetTotalStartCallCount();
                }
            }
            return result;
        }

        public int GetTotalStopCallCount()
        {
            int result = StopCallCount;
            var nodeparent = m_node as BehaviourParentNode;
            if (nodeparent != null)
            {
                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    result += nodeparent.Children[i].DebugTools.GetTotalStopCallCount();
                }
            }
            return result;
        }

        public int GetTotalStoppedCallCount()
        {
            int result = StoppedCallCount;
            var nodeparent = m_node as BehaviourParentNode;
            if (nodeparent != null)
            {
                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    result += nodeparent.Children[i].DebugTools.GetTotalStoppedCallCount();
                }
            }
            return result;
        }

        public int GetTotalTimers()
        {
            int result = m_node.GetAllTimerCount(); // .Timers.Count;
            var nodeparent = m_node as BehaviourParentNode;
            if (nodeparent != null)
            {
                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    result += nodeparent.Children[i].DebugTools.GetTotalTimers();
                }
            }
            return result;
        }

        public int GetTotalActiveTimers()
        {
            int result = m_node.GetAllTimerCount(true); //.Timers.Where(t => t.IsActive).Count();
            var nodeparent = m_node as BehaviourParentNode;
            if (nodeparent != null)
            {
                for (int i = 0; i < nodeparent.Children.Count; i++)
                {
                    result += nodeparent.Children[i].DebugTools.GetTotalActiveTimers();
                }
            }
            return result;
        }

        public bool GUIcollapse;
        public string GUItag
        {
            get
            {
                if (string.IsNullOrEmpty(m_GUItag))
                {
                    if (GUIcollapse)
                    {
                        var nodeparent = m_node as BehaviourParentNode;
                        return $"{m_node.Name} [{nodeparent.Children.Count}]";
                    }

                    return m_node.Name;

                }
                if (GUIcollapse)
                {
                    var nodeparent = m_node as BehaviourParentNode;
                    return $"{m_GUItag} [{nodeparent.Children.Count}]";
                }
                return m_GUItag;
            }
            set
            {
                m_GUItag = value;
            }
        }
        private Color m_nodeColor;
        public Color NodeColor
        {
            get
            {
                if (GUIcollapse)
                {
                    return COLLAPSE_COLOR;
                }
                return m_nodeColor;
            }
            private set
            {
                m_nodeColor = value;
            }
        }



        private void ResetValues()
        {
            this.StopRequestAt = 0.0f;
            this.LastStoppedAt = 0.0f;
            this.StartCallCount = 0;
            this.StopCallCount = 0;
            this.StoppedCallCount = 0;
            this.GUItag = "";
            this.GUIlabel = "";
            this.GUIcollapse = false;

        }

        public BehaviourDebugTools(BehaviourNode node, string GUItag = "")
        {
            ResetValues();
            m_node = node;

            switch (node.Type)
            {
                /*
                case BehaviourNode.NodeType.ROOT:
                    NodeColor = ROOT_COLOR;
                    break;
                    */
                case BehaviourNode.NodeType.COMPOSITE:
                    NodeColor = COMPOSITE_COLOR;
                    break;
                case BehaviourNode.NodeType.DECORATOR:
                    NodeColor = (node is Decorator.BehaviourObserver) ? OBSERVER_COLOR : DECORATOR_COLOR;
                    break;
                case BehaviourNode.NodeType.TASK:
                    NodeColor = TASK_COLOR;
                    break;
            }

            if (m_node is BehaviourRootNode)
            {
                NodeColor = ROOT_COLOR;
            }

            node.OnStarted.AddListener(OnStarted_Listener);
            node.OnStartedSilent.AddListener(OnStarted_Listener);

            node.OnStopping.AddListener(OnStopping_Listener);
            node.OnStoppingSilent.AddListener(OnStopping_Listener);

            node.OnStopped.AddListener(OnStopped_Listener);
            node.OnStoppedSilent.AddListener(OnStopped_Listener);

        }

#region Behaviour Events

        private void OnStarted_Listener()
        {
            StartCallCount++;
        }

        private void OnStopping_Listener()
        {
            StopCallCount++;
            StopRequestAt = Time.time;
        }

        private void OnStopped_Listener(bool success)
        {
            StoppedCallCount++;
            LastStoppedAt = Time.time;
        }

#endregion Behaviour Events

        public float GetTimeElapsedFromStopRequested()
        {
            return Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - StopRequestAt));
        }

        public float GetTimeElapsedFromStopped()
        {
            return Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - LastStoppedAt));
        }

        public bool HasNodeFailed()
        {
            float timeElapsed = GetTimeElapsedFromStopped();
            return (timeElapsed > 0.25f && timeElapsed < 1.0f
						&& (m_node.Result == null || !m_node.Result.Value)
						&& m_node.State == BehaviourNode.NodeState.INACTIVE);
        }

        public bool IsStopRequested()
        {
            float timeElapsed = GetTimeElapsedFromStopRequested();
            return timeElapsed > 0.25f && timeElapsed < 0.1f
					&& m_node.State == BehaviourNode.NodeState.INACTIVE;
        }

        public void ToggleCollapse()
        {
            GUIcollapse = !GUIcollapse;
        }
    }
}
#endif
