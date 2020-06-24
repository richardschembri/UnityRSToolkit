using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    /// <summary>
    /// Run the child node immediately, but only if last execution wasn't at least past cooldownTime.
    /// </summary>
    public class BehaviourCooldown : BehaviourParentNode
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
            // OnStopping.AddListener(OnStopping_Listener);
            // OnStoppingSilent.AddListener(OnStoppingSilent_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
            OnChildNodeStoppedSilent.AddListener(OnChildNodeStoppedSilent_Listener);

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

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="randomVariation">The random variation to the cooldown time</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails.</param>
        /// <param name="failOnCooldown">If true, fail instead of wait in case the cooldown is still active</param>
        public BehaviourCooldown(float cooldownTime, float randomVariation, bool startAfterChild, bool resetOnFailiure, bool failOnCooldown) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, cooldownTime, startAfterChild, resetOnFailiure, failOnCooldown);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="randomVariation">The random variation to the cooldown time</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails.</param>
        /// <param name="failOnCooldown">If true, fail instead of wait in case the cooldown is still active</param>
        public BehaviourCooldown(float cooldownTime, BehaviourNode decoratee, float randomVariation, bool startAfterChild, bool resetOnFailiure, bool failOnCooldown) : base("Cooldown", NodeType.DECORATOR)
        {
            AddChild(decoratee);
            Init(cooldownTime, cooldownTime, startAfterChild, resetOnFailiure, failOnCooldown);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails.</param>
        /// <param name="failOnCooldown">If true, fail instead of wait in case the cooldown is still active</param>
        public BehaviourCooldown(float cooldownTime, bool startAfterChild, bool resetOnFailiure, bool failOnCooldown) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, null, startAfterChild, resetOnFailiure, failOnCooldown);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails</param>
        /// <param name="failOnCooldown">If true, fail instead of wait in case the cooldown is still active</param>
        public BehaviourCooldown(float cooldownTime, BehaviourNode decoratee, bool startAfterChild, bool resetOnFailiure, bool failOnCooldown) : base("Cooldown", NodeType.DECORATOR)
        {
            AddChild(decoratee);
            Init(cooldownTime, null, startAfterChild, resetOnFailiure, failOnCooldown);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="randomVariation">The random variation to the cooldown time</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails</param>
        public BehaviourCooldown(float cooldownTime, float randomVariation, bool startAfterChild, bool resetOnFailiure) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, randomVariation, startAfterChild, resetOnFailiure);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="randomVariation">The random variation to the cooldown time</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails</param>
        public BehaviourCooldown(float cooldownTime, BehaviourNode decoratee, float randomVariation, bool startAfterChild, bool resetOnFailiure) : base("Cooldown", NodeType.DECORATOR)
        {
            AddChild(decoratee);
            Init(cooldownTime, randomVariation, startAfterChild, resetOnFailiure);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails</param>
        public BehaviourCooldown(float cooldownTime, bool startAfterChild, bool resetOnFailiure) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, null, startAfterChild, resetOnFailiure);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="startAfterChild">Start the cooldown after the child node stops</param>
        /// <param name="resetOnFailiure">If true, the cooldown will be reset if the child node fails</param>
        public BehaviourCooldown(float cooldownTime, BehaviourNode decoratee, bool startAfterChild, bool resetOnFailiure) : base("Cooldown", NodeType.DECORATOR)
        {
            AddChild(decoratee);
            Init(cooldownTime, null, startAfterChild, resetOnFailiure);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="randomVariation">The random variation to the cooldown time</param>
        public BehaviourCooldown(float cooldownTime, float randomVariation) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime, randomVariation);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="decoratee">The child node</param>
        /// <param name="randomVariation">The random variation to the cooldown time</param>
        public BehaviourCooldown(float cooldownTime, BehaviourNode decoratee, float randomVariation) : base("Cooldown", NodeType.DECORATOR)
        {
            AddChild(decoratee);
            Init(cooldownTime, randomVariation);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        public BehaviourCooldown(float cooldownTime) : base("Cooldown", NodeType.DECORATOR)
        {
            Init(cooldownTime);
        }

        /// <summary>
        /// Run child node immediately, but only if last execution wasn't at least past cooldownTime.
        /// </summary>
        /// <param name="cooldownTime">The cooldown time</param>
        /// <param name="decoratee">The child node</param>
        public BehaviourCooldown(float cooldownTime, BehaviourNode decoratee) : base("Cooldown", NodeType.DECORATOR)
        {
            AddChild(decoratee);
            Init(cooldownTime);
        }

        #endregion Constructors

        #region Events

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
                    // OnStopped.Invoke(false);
                    StopNode(false);
                }
            }
        }

        /*
        private void OnStopping_Common()
        {
            m_isReady = true;
            RemoveTimer(m_timeoutTimer);
        }

        private void OnStopping_Listener()
        {
            OnStopping_Common();
            if (Children[0].State == NodeState.ACTIVE)
            {
                Children[0].RequestStopNode();
            }
            else
            {
                // OnStopped.Invoke(false);
                StopNode(false);
            }
        }

        private void OnStoppingSilent_Listener(){
            OnStopping_Common();
        }
        */


        public override bool RequestStopNode(bool silent = false)
        {
            if (base.RequestStopNode(silent))
            {
                m_isReady = true;
                RemoveTimer(m_timeoutTimer);
                if (!silent)
                {
                    if (Children[0].State == NodeState.ACTIVE)
                    {
                        Children[0].RequestStopNode();
                    }
                    else
                    {
                        // OnStopped.Invoke(false);
                        StopNode(false);
                    }
                }
                return true;
            }
            return false;
        }

        /*
        private void OnChildNodeStopped_Common(BehaviourNode child, bool success)
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
        }
        */

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            //OnChildNodeStopped_Common(child, success);
            // OnStopped.Invoke(success);
            // StopNode(success);
            RunOnNextTick(ProcessChildStopped);
        }

        private void OnChildNodeStoppedSilent_Listener(BehaviourNode child, bool success)
        {
            // OnChildNodeStopped_Common(child, success);
            RunOnNextTick(ProcessChildStoppedSilent);
        }

        private void ProcessChildStoppedSilent()
        {
            if (m_resetOnFailiure && !Children[0].Result.Value)
            {
                m_isReady = true;
                RemoveTimer(m_timeoutTimer);
            }
            else if (m_startAfterChild)
            {
                AddTimer(m_cooldownTime, m_randomVariation, 0, OnTimeout);
            }
        }

        private void ProcessChildStopped()
        {
            ProcessChildStoppedSilent();
            StopNode(Children[0].Result.Value);
        }

        #endregion Events

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
    }
}
