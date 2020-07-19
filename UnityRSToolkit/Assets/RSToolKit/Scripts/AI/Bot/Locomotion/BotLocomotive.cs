using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.FSM;
using RSToolkit.Space3D;
using UnityEngine.Events;
using RSToolkit.Helpers;
using RSToolkit.Animation;
using UnityEngine.AI;
using System.Linq;

namespace RSToolkit.AI.Locomotion
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BTFiniteStateMachineManager))]
    public abstract class BotLocomotive : Bot
    {
        public enum LocomotionStates
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
      
        public BotLogicLocomotion CurrentLocomotionType { get; set; }

        public BTFiniteStateMachine<LocomotionStates> FSM { get; private set; } = new BTFiniteStateMachine<LocomotionStates>(LocomotionStates.NotMoving);

        public LocomotionStates CurrentState
        {
            get
            {
                return FSM.CurrentState;
            }
        }
        public float CurrentSpeed { get { return CurrentLocomotionType != null ? CurrentLocomotionType .CurrentSpeed : 0; } }

        private bool _fullspeed = true;
        public StopMovementConditions StopMovementCondition { get; private set; }

        public float MovingAwayDistance
        {
            get
            {
                return SqrAwarenessMagnitude * 1.1f;
            }
        }


        public override void ToggleComponentsForNetwork(bool owner)
        {
            base.ToggleComponentsForNetwork(owner);
            if (!owner)
            {
                //_botWanderManagerComponent.enabled = false;
                GroundProximityCheckerComponent.enabled = false;
            }
            else
            {
                GroundProximityCheckerComponent.enabled = true;
                //_botWanderManagerComponent.enabled = true;
            }
        }
        #region Components
        private ProximityChecker _proximityCheckerComponent;
        public ProximityChecker GroundProximityCheckerComponent
        {
            get
            {
                if (_proximityCheckerComponent == null)
                {
                    _proximityCheckerComponent = GetComponent<ProximityChecker>();
                }

                return _proximityCheckerComponent;
            }

        }

        public BotWanderManager BotWanderManagerComponent{ get; private set;}


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

        public bool IsMoving()
        {
            return CurrentState != LocomotionStates.CannotMove
                    && CurrentState != LocomotionStates.NotMoving;
        }

        public void AnimateLocomotion()
        {
            CharacterAnimParams.TrySetSpeed(AnimatorComponent, CurrentSpeed);
        }

        public bool IsAway()
        {
            if (FocusedOnTransform != null)
            {
                return !ProximityHelpers.IsWithinDistance(ColliderComponent, FocusedOnTransform.position, SqrAwarenessMagnitude);
            }
            return !ProximityHelpers.IsWithinDistance(ColliderComponent, FocusedOnPosition.Value, SqrAwarenessMagnitude);
        }

        protected bool IsNotFocusedOrIsAway()
        {
            return FocusedOnPosition == null || IsAway();
        }

        public bool IsNotFocusedOrReachedDestination()
        {
            return FocusedOnPosition == null || reachedDestination();
        }

        public float? IsCloseToSurface()
        {
            return GroundProximityCheckerComponent.IsWithinRayDistance();
        }

        public float? IsCloseToNavMeshSurface()
        {
            NavMeshHit hit;
            return IsCloseToNavMeshSurface(out hit);
        }

        public float? IsCloseToNavMeshSurface(out NavMeshHit hit)
        {
            return GroundProximityCheckerComponent.IsWithinRayDistance(out hit);
        }

        public class OnDestinationReachedEvent : UnityEvent<Vector3> { }
        public OnDestinationReachedEvent OnDestinationReached = new OnDestinationReachedEvent();

        public void RotateTowardsPosition()
        {
            CurrentLocomotionType.RotateTowardsPosition();
        }

        public void MoveTowardsPosition(bool fullspeed = true)
        {
            CurrentLocomotionType.MoveTowardsPosition(fullspeed);
        }

        public bool MoveToTetherPoint(BotLocomotive.StopMovementConditions stopMovementCondition = BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, bool fullspeed = true)
        {
            FocusOnTransform(TetherToTransform);
            return MoveToTarget(stopMovementCondition, fullspeed);
        }

        public void MoveTowardsTarget(bool fullspeed = true)
        {
            FocusOnPosition(FocusedOnTransform.position);
            try
            {
                CurrentLocomotionType.MoveTowardsPosition(fullspeed);
            }
            catch (System.Exception ex)
            {

                if (DebugMode)
                {
                    Debug.LogError($"Locomotion Error: {ex.Message}");
                }

                FSM.ChangeState(LocomotionStates.CannotMove);

            }
        }
            #region Move

            private bool MoveCommon(LocomotionStates moveType, bool fullspeed = true, StopMovementConditions stopMovementCondition = StopMovementConditions.NONE)
        {
            if (!CurrentLocomotionType.CanMove())
            {
                return false;
            }
            StopMovementCondition = stopMovementCondition;
            _fullspeed = fullspeed;
            FSM.ChangeState(moveType);
            return true;
        }

        public bool MoveToPosition(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            return MoveCommon(LocomotionStates.MovingToPosition, _fullspeed, stopMovementCondition);
        }

        public bool MoveAwayFromPosition(bool fullspeed = true)
        {
            return MoveCommon(LocomotionStates.MovingAwayFromPosition, _fullspeed);
        }

        public bool MoveToTarget(StopMovementConditions stopMovementCondition, bool fullspeed = true)
        {
            return MoveCommon(LocomotionStates.MovingToTarget, _fullspeed, stopMovementCondition);
        }

        public bool MoveAwayFromTarget(bool fullspeed = true)
        {
            return MoveCommon(LocomotionStates.MovingAwayFromTarget, _fullspeed);
        }

        public bool StopMoving()
        {
            if (CurrentState != LocomotionStates.NotMoving)
            {
                FSM.ChangeState(LocomotionStates.NotMoving);
                return true;
            }
            return false;
        }

        #endregion Move

        private bool reachedDestination()
        {
            return (StopMovementCondition == StopMovementConditions.WITHIN_PERSONAL_SPACE && IsWithinPersonalSpace())
                || (StopMovementCondition == StopMovementConditions.WITHIN_INTERACTION_DISTANCE && IsWithinInteractionDistance())
                || (StopMovementCondition == StopMovementConditions.AT_POSITION && transform.position == FocusedOnPosition.Value);
        }

        public Vector3 GetMoveAwayDestination()
        {
            if (FocusedOnPosition != null)
            {
                return transform.position + (transform.position - FocusedOnPosition.Value).normalized * MovingAwayDistance;
            }
            return transform.position + MovingAwayDistance * -transform.forward;
        }


        #region States

        private void OnStateChanged_Listener(LocomotionStates state)
        {
            CurrentLocomotionType.OnStateChange(state);
        }

        #region MovingToPosition


        private void InitStates()
        {
            FSM.OnStateChanged_AddListener(OnStateChanged_Listener);
            FSM.OnStarted_AddListener(LocomotionStates.NotMoving, NotMoving_Enter);
            FSM.SetUpdateAction(LocomotionStates.CannotMove, CannotMove_Update);
            FSM.SetUpdateAction(LocomotionStates.MovingToPosition, MovingToPosition_Update);
            FSM.SetUpdateAction(LocomotionStates.MovingToTarget, MovingToTarget_Update);
            FSM.SetUpdateAction(LocomotionStates.MovingAwayFromPosition, MovingAwayFromPosition_Update);
            FSM.SetUpdateAction(LocomotionStates.MovingAwayFromTarget, MovingAwayFromTarget_Update);
        }

        private void MovingToPosition_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(LocomotionStates.CannotMove);
            }
            else if (IsNotFocusedOrReachedDestination())
            {
                if (FocusedOnPosition != null)
                {
                    OnDestinationReached.Invoke(FocusedOnPosition.Value);
                }

                FSM.ChangeState(LocomotionStates.NotMoving);
            }
            else
            {
                try
                {
                    CurrentLocomotionType.MoveTowardsPosition(_fullspeed);
                }
                catch (System.Exception ex)
                {

                    if (DebugMode)
                    {
                        Debug.LogError($"Locomotion Error: {ex.Message}");
                    }

                    FSM.ChangeState(LocomotionStates.CannotMove);

                }
            }
        }

        #endregion MovingToPosition

        #region MovingAwayFromPosition
        private void MovingAwayFromPosition_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(LocomotionStates.CannotMove);
            }
            else if (IsNotFocusedOrIsAway())
            {
                FSM.ChangeState(LocomotionStates.NotMoving);
            }
            else
            {
                try
                {
                    CurrentLocomotionType.MoveAway();
                }
                catch (System.Exception ex)
                {

                    if (DebugMode)
                    {
                        Debug.LogError($"Locomotion Error: {ex.Message}");
                    }

                    FSM.ChangeState(LocomotionStates.CannotMove);

                }
            }
        }

        #endregion MovingAwayFromPosition


        #region MovingToTarget

        private void MovingToTarget_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(LocomotionStates.CannotMove);
            }
            else if (IsNotFocusedOrReachedDestination())
            {
                FSM.ChangeState(LocomotionStates.NotMoving);
            }
            else
            {
                MoveTowardsTarget(_fullspeed);
            }

        }

        #endregion MovingToTarget

        #region MovingAwayFromTarget
        private void MovingAwayFromTarget_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(LocomotionStates.CannotMove);
            }
            else if (IsNotFocusedOrIsAway())
            {
                FSM.ChangeState(LocomotionStates.NotMoving);
            }
            else
            {
                try
                {
                    CurrentLocomotionType.MoveAway();
                }
                catch (System.Exception ex)
                {

                    if (DebugMode)
                    {
                        Debug.LogError($"Locomotion Error: {ex.Message}");
                    }

                    FSM.ChangeState(LocomotionStates.CannotMove);

                }
            }
        }

        #endregion MovingAwayFromTarget

        #region CannotMove

        private void CannotMove_Update()
        {
            if (CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(LocomotionStates.NotMoving);
            }
        }

        #endregion CannotMove

        #region NotMoving

        private void NotMoving_Enter()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(LocomotionStates.CannotMove);
            }
        }

        #endregion NotMoving

        #endregion States
        protected abstract void InitLocomotionTypes();
        protected virtual bool InitBotWander(){
            BotWanderManagerComponent = GetComponent<BotWanderManager>();
            return BotWanderManagerComponent != null;
        }
        #region MonoBehaviour Functions
        
        protected override void Awake()
        {
            base.Awake();
            InitLocomotionTypes();
            InitStates();
            BTFiniteStateMachineManagerComponent.AddFSM(FSM);
            InitBotWander();
            //Initialize();
            //_btFiniteStateMachineManagerComponent.StartFSMs();
        }

        protected override void Update()
        {
            base.Update();
            AnimateLocomotion();
        }

        #endregion MonoBehaviour Functions
    }
}

