using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Animation;
using RSToolkit.Space3D;

namespace RSToolkit.AI
{

    public abstract class BotLocomotion : MonoBehaviour
    {

        public enum LocomotionState
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

        protected FiniteStateMachine<LocomotionState> m_fsm;
        protected FiniteStateMachine<LocomotionState> m_FSM
        {
            get
            {
                if (m_fsm == null)
                {
                    m_fsm = FiniteStateMachine<LocomotionState>.Initialize(this, LocomotionState.NotMoving);
                }
                return m_fsm;
            }
        }

        public LocomotionState CurrentState
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

        private ProximityChecker m_proximityCheckerComponent;
        public ProximityChecker GroundProximityCheckerComponent
        {
            get
            {
                if (m_proximityCheckerComponent == null)
                {
                    m_proximityCheckerComponent = GetComponent<ProximityChecker>();
                }

                return m_proximityCheckerComponent;
            }

        }

        public bool IsFarFromGround()
        {            
            return GroundProximityCheckerComponent.IsBeyondRayDistance(GroundProximityCheckerComponent.RayDistance);
        }

        public bool IsCloseToGround()
        {
            return GroundProximityCheckerComponent.IsWithinRayDistance() != null;
        }

        public bool IsGrounded()
        {
            return GroundProximityCheckerComponent.IsTouching();
        }

        public class OnDestinationReachedEvent : UnityEvent<Vector3> { }
        public OnDestinationReachedEvent OnDestinationReached = new OnDestinationReachedEvent();

        public abstract float CurrentSpeed { get; }

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
            m_FSM.ChangeState(LocomotionState.MovingToPosition);
        }

        public void MoveToTarget(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            m_stopMovementCondition = stopMovementCondition;
            m_fullspeed = fullspeed;
            m_FSM.ChangeState(LocomotionState.MovingToTarget);
        }

        public abstract void RotateTowardsPosition();

        bool reachedDestination()
        {
            return (m_stopMovementCondition == StopMovementConditions.WITHIN_PERSONAL_SPACE && BotComponent.IsWithinPersonalSpace())
                || (m_stopMovementCondition == StopMovementConditions.WITHIN_INTERACTION_DISTANCE && BotComponent.IsWithinInteractionDistance())
                || (m_stopMovementCondition == StopMovementConditions.AT_POSITION && transform.position == BotComponent.FocusedOnPosition.Value);
        }

        protected virtual void MovingToPosition_Update()
        {
            if(BotComponent.FocusedOnPosition == null || reachedDestination())
            {
                if(BotComponent.FocusedOnPosition != null)
                {
                    OnDestinationReached.Invoke(BotComponent.FocusedOnPosition.Value);
                }
                
                m_FSM.ChangeState(LocomotionState.NotMoving);
            }
            else
            {
                MoveTowardsPosition(m_fullspeed);
            }
            CharacterAnimParams.TrySetSpeed(BotComponent.AnimatorComponent, CurrentSpeed);
        }

        protected virtual void MovingToTarget_Update()
        {
            if (BotComponent.FocusedOnPosition == null || reachedDestination())
            {
                m_FSM.ChangeState(LocomotionState.NotMoving);
            }
            else
            {
                MoveTowardsTarget(m_fullspeed);
            }
        }

        public bool StopMoving()
        {
            if(CurrentState != LocomotionState.NotMoving)
            {
                m_FSM.ChangeState(LocomotionState.NotMoving);
            }
            return false;
        }

    }
}