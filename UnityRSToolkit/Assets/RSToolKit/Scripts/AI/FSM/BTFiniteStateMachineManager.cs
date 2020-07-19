using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RSToolkit.AI.Behaviour;
using UnityEngine;

namespace RSToolkit.AI.FSM
{
    [DisallowMultipleComponent]
    public class BTFiniteStateMachineManager : MonoBehaviour
    {
        private List<IBTFiniteStateMachine> _fsmList = new List<IBTFiniteStateMachine>();
        public ReadOnlyCollection<IBTFiniteStateMachine> FSMList { get { return _fsmList.AsReadOnly(); } }

        public BehaviourRootNode FSMBehaviourtree { get; private set; } = new BehaviourRootNode();
        private BehaviourParallel _parallelfsm = new BehaviourParallel(BehaviourParallel.StopCondition.ALL_CHILDREN,
                                                                        BehaviourParallel.StopCondition.ALL_CHILDREN);

        public void AddFSM(IBTFiniteStateMachine fsm)
        {
            
            if (_fsmList.Contains(fsm))
            {
                throw new Exception($"{fsm.GetName()} already exists");
            }

            _fsmList.Add(fsm);
            _parallelfsm.AddChild(fsm.GetFSMNode());
            _parallelfsm.OnChildNodeStopped.AddListener(ParallelFSMOnChildNodeStopped);
        }

        void ParallelFSMOnChildNodeStopped(BehaviourNode child, bool success){
            child.StartNodeOnNextTick();
        }

        public BTFiniteStateMachine<T> AddFSM<T>(T initialState, string name = "") where T : struct, IConvertible, IComparable
        {
            var fsm = new BTFiniteStateMachine<T>(initialState, name);
            AddFSM(fsm);
            return fsm;
        }

        public void StartFSMs(bool silent = false)
        {
            if (!FSMBehaviourtree.Children.Contains(_parallelfsm))
            {
                FSMBehaviourtree.Name = gameObject.name;
                _parallelfsm.Name = "State Machines";
                FSMBehaviourtree.AddChild(_parallelfsm);
            }
            FSMBehaviourtree.StartNode();
        }

        private void UpdateCommon(BehaviourNode.UpdateType updateType)
        {
            BehaviourNode.OverrideElapsedTime(Time.time);
            FSMBehaviourtree?.UpdateRecursively(updateType);
        }

        void Update()
        {
            UpdateCommon(BehaviourNode.UpdateType.DEFAULT);
        }

        void FixedUpdate()
        {
            UpdateCommon(BehaviourNode.UpdateType.FIXED);
        }

        void LateUpdate()
        {
            UpdateCommon(BehaviourNode.UpdateType.LATE);
        }
    }
}
