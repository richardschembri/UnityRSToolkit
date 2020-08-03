using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour.Task;

namespace RSToolkit.AI.Behaviour.Composite
{
    public class BehaviourStateSelector<T> : BehaviourParentNode where T : struct, IConvertible, IComparable
    {
        private Array _states;
        private Dictionary<T, BehaviourAction> _stateActions;

        public T CurrentState { get; private set; }
        public T LastState { get; private set; }
        private T _nextstate;

        public event Action<T> OnChanged;

        public BehaviourStateSelector(T initialState) : base("Enum Selector", NodeType.COMPOSITE){
            _nextstate = initialState;
            CurrentState = initialState;
            _states = Enum.GetValues(typeof(T));
            _stateActions = new Dictionary<T, BehaviourAction>();
            if (_states.Length < 1) { throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition"); }
            OnStarted.AddListener(OnStarted_Listener);
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        public BehaviourAction SetStateAction(T state, System.Func<bool, BehaviourAction.ActionResult> updateFunc){
            var stateaction = new BehaviourAction(updateFunc, Enum.GetName(typeof(T), state));

            if (_stateActions.ContainsKey(state))
            {
                _stateActions[state] = stateaction;
            }
            else
            {
                _stateActions.Add(state, stateaction);
                AddChild(stateaction);
            }
            
            return stateaction;
        }

        public BehaviourAction GetStateBehaviour(T state)
        {
            return _stateActions[state];
        }

        public void ChangeState(T newState, bool silent = false)
        {
            _nextstate = newState;
            if(_stateActions[CurrentState].State == NodeState.INACTIVE){
                ChangeState(silent);
            }else if(_stateActions[CurrentState].State == NodeState.ACTIVE){
                _stateActions[CurrentState].RequestStopNode(silent);
            }
        }

        private void ChangeState()
        {
            ChangeState(false);
        }

        private void ChangeState(bool silent){
            LastState = CurrentState;
            CurrentState = _nextstate;
            if (!silent)
            {
                OnChanged?.Invoke(CurrentState);
            }            
            _stateActions[CurrentState].StartNode(silent);
        }

        #region Events

        private void OnStarted_Listener()
        {
            RunOnNextTick(ChangeState);
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(State == NodeState.ACTIVE && _stateActions[_nextstate] != _stateActions[CurrentState]){
                RunOnNextTick(ChangeState);
            }else if(State == NodeState.STOPPING){
                StopNode(true);
            }else{
                StopNode(false);
            }
        }

        #endregion Events
    }
}