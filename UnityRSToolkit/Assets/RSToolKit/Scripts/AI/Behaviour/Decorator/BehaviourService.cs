using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour.Decorator
{
    public class BehaviourService : BehaviourNode
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
        }

        private void Init(System.Action serviceAction)
        {
            OnStarted.AddListener(OnStarted_Listener);
            OnStopped.AddListener(OnStopped_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            m_serviceAction = serviceAction;
        }
        public BehaviourService(float interval, float randomVariation, System.Action serviceAction) : base("Service", NodeType.DECORATOR)
        {
            IntervalInit(interval, randomVariation, serviceAction); 
        }
        public BehaviourService(float interval, System.Action serviceAction) : base("Service", NodeType.DECORATOR)
        {
            IntervalInit(interval, null, serviceAction); 
        }
        public BehaviourService(System.Action serviceAction) : base("Service", NodeType.DECORATOR)
        {
            Init(serviceAction);
        }
        private void InvokeServiceActionAtRandomInterval()
        {
            m_serviceAction();
            m_serviceTimer = AddTimer(m_interval, m_randomVariation, 0, InvokeServiceActionAtRandomInterval);
        }
        private void AddRandomTimer()
        {
            m_serviceTimer = AddTimer(m_interval,0f, -1, m_serviceAction); m_serviceAction();
        }
        private void OnStarted_Listener()
        {
            if(m_interval <= 0f)
            {
                // AddUpdateObserver
                //m_serviceAction();
                // using update
            }else if (m_randomVariation <= 0f)
            {
                AddRandomTimer();
            }
            else
            {
                InvokeServiceActionAtRandomInterval();
            }
            Children[0].StartNode();
        }
        public override void Update()
        {
            base.Update();
            m_serviceAction();

        }
        private void OnStopped_Listener(bool success)
        {
            Children[0].RequestStopNode();
        }
        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(m_interval <= 0f)
            {
                // RemoveUpdateObserver
            }
            else if(m_randomVariation <= 0f)
            {
                RemoveTimer(m_serviceTimer);
            }
            else
            {
                m_serviceAction();
                AddRandomTimer();
            }
        }
    }
}
