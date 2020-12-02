using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.FSM;
using RSToolkit.Helpers;
using System.Linq;

namespace RSToolkit.AI.Locomotion
{
    [RequireComponent(typeof(BotLocomotive))]
    public class BotPartWanderManager : MonoBehaviour
    {

        public enum FStatesWander
        {
            NotWandering,
            FindNewPosition,
            MovingToPosition,
            CannotWander
        }

        private const string DEBUG_TAG = "BotWanderManager";
        
        public bool DebugMode = false;

        public bool _waitOnStart = true;

        #region FSM
        public BTFiniteStateMachine<FStatesWander> FSM { get; private set; } = new BTFiniteStateMachine<FStatesWander>(FStatesWander.NotWandering);
        private BTFiniteStateMachineManager _btFiniteStateMachineManagerComponent; 
        private BTFiniteStateMachineManager BTFiniteStateMachineManagerComponent{
            get
            {
                if (_btFiniteStateMachineManagerComponent == null)
                {
                    _btFiniteStateMachineManagerComponent = GetComponent<BTFiniteStateMachineManager>();
                }
                return _btFiniteStateMachineManagerComponent  ;
            }
        } 
        #endregion FSM

        private BotLocomotive m_botLocomotiveComponent;
        public BotLocomotive BotLocomotiveComponent
        {
            get
            {
                if (m_botLocomotiveComponent == null)
                {
                    m_botLocomotiveComponent = GetComponent<BotLocomotive>();
                }
                return m_botLocomotiveComponent;
            }

        }
        protected BotPartWander _currentBotWanderComponent;

        protected BotPartWander[] _botWanderComponents;
        protected BotPartWander[] _BotWanderComponents
        {
            get
            {
                if (_botWanderComponents == null)
                {
                    _botWanderComponents = GetComponents<BotPartWander>();
                }
                return _botWanderComponents;
            }
            private set
            {
                _botWanderComponents = value;
            }
        }

        public void SetCurrentBotWander(BotPartWander b)
        {
            if (_BotWanderComponents.Contains(b))
            {
                StopWandering(true);
                _currentBotWanderComponent = b;
            }
            else
            {
                throw new System.Exception($"{name} does not contain component");
            }
        }

        public Transform WanderCenter
        {
            get
            {
                return BotLocomotiveComponent.TetherToTransform != null ? BotLocomotiveComponent.TetherToTransform : transform;
            }
        }

        private float GetWaitTime()
        {
            if ((FSM.LastState == FStatesWander.NotWandering || FSM.LastState == FStatesWander.CannotWander) && !_waitOnStart)
            {
                return 0.1f;
            }

            if (_currentBotWanderComponent.randomizeWait)
            {
                return RandomHelpers.RandomFloatWithinRange(_currentBotWanderComponent.WaitTime *.75f, _currentBotWanderComponent.WaitTime);
            }
            return _currentBotWanderComponent.WaitTime;
        }

        public void Wander(bool waitOnStart)
        {
            Wander(BotLocomotiveComponent.SqrAwarenessMagnitude, waitOnStart);
        }

        public bool Wander(float radius, bool waitOnStart)
        {
            _waitOnStart = waitOnStart;
            _currentBotWanderComponent.SetWanderRadius(radius);

            if (!_currentBotWanderComponent.CanWander())
            {
                FSM.ChangeState(FStatesWander.CannotWander);
            }
            else if (FSM.CurrentState == FStatesWander.NotWandering)
            {
                FSM.ChangeState(FStatesWander.FindNewPosition);
                return true;
            }

            return false;
        }
        public bool StopWandering(bool stopMoving = false)
        {
            if (FSM.CurrentState != FStatesWander.NotWandering)
            {
                if (stopMoving)
                {
                    BotLocomotiveComponent.StopMoving();
                }
                FSM.ChangeState(FStatesWander.NotWandering);
                return true;
            }

            return false;
        }

        public bool IsWandering()
        {
            return FSM.CurrentState != FStatesWander.NotWandering
                    && FSM.CurrentState != FStatesWander.CannotWander;
        }

