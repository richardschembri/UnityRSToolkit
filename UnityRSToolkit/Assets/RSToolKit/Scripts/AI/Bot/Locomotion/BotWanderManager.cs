using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.FSM;
using RSToolkit.Helpers;
using System.Linq;

namespace RSToolkit.AI.Locomotion
{
    [RequireComponent(typeof(BotLocomotive))]
    public class BotWanderManager : MonoBehaviour
    {

        public enum WanderStates
        {
            NotWandering,
            FindNewPosition,
            MovingToPosition,
            CannotWander
        }

        private const string DEBUG_TAG = "BotWanderManager";
        public bool DebugMode = false;

        #region FSM
        public BTFiniteStateMachine<WanderStates> FSM { get; private set; } = new BTFiniteStateMachine<WanderStates>(WanderStates.NotWandering);
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
        protected BotWander _currentBotWanderComponent;

        protected BotWander[] _botWanderComponents;
        protected BotWander[] _BotWanderComponents
        {
            get
            {
                if (_botWanderComponents == null)
                {
                    _botWanderComponents = GetComponents<BotWander>();
                }
                return _botWanderComponents;
            }
            private set
            {
                _botWanderComponents = value;
            }
        }

        public void SetCurrentBotWander(BotWander b)
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
            if (FSM.LastState == WanderStates.NotWandering || FSM.LastState == WanderStates.CannotWander)
            {
                return 0.1f;
            }

            if (_currentBotWanderComponent.randomizeWait)
            {
                return RandomHelpers.RandomFloatWithinRange(_currentBotWanderComponent.WaitTime *.75f, _currentBotWanderComponent.WaitTime);
            }
            return _currentBotWanderComponent.WaitTime;
        }

        public void Wander()
        {
            Wander(BotLocomotiveComponent.SqrAwarenessMagnitude);
        }

        public bool Wander(float radius)
        {
            _currentBotWanderComponent.SetWanderRadius(radius);

            if (!_currentBotWanderComponent.CanWander())
            {
                FSM.ChangeState(WanderStates.CannotWander);
            }
            else if (FSM.CurrentState == WanderStates.NotWandering)
            {
                FSM.ChangeState(WanderStates.FindNewPosition);
                return true;
            }

            return false;
        }
        public bool StopWandering(bool stopMoving = false)
        {
            if (FSM.CurrentState != WanderStates.NotWandering)
            {
                if (stopMoving)
                {
                    BotLocomotiveComponent.StopMoving();
                }
                FSM.ChangeState(WanderStates.NotWandering);
                return true;
            }

            return false;
        }

        public bool IsWandering()
        {
            return FSM.CurrentState != WanderStates.NotWandering
                    && FSM.CurrentState != WanderStates.CannotWander;
        }

        #region States
        private void InitStates()
        {
            FSM.OnStarted_AddListener(WanderStates.FindNewPosition, FindNewPosition_Enter);
            FSM.SetUpdateAction(WanderStates.FindNewPosition, FindNewPosition_Update);

            FSM.OnStarted_AddListener(WanderStates.MovingToPosition, MovingToPosition_Enter);
            FSM.SetUpdateAction(WanderStates.MovingToPosition, MovingToPosition_Update);
            FSM.OnStopped_AddListener(WanderStates.MovingToPosition, MovingToPosition_Exit);

            FSM.SetUpdateAction(WanderStates.CannotWander, CannotWander_Update);
        }

        #region FindNewPosition
        private float m_findPositionTimeout = 0f;
        void FindNewPosition_Enter()
        {
            BotLocomotiveComponent.UnFocus();
            m_findPositionTimeout = GetWaitTime();
        }

        Vector3? _newWanderPosition;
        void FindNewPosition_Update()
        { 
            m_findPositionTimeout -= Time.deltaTime;
            if(m_findPositionTimeout > 0){
                return;
            }

            _newWanderPosition = _currentBotWanderComponent.GetNewWanderPosition(WanderCenter);
            if(_newWanderPosition != null){
                BotLocomotiveComponent.FocusOnPosition(_newWanderPosition.Value );
                FSM.ChangeState(WanderStates.MovingToPosition);
            }else{
                FSM.ChangeState(WanderStates.CannotWander);
            }
        }
        #endregion FindNewPosition

        #region MovingToPosition
        void MovingToPosition_Update()
        {
            if (!_currentBotWanderComponent.CanWander())
            {
                FSM.ChangeState(WanderStates.CannotWander);
            }
            else if ( BotLocomotiveComponent.CurrentState == BotLocomotive.LocomotionStates.NotMoving)
            {

                FSM.ChangeState(_currentBotWanderComponent.AutoWander ? WanderStates.FindNewPosition : WanderStates.NotWandering);
            }

        }
        IEnumerator m_movingToPosition_TimeOut;
        void MovingToPosition_Enter()
        {
            if (_currentBotWanderComponent.MovementTimeout > 0)
            {
                m_movingToPosition_TimeOut = MovingToPosition_TimeOut();
                if(!BotLocomotiveComponent.MoveToPosition(BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, false))
                {
                    FSM.ChangeState(WanderStates.CannotWander);
                    return;
                }
                DebugHelpers.LogInDebugMode(DebugMode, DEBUG_TAG, $"{transform.name} Wandering to {BotLocomotiveComponent.FocusedOnPosition.ToString()}");
                StartCoroutine(m_movingToPosition_TimeOut);
            }
        }

        void MovingToPosition_Exit(bool success)
        {
            StopCoroutine(m_movingToPosition_TimeOut);
        }

        IEnumerator MovingToPosition_TimeOut()
        {
            yield return new WaitForSeconds(_currentBotWanderComponent.MovementTimeout);
            if (FSM.CurrentState == WanderStates.MovingToPosition)
            {
                if (DebugMode)
                {
                    Debug.Log("Movement timeout!");
                }
                FSM.ChangeState(WanderStates.FindNewPosition);
            }
        }
        #endregion MovingToPosition

        #region CannotWander
        void CannotWander_Update()
        {
            if (_currentBotWanderComponent.CanWander())
            {
                FSM.ChangeState(WanderStates.FindNewPosition);
            }
        }
        #endregion CannotWander
        #endregion States
        // Start is called before the first frame update
        public void Initialize(BotWander initialBotWander){
            SetCurrentBotWander(initialBotWander);
            InitStates();
            BTFiniteStateMachineManagerComponent.AddFSM(FSM);
        }

    }

}