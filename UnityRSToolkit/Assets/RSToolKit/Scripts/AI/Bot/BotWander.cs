using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;
namespace RSToolkit.AI
{
    [RequireComponent(typeof(Bot))]
    public abstract class BotWander : MonoBehaviour
    {
        public enum WanderStates
        {
            NotWandering,
            FindNewPosition,
            MovingToPosition,
            CannotWander
        }

        // public bool WanderOnAwake = false;
        public float waitTime = 0f;
        public bool randomizeWait = false;
        public bool debugMode = false;
        public float movementTimeout = 5f;

        private float GetWaitTime()
        {
            if (randomizeWait)
            {
                return RandomHelpers.RandomFloatWithinRange(0f, waitTime);
            }
            return waitTime;
        }

        protected FiniteStateMachine<WanderStates> m_fsm;

        public WanderStates CurrentState
        {
            get
            {
                return m_fsm.State;
            }
        }

        public Vector3? WanderPosition { get; private set; } = null;
        public float defaultWanderRadius = 20f;
        public float m_wanderRadius = 20f;

        private Bot m_botComponent;
        public Bot BotComponent
        {
            get
            {
                if (m_botComponent == null)
                {
                    m_botComponent = GetComponent<Bot>();
                }
                return m_botComponent;
            }

        }

        public abstract bool CanWander();
       

        IEnumerator FindNewPosition_Enter()
        {
            yield return new WaitForSeconds(GetWaitTime());
            BotComponent.UnFocus();
            WanderPosition = GetNewWanderPosition(m_wanderRadius);
            BotComponent.FocusOnPosition(WanderPosition.Value);
            m_fsm.ChangeState(WanderStates.MovingToPosition);
        }

        void MovingToPosition_Update()
        {
            if (!CanWander())
            {
                m_fsm.ChangeState(WanderStates.CannotWander, FiniteStateTransition.Overwrite);
            }
            else if (!BotComponent.IsWithinPersonalSpace())
            {
                MoveTowardsWanderPosition();
            }
            else
            {
                m_fsm.ChangeState(WanderStates.FindNewPosition);
            }
            
        }

        IEnumerator m_movingToPosition_TimeOut;
        void MovingToPosition_Enter()
        {
            if (movementTimeout > 0)
            {
                m_movingToPosition_TimeOut = MovingToPosition_TimeOut();
                StartCoroutine(m_movingToPosition_TimeOut);
            }
        }

        void MovingToPosition_Exit()
        {
            StopCoroutine(m_movingToPosition_TimeOut);
        }

        IEnumerator MovingToPosition_TimeOut()
        {
            yield return new WaitForSeconds(movementTimeout);
            if (m_fsm.State == WanderStates.MovingToPosition)
            {
                Debug.Log("Movement timeout!");
                m_fsm.ChangeState(WanderStates.FindNewPosition);
            }
        }

        void CannotWander_Update()
        {
            if (CanWander())
            {
                m_fsm.ChangeState(WanderStates.FindNewPosition);
            }
        }

        public virtual bool Wander(float radius)
        {
            m_wanderRadius = radius;
            
            if (CurrentState == WanderStates.NotWandering)
            {
                m_fsm.ChangeState(WanderStates.FindNewPosition);
                return true;
            }

            return false;
        }

        public bool Wander()
        {
            return Wander(defaultWanderRadius);
        }

        public virtual bool StopWandering()
        {
            if (CurrentState != WanderStates.NotWandering)
            {
                m_fsm.ChangeState(WanderStates.NotWandering, FiniteStateTransition.Overwrite);
                return true;
            }

            return false;
        }

        protected abstract void MoveTowardsWanderPosition();

        protected abstract Vector3 GetNewWanderPosition(float radius);

        protected virtual void Awake()
        {
            m_fsm = FiniteStateMachine<WanderStates>.Initialize(this,WanderStates.NotWandering);
            if (debugMode)
            {
                m_fsm.Changed += Fsm_Changed;
            }    
        }

        private void Fsm_Changed(WanderStates state)
        {
            try
            {
                Debug.Log($"{transform.name} WanderStates changed from {m_fsm.LastState.ToString()} to {state.ToString()}");
            }
            catch(System.Exception ex)
            {
                Debug.Log($"{transform.name} WanderStates changed to {state.ToString()}");
            }
            
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(0f, 0f, 0.75f, .075f);

            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, defaultWanderRadius);

            if(WanderPosition != null)
            {
                UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
                UnityEditor.Handles.DrawSolidDisc(WanderPosition.Value, Vector3.up, 0.25f);
            }
            
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}