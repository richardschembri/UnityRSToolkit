using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Animation;
using RSToolkit.Space3D;

namespace RSToolkit.AI.Locomotion
{

    public abstract class BotLocomotion : MonoBehaviour
    {
        public bool DebugMode = false;
        public float MovingAwayCutoffDistance = 9f;
        public float MovingAwayDistance = 15f;

        public float SqrMovingAwayDistance {get{return MovingAwayDistance * MovingAwayDistance;}}
        public enum LocomotionState
        {
            NotMoving,
            CannotMove,
            MovingToPosition,
            MovingToTarget,
            MovingAwayFromPosition,
            MovingAwayFromTarget
        }

        public enum StopMovementConditions
        {
            NONE,
            AT_POSITION,
            WITHIN_PERSONAL_SPACE,
            WITHIN_INTERACTION_DISTANCE,   
        }

        #region FSM

        protected FiniteStateMachine<LocomotionState> m_fsm;
        protected FiniteStateMachine<LocomotionState> m_FSM
        {
            get
            {
                InitFSM();
                return m_fsm;
            }
        }

        private void InitFSM(bool force = false)
        {
            if (m_fsm == null || force)
            {
                m_fsm = FiniteStateMachine<LocomotionState>.Initialize(this, LocomotionState.NotMoving);
            }
        }

        #endregion FSM

        public LocomotionState CurrentState
        {
            get
            {
                return m_FSM.State;
            }
        }

        private bool m_fullspeed = true;
        protected StopMovementConditions m_stopMovementCondition { get; private set; }

        #region Components
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
        #endregion Components

        public bool IsFarFromGround()
        {            
            return GroundProximityCheckerComponent.IsBeyondRayDistance(GroundProximityCheckerComponent.MaxRayDistance);
        }

        public bool IsCloseToGround()
        {
            return GroundProximityCheckerComponent.IsWithinRayDistance() != null;
        }

        public bool IsAlmostGrounded()
        {
            return GroundProximityCheckerComponent.IsAlmostTouching();
        }

        public bool IsMoving(){
            return CurrentState != LocomotionState.CannotMove 
                    && CurrentState != LocomotionState.NotMoving;
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

        public bool MoveToPosition(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            if (!CanMove())
            {
                return false;
            }
            m_stopMovementCondition = stopMovementCondition;
            m_fullspeed = fullspeed;
            m_FSM.ChangeState(LocomotionState.MovingToPosition);

            return true;
        }

        public bool MoveToTarget(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            if (!CanMove())
            {
                return false;
            }
            m_stopMovementCondition = stopMovementCondition;
            m_fullspeed = fullspeed;
            m_FSM.ChangeState(LocomotionState.MovingToTarget);

            return true;
        }

        public abstract void RotateTowardsPosition();

        bool reachedDestination()
        {
            return (m_stopMovementCondition == StopMovementConditions.WITHIN_PERSONAL_SPACE && BotComponent.IsWithinPersonalSpace())
                || (m_stopMovementCondition == StopMovementConditions.WITHIN_INTERACTION_DISTANCE && BotComponent.IsWithinInteractionDistance())
                || (m_stopMovementCondition == StopMovementConditions.AT_POSITION && transform.position == BotComponent.FocusedOnPosition.Value);
        }


        public bool IsNotFocusedOrReachedDestination()
        {
            return BotComponent.FocusedOnPosition == null || reachedDestination();
        }

        public bool StopMoving()
        {
            if (CurrentState != LocomotionState.NotMoving)
            {
                m_FSM.ChangeState(LocomotionState.NotMoving);
                return true;
            }
            return false;
        }

        public virtual Vector3 GetMoveAwayDestination()
        {
            if (BotComponent.FocusedOnPosition != null)
            {
                return transform.position + (transform.position - BotComponent.FocusedOnPosition.Value).normalized * MovingAwayDistance;
            }
            return transform.position + MovingAwayDistance * -transform.forward;
        }

        protected virtual bool CanMove()
        {
            return true;
        }

        public void Animate()
        {
            CharacterAnimParams.TrySetSpeed(BotComponent.AnimatorComponent, CurrentSpeed);
        }

        #region States

        #region MovingToPosition

        protected virtual void MovingToPosition_Update()
        {
            if (!CanMove())
            {
                m_FSM.ChangeState(LocomotionState.CannotMove);
            }
            else if(IsNotFocusedOrReachedDestination())
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
        }

        #endregion MovingToPosition

        #region MovingAwayFromPosition

        #endregion MovingAwayFromPosition


        #region MovingToTarget

        protected virtual void MovingToTarget_Update()
        {
            if (!CanMove())
            {
                m_FSM.ChangeState(LocomotionState.CannotMove);
            }
            else if (IsNotFocusedOrReachedDestination())
            {
                m_FSM.ChangeState(LocomotionState.NotMoving);
            }
            else
            {
                MoveTowardsTarget(m_fullspeed);
            }
            
        }

        #endregion MovingToTarget

        #region MovingAwayFromTarget

        #endregion MovingAwayFromTarget

        #region CannotMove

        protected virtual void CannotMove_Update()
        {
            if (CanMove())
            {
                m_FSM.ChangeState(LocomotionState.NotMoving);
            }
        }

        #endregion CannotMove

        #region NotMoving

        protected virtual void NotMoving_Enter()
        {
            if (!CanMove())
            {
                m_FSM.ChangeState(LocomotionState.CannotMove);
            }
        }

        #endregion NotMoving

        #endregion States


        #region MonoBehaviour Functions
        protected virtual void Awake()
        {
            m_botComponent = GetComponent<Bot>();
        }

        protected virtual void Update()
        {
            Animate();
        }
        #endregion MonoBehaviour Functions


    }
}