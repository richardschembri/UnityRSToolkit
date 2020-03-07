using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourCooldown : BehaviourNode
    {
        private bool m_startAfterChild = false;
        private bool m_resetOnFailiure = false;
        private bool m_failOnCooldown = false;
        private float m_cooldownTime = 0.0f;
        private float m_randomVariation = 0.05f;
        private bool m_isReady = true;
        NodeTimer m_timeoutTimer;
        private void Init(float cooldownTime, float? randomVariation = null, bool startAfterChild = false, bool resetOnFailiure = false, bool failOnCooldown = false)
        {

            OnStarted.AddListener(OnStarted_Listener);
            OnStopping.AddListener(OnStopping_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);

            m_startAfterChild = false;
            m_cooldownTime = cooldownTime;
            m_resetOnFailiure = false;
            if(randomVariation != null)
            {
                m_randomVariation = randomVariation.Value;
            }
            else
            {
                m_randomVariation = cooldownTime * 0.1f;
            }
        }

        #region Constructors
        public BehaviourCooldown(float cooldownTime, float randomVariation, bool startAfterChild, bool resetOnFailiure, bool failOnCooldown) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, cooldownTime, startAfterChild, resetOnFailiure, failOnCooldown);
        }

        public BehaviourCooldown(float cooldownTime, bool startAfterChild, bool resetOnFailiure, bool failOnCooldown) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, null, startAfterChild, resetOnFailiure, failOnCooldown);
        }

        public BehaviourCooldown(float cooldownTime, float randomVariation, bool startAfterChild, bool resetOnFailiure) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, randomVariation, startAfterChild, resetOnFailiure);
        }

        public BehaviourCooldown(float cooldownTime, bool startAfterChild, bool resetOnFailiure) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, null, startAfterChild, resetOnFailiure);
        }

        public BehaviourCooldown(float cooldownTime, float randomVariation) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, randomVariation);
        }

        public BehaviourCooldown(float cooldownTime) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime);
        }
        #endregion Constructors

        private void OnTimeout()
        {
            if (State == NodeState.ACTIVE && Children[0].State != NodeState.ACTIVE)
            {
                AddTimer(m_cooldownTime, m_randomVariation, 0, OnTimeout);
                Children[0].StartNode();
            }
            else
            {
                m_isReady = true;
            }
        }

        private void OnStarted_Listener()
        {
            if (m_isReady)
            {
                m_isReady = false;
                if (!m_startAfterChild)
                {
                    m_timeoutTimer = AddTimer(m_cooldownTime, m_randomVariation, 0, OnTimeout);
                }
                Children[0].StartNode();
            }
            else
            {
                if (m_failOnCooldown)
                {
                    OnStopped.Invoke(false);
                }
            }
        }

        private void OnStopping_Listener()
        {
            m_isReady = true;
            RemoveTimer(m_timeoutTimer);
            if (Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                OnStopped.Invoke(false);
            }
        }
        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if (m_resetOnFailiure && !success)
            {
                m_isReady = true;
                RemoveTimer(m_timeoutTimer);
            }
            else if (m_startAfterChild)
            {
                AddTimer(m_cooldownTime, m_randomVariation, 0, OnTimeout);
            }
            OnStopped.Invoke(success);
        }
    }
}
