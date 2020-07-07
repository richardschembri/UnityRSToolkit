using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour.Task;

namespace RSToolkit.AI.Behaviour.Composite
{
    public class BehaviourStateSelector<T> : BehaviourParentNode where T : struct, IConvertible, IComparable
    {
        private Array m_states;
        private Dictionary<T, BehaviourAction> m_stateActions;

        public T CurrentState{get; private set;}
        private T m_nextstate;

        public BehaviourStateSelector(T initialState) : base("Enum Selector", NodeType.COMPOSITE){
            m_nextstate = initialState;
            m_states = Enum.GetValues(typeof(T));
            if (m_states.Length < 1) { throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition"); }
            OnChildNodeStopped.AddListener(OnChildNodeStopped_Listener);
        }

        public BehaviourAction SetStateAction(T state, System.Func<bool, BehaviourAction.ActionResult> updateFunc){
            var stateaction = new BehaviourAction(updateFunc, Enum.GetName(typeof(T), state));
            m_stateActions.Add(state, stateaction);
            return stateaction;
        }

        public void ChangeState(T newState){
            m_nextstate = newState;
            if(m_stateActions[CurrentState].State == NodeState.INACTIVE){
                ChangeState();
            }else if(m_stateActions[CurrentState].State == NodeState.ACTIVE){
                m_stateActions[CurrentState].RequestStopNode();
            }
        }

        private void ChangeState(){
            CurrentState = m_nextstate;
            m_stateActions[CurrentState].StartNode(false);
        }

        private void OnStarted_Listener()
        {
            RunOnNextTick(ChangeState);
        }

        private void OnChildNodeStopped_Listener(BehaviourNode child, bool success)
        {
            if(State == NodeState.ACTIVE && m_stateActions[m_nextstate] != m_stateActions[CurrentState]){
                RunOnNextTick(ChangeState);
            }else if(State == NodeState.STOPPING){
                StopNode(true);
            }else{
                StopNode(false);
            }
        }
    }
}