using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    /// <summary>
    /// Run the given service function along with the child node every tick/interval.
    /// </summary>
    public class BehaviourService : BehaviourParentNode
    {
        private System.Action m_serviceAction;

        private float m_interval = -1.0f;
        private float m_randomVariation;
        private NodeTimer m_serviceTimer;

        private void IntervalInit(float interval, float? randomVariation, System.Action serviceAction)
        {
            Init(serviceAction);
            m_interval = interval;
            if(randomVariation != null)
            {
                m_randomVariation = randomVariation.Value;
            }
            else
            {
                m_randomVariation = interval * 0.05f;
            }
#if UNITY_EDITOR
            this.DebugTools.GUIlabel = "" + (interval - m_randomVariation) + "..." + (interval + m_randomVariation) + "s";
#endif
        }

        private void Init(System.Action serviceAction)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            m_serviceAction = serviceAction;
#if UNITY_EDITOR
            DebugTools.GUIlabel = "every tick";
#endif
        }

        #region Constructors
        
        /// <summary>
        /// Run the given service function, start the child node and then run the service action every tick.
        /// </summary>
        /// <param name="interval">The interval time</param>
        /// <param name="randomVariation">The interval variance</param>
        /// <param name="serviceAction">The action to run every tick</param>
        public BehaviourService(float interval, float randomVariation, System.Action serviceAction) : base("Service", NodeType.DECORATOR)
        {
            IntervalInit(interval, randomVariation, serviceAction); 
        }

        /// <summary>
        /// Run the given service function, start the child node and then run the service action every tick.
        /// </summary>
        /// <param name="interval">The interval time</param>
        /// <param name="serviceAction">The action to run per interval</param>
        public BehaviourService(float interval, System.Action serviceAction) : base("Service", NodeType.DECORATOR)
        {
            IntervalInit(interval, null, serviceAction);
            
        }

        /// <summary>
        /// Run the given service function, start the child node and then run the service action every tick.
        /// </summary>
        /// <param name="serviceAction">The action to run every tick</param>
        public BehaviourService(System.Action serviceAction) : base("Service", NodeType.DECORATOR)
        {
            Init(serviceAction);
            
        }

        #endregion Constructors

        #region Events

        private void OnStarted_Listener()
        {
            if(m_interval <= 0f)
            {
                // AddUpdateObserver
                m_serviceTimer = AddTimer(0f, 0f, -1, m_serviceAction);

            }
            else if (m_randomVariation <= 0f)
            {
                AddRandomTimer();
            }
            else
            {
                InvokeServiceActionAtRandomInterval();
            }
            Children[0].StartNode();
        }
       
        private void OnStopping_Listener()
        {
            Children[0].RequestStopNode();
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            /*
            if(m_randomVariation <= 0f || m_interval <= 0f)
            {
                RemoveTimer(m_serviceTimer);
            }
            else
            {
                m_serviceAction();
                AddRandomTimer();
            }*/
            RemoveTimer(m_serviceTimer);
            OnStopped.Invoke(success);
        }

        #endregion Events

        private void InvokeServiceActionAtRandomInterval()
        {
            m_serviceAction();
            m_serviceTimer = AddTimer(m_interval, m_randomVariation, 0, InvokeServiceActionAtRandomInterval);
        }

        private void AddRandomTimer()
        {
            m_serviceAction();
            m_serviceTimer = AddTimer(m_interval, 0f, -1, m_serviceAction);
        }

    }
}
