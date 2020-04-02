using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = System.Object;

namespace RSToolkit.AI
{
    public class FiniteStateMachineRunner : MonoBehaviour
    {
        private List<IFiniteStateMachine> stateMachineList = new List<IFiniteStateMachine>();

        /// <summary>
        /// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. 
        /// </summary>
        /// <typeparam name="T">An Enum listing different state transitions</typeparam>
        /// <param name="component">The component whose state will be managed</param>
        /// <returns></returns>
        public FiniteStateMachine<T> Initialize<T>(MonoBehaviour component) where T : struct, IConvertible, IComparable
        {
            var fsm = new FiniteStateMachine<T>(this, component);

            stateMachineList.Add(fsm);

            return fsm;
        }

        /// <summary>
        /// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the startState
        /// </summary>
        /// <typeparam name="T">An Enum listing different state transitions</typeparam>
        /// <param name="component">The component whose state will be managed</param>
        /// <param name="startState">The default start state</param>
        /// <returns></returns>
        public FiniteStateMachine<T> Initialize<T>(MonoBehaviour component, T startState) where T : struct, IConvertible, IComparable
        {
            var fsm = Initialize<T>(component);

            fsm.ChangeState(startState);

            return fsm;
        }

        void FixedUpdate()
        {
            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled) fsm.CurrentStateMap.FixedUpdate();
            }
        }

        void Update()
        {
            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled)
                {
                    fsm.CurrentStateMap.Update();
                }
            }
        }

        void LateUpdate()
        {
            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled)
                {
                    fsm.CurrentStateMap.LateUpdate();
                }
            }
        }

        //void OnCollisionEnter(Collision collision)
        //{
        //	if(currentState != null && !IsInTransition)
        //	{
        //		currentState.OnCollisionEnter(collision);
        //	}
        //}

        public static void DoNothing()
        {
        }

        public static void DoNothingCollider(Collider other)
        {
        }

        public static void DoNothingCollision(Collision other)
        {
        }

        public static IEnumerator DoNothingCoroutine()
        {
            yield break;
        }
    }

    public class FiniteStateMapping
    {
        public object state;

        public bool hasEnterRoutine;
        public Action EnterCall = FiniteStateMachineRunner.DoNothing;
        public Func<IEnumerator> EnterRoutine = FiniteStateMachineRunner.DoNothingCoroutine;

        public bool hasExitRoutine;
        public Action ExitCall = FiniteStateMachineRunner.DoNothing;
        public Func<IEnumerator> ExitRoutine = FiniteStateMachineRunner.DoNothingCoroutine;

        public Action Finally = FiniteStateMachineRunner.DoNothing;
        public Action Update = FiniteStateMachineRunner.DoNothing;
        public Action LateUpdate = FiniteStateMachineRunner.DoNothing;
        public Action FixedUpdate = FiniteStateMachineRunner.DoNothing;
        public Action<Collision> OnCollisionEnter = FiniteStateMachineRunner.DoNothingCollision;

        public FiniteStateMapping(object state)
        {
            this.state = state;
        }

    }
}