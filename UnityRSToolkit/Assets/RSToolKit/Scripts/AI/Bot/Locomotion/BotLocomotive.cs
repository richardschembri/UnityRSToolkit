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
        public bool IsFreefall { get; protected set; } = false;

        public enum FStatesLocomotion
        {
            NotMoving,
            CannotMove,
            MovingToPosition,
            MovingToTarget,
            MovingAwayFromPosition,
            MovingAwayFromTarget
        }
      
        public BotLogicLocomotion CurrentLocomotionType { get; set; }

        public BTFiniteStateMachine<FStatesLocomotion> FSM { get; private set; } = new BTFiniteStateMachine<FStatesLocomotion>(FStatesLocomotion.NotMoving);

        public FStatesLocomotion CurrentFState
        {
            get
            {
                return FSM.CurrentState;
            }
        }
        public float CurrentSpeed { get { return CurrentLocomotionType != null ? CurrentLocomotionType .CurrentSpeed : 0; } }

        protected bool _fullspeed = true;

        public DistanceType? StopMovementCondition { get; private set; } = null;

        public float MovingAwayDistance
        {
            get
            {
                return SqrAwarenessMagnitude * 1.1f;
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

        public BotPartWanderManager BotWanderManagerComponent{ get; private set;}

        public ProximityChecker JumpProximityChecker;
        #endregion Components

        /*
        public bool IsFarFromGround()
        {
            return GroundProximityCheckerComponent.IsWithinRayDistance() == null;
        }
        */

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
            return CurrentFState != FStatesLocomotion.CannotMove
                    && CurrentFState != FStatesLocomotion.NotMoving;
        }

        public void AnimateLocomotion()
        {
            CharacterAnimParams.TrySetSpeed(AnimatorComponent, CurrentSpeed);
        }

        public bool IsAway()
        {
            return !IsWithinDistance(DistanceType.AWARENESS);
        }

        public bool WillBeAway()
        {
            if (FocusedOnPosition == null)
            {
                return false;
            }

            return Vector3.Distance(FocusedOnPosition.Value, transform.position) >= MovingAwayDistance;
        }

        protected bool IsNotFocusedOrIsAway()
        {
            return FocusedOnPosition == null || IsAway();
        }

        public bool IsNotFocusedOrReachedDestination()
        {
            return FocusedOnPosition == null || HasReachedDestination();
        }

        public float? IsCloseToSurface()
        {
            return GroundProximityCheckerComponent.IsWithinRayDistance();
        }

        float? _surfaceDistance = null;
        public bool IsCloseToSurface(float value)
        {
            _surfaceDistance = GroundProximityCheckerComponent.IsWithinRayDistance();
            return (_surfaceDistance != null && _surfaceDistance.Value <= value);
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

        public bool IsCloseToNavMeshSurface(float value, out NavMeshHit hit)
        {
            _surfaceDistance = GroundProximityCheckerComponent.IsWithinRayDistance(out hit);
            return (_surfaceDistance != null && _surfaceDistance.Value <= value);
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

        public bool MoveToTetherPoint(Bot.DistanceType? stopMovementCondition = Bot.DistanceType.PERSONAL_SPACE, bool fullspeed = true)
        {
            FocusOnTransform(TetherToTransform);
            return MoveToTarget(stopMovementCondition, fullspeed);
        }

        public bool MoveTowardsTarget(bool fullspeed = true)
        {
            if(FocusedOnTransform == null)
            {
                FSM.ChangeState(FStatesLocomotion.NotMoving);
                return false;
            }
            FocusOnPosition(FocusedOnTransform.position);
            try
            {
                return CurrentLocomotionType.MoveTowardsPosition(fullspeed);
            }
            catch (System.Exception ex)
            {
                LogErrorLocomotion(ex);
                FSM.ChangeState(FStatesLocomotion.CannotMove);
                return false;
            }
        }

        #region Move

        private bool MoveCommon(FStatesLocomotion moveType, bool fullspeed = true, DistanceType? stopMovementCondition = null)
        {            
            StopMovementCondition = stopMovementCondition;
            if (!CurrentLocomotionType.CanMove() || HasReachedDestination())
            {
                // StopMovementCondition = null; // StopMovementConditions.NONE;
                return false;
            }
            _fullspeed = fullspeed;
            FSM.ChangeState(moveType);
            return true;
        }

        public bool MoveToPosition(DistanceType? stopMovementCondition, bool fullspeed = true)
        {
            return MoveCommon(FStatesLocomotion.MovingToPosition, fullspeed, stopMovementCondition);
        }

        public bool MoveToPosition(Vector3 position, DistanceType? stopMovementCondition, bool fullspeed = true)
        {
            FocusOnPosition(position);
            return MoveToPosition(stopMovementCondition, fullspeed);
        }

        public bool MoveAwayFromPosition(bool fullspeed = true)
        {
            return MoveCommon(FStatesLocomotion.MovingAwayFromPosition, fullspeed);
        }

        public bool MoveToTarget(DistanceType? stopMovementCondition, bool fullspeed = true)
        {
            return MoveCommon(FStatesLocomotion.MovingToTarget, fullspeed, stopMovementCondition);
        }

        public bool MoveToTarget(Transform transform, DistanceType? stopMovementCondition, bool fullspeed = true)
        {
            if (AttractMyAttention_ToTransform(transform, true))
            {
                return MoveToTarget(stopMovementCondition, fullspeed);
            }
            return false;
        }

        public bool MoveAwayFromTarget(bool fullspeed = true)
        {
            return MoveCommon(FStatesLocomotion.MovingAwayFromTarget, fullspeed);
        }

        public bool StopMoving()
        {
            if (CurrentFState != FStatesLocomotion.NotMoving)
            {
                FSM.ChangeState(FStatesLocomotion.NotMoving);
                return true;
            }
            return false;
        }

        #endregion Move

        public bool HasReachedDestination()
        {
            return StopMovementCondition == null ? false : CurrentLocomotionType.HasReachedDestination(StopMovementCondition.Value);
        }

        public Vector3 GetMoveAwayDestination(float moveAwayBy)
        {
            if (FocusedOnPosition != null)
            {
                return transform.position + ((transform.position - FocusedOnPosition.Value).normalized * moveAwayBy);
            }
            return transform.position + moveAwayBy * -transform.forward;
        }

        public Vector3 GetMoveAwayDestination()
        {
            return GetMoveAwayDestination(MovingAwayDistance);
        }


        #region States

        private void OnStateChanged_Listener(FStatesLocomotion state)
        {
            CurrentLocomotionType.OnStateChange(state);
        }

        #region MovingToPosition


        private void InitStates()
        {
            FSM.OnStateChanged_AddListener(OnStateChanged_Listener);
            FSM.OnStarted_AddListener(FStatesLocomotion.NotMoving, NotMoving_Enter);
            FSM.SetUpdateAction(FStatesLocomotion.CannotMove, CannotMove_Update);
            FSM.SetUpdateAction(FStatesLocomotion.MovingToPosition, MovingToPosition_Update);
            FSM.OnStarted_AddListener(FStatesLocomotion.MovingToPosition, MovingToPositionOnStarted_Listener);
            FSM.SetUpdateAction(FStatesLocomotion.MovingToTarget, MovingToTarget_Update);
            FSM.SetUpdateAction(FStatesLocomotion.MovingAwayFromPosition, MovingAwayFromPosition_Update);
            FSM.OnStarted_AddListener(FStatesLocomotion.MovingAwayFromPosition, MovingAwayFromPositionOnStarted_Listener);
            FSM.SetUpdateAction(FStatesLocomotion.MovingAwayFromTarget, MovingAwayFromTarget_Update);
        }

        private void MovingToPositionOnStarted_Listener()
        {
            try
            {
                CurrentLocomotionType.MoveTowardsPosition(_fullspeed);
            }
            catch (System.Exception ex)
            {
                LogErrorLocomotion(ex);
                FSM.ChangeState(FStatesLocomotion.CannotMove);

            }
        }

        protected virtual void MovingToPosition_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(FStatesLocomotion.CannotMove);
            }
            else if (IsNotFocusedOrReachedDestination())
            {
                if (FocusedOnPosition != null)
                {
                    OnDestinationReached.Invoke(FocusedOnPosition.Value);
                }

                FSM.ChangeState(FStatesLocomotion.NotMoving);
            }
        }

        #endregion MovingToPosition

        #region MovingAwayFromPosition

        private void MovingAwayFromPositionOnStarted_Listener()
        {
            try
            {
                CurrentLocomotionType.MoveAway(_fullspeed);
            }
            catch (System.Exception ex)
            {
                LogErrorLocomotion(ex);
                FSM.ChangeState(FStatesLocomotion.CannotMove);

            }
        }

        private void MovingAwayFromPosition_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(FStatesLocomotion.CannotMove);
            }
            else if (IsNotFocusedOrIsAway())
            {
                FSM.ChangeState(FStatesLocomotion.NotMoving);
            }            
            else if(!WillBeAway())
            {
                try
                {
                    CurrentLocomotionType.MoveAway(_fullspeed);
                }
                catch (System.Exception ex)
                {
                    LogErrorLocomotion(ex);                   
                    FSM.ChangeState(FStatesLocomotion.CannotMove);

                }
            }
           
        }

        #endregion MovingAwayFromPosition


        #region MovingToTarget

        private void MovingToTarget_Update()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(FStatesLocomotion.CannotMove);
            }
            else if (IsNotFocusedOrReachedDestination())
            {
                FSM.ChangeState(FStatesLocomotion.NotMoving);
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
                FSM.ChangeState(FStatesLocomotion.CannotMove);
            }
            else if (IsNotFocusedOrIsAway())
            {
                FSM.ChangeState(FStatesLocomotion.NotMoving);
            }
            else
            {
                try
                {
                    CurrentLocomotionType.MoveAway();
                }
                catch (System.Exception ex)
                {
                    LogErrorLocomotion(ex);
                    FSM.ChangeState(FStatesLocomotion.CannotMove);

                }
            }
        }

        #endregion MovingAwayFromTarget

        #region CannotMove

        private void CannotMove_Update()
        {
            if (CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(FStatesLocomotion.NotMoving);
            }
        }

        #endregion CannotMove

        #region NotMoving

        private void NotMoving_Enter()
        {
            if (!CurrentLocomotionType.CanMove())
            {
                FSM.ChangeState(FStatesLocomotion.CannotMove);
            }
        }

        #endregion NotMoving

        #endregion States

        protected abstract void InitLocomotionTypes();
        protected virtual bool InitBotWander(){
            BotWanderManagerComponent = GetComponent<BotPartWanderManager>();
            return BotWanderManagerComponent != null;
        }

        public override bool Initialize(bool force = false)
        {
            if (!base.Initialize(force))
            {
                return false;
            }
            InitLocomotionTypes();
            InitStates();
            BTFiniteStateMachineManagerComponent.AddFSM(FSM);
            InitBotWander();
            return true;
        }

        public void FreezePosition()
        {
            RigidBodyComponent.isKinematic = true;
            RigidBodyComponent.constraints = RigidbodyConstraints.FreezePosition;
        }

        public void UnFreezePosition()
        {
            RigidBodyComponent.isKinematic = false;
            RigidBodyComponent.constraints = RigidbodyConstraints.None;
        }


        #region MonoBehaviour Functions

        protected override void Awake()
        {
            base.Awake();
            
            //Initialize();
            //_btFiniteStateMachineManagerComponent.StartFSMs();
        }

        protected override void Update()
        {
            base.Update();
            AnimateLocomotion();
        }

        #endregion MonoBehaviour Functions

        protected void LogErrorLocomotion(System.Exception ex)
        {
            LogErrorInDebugMode($"Locomotion Error:", ex);
        }
    }
}
