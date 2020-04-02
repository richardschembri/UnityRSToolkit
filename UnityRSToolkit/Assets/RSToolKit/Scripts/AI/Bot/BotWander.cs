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
            if(m_FSM.LastState == WanderStates.NotWandering || m_FSM.LastState == WanderStates.CannotWander)
            {
                return 0.1f;
            }

            if (randomizeWait)
            {
                return RandomHelpers.RandomFloatWithinRange(0f, waitTime);
            }
            return waitTime;
        }

        private FiniteStateMachine<WanderStates> m_fsm;
        protected FiniteStateMachine<WanderStates> m_FSM
        {
            get
            {
                if (m_fsm == null)
                {
                    m_fsm = FiniteStateMachine<WanderStates>.Initialize(this, WanderStates.NotWandering);
                }
                return m_fsm;
            }
        }

        public WanderStates CurrentState
        {
            get
            {
                return m_FSM.State;
            }
        }

        // public Vector3? WanderPosition { get; private set; } = null;
        public float defaultWanderRadius = 20f;
        private float m_wanderRadius = 20f;

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
            
            BotComponent.UnFocus();
            yield return new WaitForSeconds(GetWaitTime());
            BotComponent.FocusOnPosition(GetNewWanderPosition(m_wanderRadius));
            m_FSM.ChangeState(WanderStates.MovingToPosition);
        }

        void MovingToPosition_Update()
        {
            if (!CanWander())
            {
                m_FSM.ChangeState(WanderStates.CannotWander, FiniteStateTransition.Overwrite);
            }
            else if (BotComponent.GetMovementState() == BotMovement.MovementState.NotMoving)
            {
                m_FSM.ChangeState(WanderStates.FindNewPosition);
            }
            
        }

        IEnumerator m_movingToPosition_TimeOut;
        void MovingToPosition_Enter()
        {
            if (movementTimeout > 0)
            {
                m_movingToPosition_TimeOut = MovingToPosition_TimeOut();
                BotComponent.MoveToPosition(BotMovement.StopMovementConditions.WITHIN_PERSONAL_SPACE, false);
                if (debugMode)
                {
                    Debug.Log($"Wandering to {BotComponent.FocusedOnPosition.ToString()}");
                }
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
            if (m_FSM.State == WanderStates.MovingToPosition)
            {
                Debug.Log("Movement timeout!");
                m_FSM.ChangeState(WanderStates.FindNewPosition);
            }
        }

        void CannotWander_Update()
        {
            if (CanWander())
            {
                m_FSM.ChangeState(WanderStates.FindNewPosition);
            }
        }

        public bool Wander(float radius)
        {
            m_wanderRadius = radius;

            if (!CanWander())
            {
                m_FSM.ChangeState(WanderStates.CannotWander);
            }
            else if (CurrentState == WanderStates.NotWandering)
            {
                m_FSM.ChangeState(WanderStates.FindNewPosition);
                return true;
            }

            return false;
        }

        public bool Wander()
        {
            return Wander(defaultWanderRadius);
        }

        public bool StopWandering(bool stopMoving = false)
        {
            if (CurrentState != WanderStates.NotWandering)
            {
                if (stopMoving)
                {
                    BotComponent.StopMoving();
                }            
                m_FSM.ChangeState(WanderStates.NotWandering, FiniteStateTransition.Overwrite);
                return true;
            }

            return false;
        }

        protected abstract Vector3 GetNewWanderPosition(float radius);

        protected virtual void Awake()
        {
            if (debugMode)
            {
                m_FSM.Changed += Fsm_Changed;
            }    
        }

        private void Fsm_Changed(WanderStates state)
        {
            try
            {
                Debug.Log($"{transform.name} WanderStates changed from {m_FSM.LastState.ToString()} to {state.ToString()}");
            }
            catch(System.Exception ex)
            {
                Debug.Log($"{transform.name} WanderStates changed to {state.ToString()}");
            }
            
        }

        public bool IsWandering()
        {
            return CurrentState != BotWander.WanderStates.NotWandering
                    && CurrentState != BotWander.WanderStates.CannotWander;
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(0f, 0f, 0.75f, .075f);

            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, defaultWanderRadius);
            
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}