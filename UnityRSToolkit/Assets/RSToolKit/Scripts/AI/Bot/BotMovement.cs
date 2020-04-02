using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.AI
{
    public abstract class BotMovement : MonoBehaviour
    {

        public enum MovementState
        {
            NotMoving,
            MovingToPosition,
            MovingToTarget
        }

        public enum StopMovementConditions
        {
            NONE,
            AT_POSITION,
            WITHIN_PERSONAL_SPACE,
            WITHIN_INTERACTION_DISTANCE,   
        }

        protected FiniteStateMachine<MovementState> m_fsm;
        protected FiniteStateMachine<MovementState> m_FSM
        {
            get
            {
                if (m_fsm == null)
                {
                    m_fsm = FiniteStateMachine<MovementState>.Initialize(this, MovementState.NotMoving);
                }
                return m_fsm;
            }
        }

        public MovementState CurrentState
        {
            get
            {
                return m_FSM.State;
            }
        }

        private bool m_fullspeed = true;
        protected StopMovementConditions m_stopMovementCondition { get; private set; }

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

        public class OnDestinationReachedEvent : UnityEvent<Vector3> { }
        public OnDestinationReachedEvent OnDestinationReached = new OnDestinationReachedEvent();

        public abstract void MoveTowardsPosition(bool fullspeed = true);
        public void MoveTowardsTarget(bool fullspeed = true)
        {
            BotComponent.FocusOnPosition(BotComponent.FocusedOnTransform.position);
            MoveTowardsPosition(fullspeed);
        }

        public void MoveToPosition(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            m_stopMovementCondition = stopMovementCondition;
            m_fullspeed = fullspeed;
            m_FSM.ChangeState(MovementState.MovingToPosition);
        }

        public void MoveToTarget(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            m_stopMovementCondition = stopMovementCondition;
            m_fullspeed = fullspeed;
            m_FSM.ChangeState(MovementState.MovingToTarget);
        }

        public abstract void RotateTowardsPosition();

        bool reachedDestination()
        {
            return (m_stopMovementCondition == StopMovementConditions.WITHIN_PERSONAL_SPACE && BotComponent.IsWithinPersonalSpace())
                || (m_stopMovementCondition == StopMovementConditions.WITHIN_INTERACTION_DISTANCE && BotComponent.IsWithinInteractionDistance())
                || (m_stopMovementCondition == StopMovementConditions.AT_POSITION && transform.position == BotComponent.FocusedOnPosition.Value);
        }

        void MovingToPosition_Update()
        {
            if(BotComponent.FocusedOnPosition == null || reachedDestination())
            {
                if(BotComponent.FocusedOnPosition != null)
                {
                    OnDestinationReached.Invoke(BotComponent.FocusedOnPosition.Value);
                }
                
                m_FSM.ChangeState(MovementState.NotMoving);
            }
            else
            {
                MoveTowardsPosition(m_fullspeed);
            }
        }

        void MovingToTarget_Update()
        {
            if (BotComponent.FocusedOnPosition == null || reachedDestination())
            {
                m_FSM.ChangeState(MovementState.NotMoving);
            }
            else
            {
                MoveTowardsTarget(m_fullspeed);
            }
        }

        public bool StopMoving()
        {
            if(CurrentState != MovementState.NotMoving)
            {
                m_FSM.ChangeState(MovementState.NotMoving);
            }
            return false;
        }

    }
}