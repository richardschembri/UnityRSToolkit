using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour.Composite;
using System;
using System.Linq;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Task;
using UnityEngine.Events;
using System.Collections.ObjectModel;

namespace RSToolkit.AI.FSM
{
    public interface IBTFiniteStateMachine
    {
        string GetName();
        string GetCurrentStateText();
        BehaviourNode GetFSMNode();
    }

    public class BTFiniteStateMachine<T> : IBTFiniteStateMachine where T : struct, IConvertible, IComparable {

        public BehaviourStateSelector<T> _stateSelector;       
        private Dictionary<T, Action> _updateActions = new Dictionary<T, Action>();
        private List<Action<T>> _onChangeListeners = new List<Action<T>>();

#region Constructor

        public BTFiniteStateMachine(T initialState, string name = "")
        {
            _stateSelector = new BehaviourStateSelector<T>(initialState);
            _stateSelector.Name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            
            var states = Enum.GetValues(typeof(T));
            foreach(T state in states)
            {
                _stateSelector.SetStateAction(state, DoFSMState);
            }
        }

#endregion Constructor

#region Change State
        public void ChangeState(T newState, bool silent = false)
        {
            _stateSelector.ChangeState(newState, silent);
        }
        public void ChangeStateIn(float time, T newState, bool silent = false)
        {
            _stateSelector.ChangeStateIn(time, newState, silent);
        }
#endregion Change State

        public string GetName()
        {
            return _stateSelector.Name;
        }

        public T CurrentState
        {
            get{
                return _stateSelector.CurrentState;
            }
        }

        public T LastState{
            get{
                return _stateSelector.LastState;
            }
        }

        public T NextState
        {
            get
            {
                return _stateSelector.NextState;
            }
        }

        public string GetCurrentStateText()
        {
            return CurrentState.ToString();
        }

        #region SetUpdate

        public void SetUpdateAction(T state, Action action)
        {
            if(!_updateActions.ContainsKey(state)){
                _updateActions.Add(state, action);
                return;
            }

            _updateActions[state] = action;            
        }

        public void SetFixedUpdateAction(T state, Action action)
        {
            _stateSelector.GetStateBehaviour(state).SetFixedUpdate(action);
        }

        public void SetLateUpdateAction(T state, Action action)
        {
            _stateSelector.GetStateBehaviour(state).SetLateUpdate(action);
        }

        #endregion SetUpdate

        #region AddListener

        public void OnStarted_AddListener(T state, UnityAction listener)
        {
            _stateSelector.GetStateBehaviour(state).OnStarted.AddListener(listener);
        }

        public void OnStopping_AddListener(T state, UnityAction listener)
        {
            _stateSelector.GetStateBehaviour(state).OnStopping.AddListener(listener);
        }

        public void OnStopped_AddListener(T state, UnityAction<bool> listener)
        {
            _stateSelector.GetStateBehaviour(state).OnStopped.AddListener(listener);
        }

        
        public void OnStateChanged_AddListener(Action<T> listener)
        {
            _onChangeListeners.Add(listener);
            _stateSelector.OnChanged += listener;
        }

        #endregion AddListener

        #region RemoveAllListeners

        public void RemoveAllListeners_OnStarted(T state)
        {
            _stateSelector.GetStateBehaviour(state).OnStarted.RemoveAllListeners();
        }

        public void RemoveAllListeners_OnStopping(T state)
        {
            _stateSelector.GetStateBehaviour(state).OnStopping.RemoveAllListeners();
        }

        public void RemoveAllListeners_OnStopped(T state)
        {
            _stateSelector.GetStateBehaviour(state).OnStopped.RemoveAllListeners();
        }

        public void RemoveAllListeners(T state)
        {
            RemoveAllListeners_OnStarted(state);
            RemoveAllListeners_OnStopping(state);
            RemoveAllListeners_OnStopped(state);
        }

        public void RemoveAllListeners_OnStateChanged()
        {
            for (int i = 0; i < _onChangeListeners.Count; i++)
            {
                _stateSelector.OnChanged -= _onChangeListeners[i];
            }

            _onChangeListeners.Clear();
        }

        public void RemoveAllListeners()
        {
            for(int i = 0; i < _stateSelector.States.Length; i++)
            {
                RemoveAllListeners((T)_stateSelector.States.GetValue(i));
            }

            RemoveAllListeners_OnStateChanged();
        }

        #endregion RemoveAllListeners

        public BehaviourNode GetFSMNode()
        {
            return _stateSelector;
        }
        private BehaviourAction.ActionResult DoFSMState(bool cancel)
        {
            if (cancel)
            {
                return BehaviourAction.ActionResult.SUCCESS;
            }
           
            if (_updateActions.ContainsKey(_stateSelector.CurrentState))
            {
                _updateActions[_stateSelector.CurrentState].Invoke();
            }

            return BehaviourAction.ActionResult.PROGRESS;

        }

    }


}