        #region States
        private void InitStates()
        {
            FSM.OnStarted_AddListener(FStatesWander.FindNewPosition, FindNewPosition_Enter);
            FSM.SetUpdateAction(FStatesWander.FindNewPosition, FindNewPosition_Update);

            FSM.OnStarted_AddListener(FStatesWander.MovingToPosition, MovingToPosition_Enter);
            FSM.SetUpdateAction(FStatesWander.MovingToPosition, MovingToPosition_Update);
            FSM.OnStopped_AddListener(FStatesWander.MovingToPosition, MovingToPosition_Exit);

            FSM.SetUpdateAction(FStatesWander.CannotWander, CannotWander_Update);
        }

        #region FindNewPosition
        private float _findPositionTimeout = 0f;
        void FindNewPosition_Enter()
        {
            BotLocomotiveComponent.UnFocus();
            _findPositionTimeout = GetWaitTime();
        }

        Vector3? _newWanderPosition;
        void FindNewPosition_Update()
        { 
            
            _findPositionTimeout -= Time.deltaTime;
            if(_findPositionTimeout > 0){
                return;
            }

            _newWanderPosition = _currentBotWanderComponent.GetNewWanderPosition(WanderCenter);
            if(_newWanderPosition != null){
                BotLocomotiveComponent.FocusOnPosition(_newWanderPosition.Value );
                FSM.ChangeState(FStatesWander.MovingToPosition);
            }else{
                FSM.ChangeState(FStatesWander.CannotWander);
            }
        }
        #endregion FindNewPosition

        #region MovingToPosition
        void MovingToPosition_Update()
        {
            if (!_currentBotWanderComponent.CanWander())
            {
                FSM.ChangeState(FStatesWander.CannotWander);
            }
            else if ( BotLocomotiveComponent.CurrentFState == BotLocomotive.FStatesLocomotion.NotMoving 
                        && BotLocomotiveComponent.FSM.NextState != BotLocomotive.FStatesLocomotion.MovingToPosition 
                        && BotLocomotiveComponent.FSM.NextState != BotLocomotive.FStatesLocomotion.MovingAwayFromPosition
                            )
            {

                FSM.ChangeState(_currentBotWanderComponent.AutoWander ? FStatesWander.FindNewPosition : FStatesWander.NotWandering);
            }

        }
        IEnumerator _movingToPosition_TimeOut;
        void MovingToPosition_Enter()
        {
            if (_currentBotWanderComponent.MovementTimeout > 0)
            {
                _movingToPosition_TimeOut = MovingToPosition_TimeOut();
                if(!BotLocomotiveComponent.MoveToPosition(_currentBotWanderComponent.StopMovementCondition, false))
                {
                    FSM.ChangeState(FStatesWander.CannotWander);
                    return;
                }
                DebugHelpers.LogInDebugMode(DebugMode, DEBUG_TAG, $"{transform.name} Wandering to {BotLocomotiveComponent.FocusedOnPosition.ToString()}");
                StartCoroutine(_movingToPosition_TimeOut);
            }
        }

        void MovingToPosition_Exit(bool success)
        {
            StopCoroutine(_movingToPosition_TimeOut);
        }

        IEnumerator MovingToPosition_TimeOut()
        {
            yield return new WaitForSeconds(_currentBotWanderComponent.MovementTimeout);
            if (FSM.CurrentState == FStatesWander.MovingToPosition)
            {
                if (DebugMode)
                {
                    Debug.Log("Movement timeout!");
                }
                FSM.ChangeState(FStatesWander.FindNewPosition);
            }
        }
        #endregion MovingToPosition

        #region CannotWander
        void CannotWander_Update()
        {
            if (_currentBotWanderComponent.CanWander())
            {
                FSM.ChangeState(FStatesWander.FindNewPosition);
            }
        }
        #endregion CannotWander
        #endregion States
        // Start is called before the first frame update
        public void Initialize(BotPartWander initialBotWander){
            SetCurrentBotWander(initialBotWander);
            InitStates();
            BTFiniteStateMachineManagerComponent.AddFSM(FSM);
        }

        #region Mono Functions
        void OnCollisionEnter(Collision collision)
        {
            if(FSM.CurrentState == FStatesWander.MovingToPosition )
            {

                // BotLocomotiveComponent.StopMoving();
                BotLocomotiveComponent.FocusOnPosition(collision.GetContact(0).point);
                BotLocomotiveComponent.MoveAwayFromPosition(false);
                //FSM.ChangeState(FStatesWander.FindNewPosition);
            }
        }

        #endregion Mono Functions
    }

}