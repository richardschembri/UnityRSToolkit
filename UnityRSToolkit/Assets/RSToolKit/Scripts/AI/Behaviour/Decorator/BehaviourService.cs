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
            OnStartedSilent.AddListener(OnStartedSilent_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnChildNodeStoppedSilent.AddListener(OnChildNodeStoppedSilent_Listener);
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

        private void OnStarted_Common()
        {
            if (m_interval <= 0f)
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
        }

        /*
        public override bool StartNode(bool silent = false)
        {
            if (base.StartNode(silent))
            {
                if (m_interval <= 0f)
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
                if (!silent)
                {
                    Children[0].StartNode();
                }
                return true;
            }
            return false;
        }
        */

        private void OnStarted_Listener()
        {
            OnStarted_Common();
            // Children[0].StartNode();
            StartFirstChildNodeOnNextTick();
        }

        private void OnStartedSilent_Listener()
        {
            OnStarted_Common();
        }
        

        private void OnStopping_Listener()
        {
            // Children[0].RequestStopNode();
            Children[0].RequestStopNodeOnNextTick();
        }

        /*
        public override bool RequestStopNode(bool silent = false)
        {
            if (base.RequestStopNode(silent))
            {
                if (!silent)
                {
                    Children[0].RequestStopNode();
                }
                return true;
            }
            return false;
        }
        */

        private void OnChildNodeStopped_Common(BehaviourNode child, bool success){
            RemoveTimer(m_serviceTimer);
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
            OnChildNodeStopped_Common(child, success);
            // OnStopped.Invoke(success);
            // StopNode(success);
            StopNodeOnNextTick(success);
        }

        private void OnChildNodeStoppedSilent_Listener(BehaviourNode child, bool success)
        {
            OnChildNodeStopped_Common(child, success);
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
