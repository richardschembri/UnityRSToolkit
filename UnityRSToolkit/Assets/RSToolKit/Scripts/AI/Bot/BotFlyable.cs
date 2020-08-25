using RSToolkit.Space3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Animation;
using RSToolkit.AI.Helpers;
using RSToolkit.AI.Locomotion;
using UnityEngine.AI;
using RSToolkit.AI.FSM;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Flying3DObject))]
    [RequireComponent(typeof(ProximityChecker))]
    public class BotFlyable : BotLocomotive
    {
        public bool StartInAir = true;
        bool _freefall = false;

        public BotLogicNavMesh BotLogicNavMeshRef { get; set; }
        public BotLogicFlight BotLogicFlyingRef { get; set; }

        public enum FStatesFlyable
        {
            NotFlying = 0,
            Landing = 1,
            TakingOff = 2,
            Flying = 3
        }

        public FStatesFlyable CurrentFlyableState
        {
            get
            {
#if UNITY_EDITOR
                if (FSMFlyable == null)
                {
                    return StartInAir ? FStatesFlyable.Flying : FStatesFlyable.NotFlying;
                }
#endif
                return FSMFlyable.CurrentState;
            }
        }

        public float FlightMagnitudeModifier = 1.25f;

        public override float InteractionMagnitude
        {
            get
            {
                if (CurrentFlyableState != FStatesFlyable.NotFlying)
                {
                    return base.InteractionMagnitude * FlightMagnitudeModifier;
                }
                return base.InteractionMagnitude;
            }
        }

        public BTFiniteStateMachine<FStatesFlyable> FSMFlyable { get; private set; } // = new BTFiniteStateMachine<FStatesFlyable>(FStatesFlyable.Flying);

        #region Components

        private BotLocomotive m_botFSMLocomotionComponent;
        public BotLocomotive BotFSMLocomotionComponent
        {
            get
            {
                if (m_botFSMLocomotionComponent == null)
                {
                    m_botFSMLocomotionComponent = GetComponent<BotLocomotive>();
                }
                return m_botFSMLocomotionComponent;
            }
        }

        private NavMeshAgent _navMeshAgentComponent;
        public NavMeshAgent NavMeshAgentComponent
        {
            get
            {
                if (_navMeshAgentComponent == null)
                {
                    _navMeshAgentComponent = GetComponent<NavMeshAgent>();
                }

                return _navMeshAgentComponent;
            }

        }
        protected BotPartWanderNavMesh BotWanderNavMeshComponent { get; private set; }

        protected BotPartWanderFlying BotWanderFlyingComponent { get; private set; }

        private Flying3DObject m_flying3DObjectComponent;
        public Flying3DObject Flying3DObjectComponent
        {
            get
            {
                if (m_flying3DObjectComponent == null)
                {
                    m_flying3DObjectComponent = GetComponent<Flying3DObject>();
                }
                return m_flying3DObjectComponent;
            }
        }

        private Rigidbody m_rigidBodyComponent;
        public Rigidbody RigidBodyComponent
        {
            get
            {
                if (m_rigidBodyComponent == null)
                {
                    m_rigidBodyComponent = GetComponent<Rigidbody>();
                }

                return m_rigidBodyComponent;
            }

        }

        #endregion Components

        protected override void ToggleComponentsForNetwork()
        {
            base.ToggleComponentsForNetwork();
            if (_IsNetworkPeer)
            {
                NavMeshAgentComponent.enabled = false;
                Flying3DObjectComponent.enabled = false;
            }
            else
            {
                BotFSMLocomotionComponent.GroundProximityCheckerComponent.enabled = true;
                ToggleFlight(CurrentFlyableState != FStatesFlyable.NotFlying);
            }
        }

        private void ToggleFlight(bool on)
        {
            if (on)
            {
                RigidBodyComponent.constraints = RigidbodyConstraints.None;
                CurrentLocomotionType = BotLogicFlyingRef;
                BotWanderManagerComponent?.SetCurrentBotWander(BotWanderFlyingComponent);
            }
            else
            {
                RigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                RigidBodyComponent.velocity = Vector3.zero;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                CurrentLocomotionType = BotLogicNavMeshRef;
                BotWanderManagerComponent?.SetCurrentBotWander(BotWanderNavMeshComponent);
            }

            NavMeshAgentComponent.enabled = !on;
            Flying3DObjectComponent.enabled = on;

        }

        public bool TakeOff()
        {
            if (CurrentFlyableState == FStatesFlyable.NotFlying)
            {
                FSMFlyable.ChangeState(FStatesFlyable.TakingOff);
                return true;
            }

            return false;
        }

        public bool Land(bool onNavMesh = true, bool freefall = false)
        {

            if (CurrentFlyableState != FStatesFlyable.Flying || (onNavMesh && !NavMeshAgentComponent.IsAboveNavMeshSurface()))
            {
                return false;
            }
            _freefall = freefall;
            FSMFlyable.ChangeState(FStatesFlyable.Landing);

            return true;
        }

        public bool CanMove()
        {
            return CurrentFlyableState == FStatesFlyable.Flying || CurrentFlyableState == FStatesFlyable.NotFlying;
        }

        public bool IsAboveNavMeshSurface()
        {
            return BotLogicNavMeshRef.IsAboveNavMeshSurface();
        }


        private void InitStates()
        {
            FSMFlyable = new BTFiniteStateMachine<FStatesFlyable>(StartInAir ? FStatesFlyable.Flying : FStatesFlyable.NotFlying);
            FSMFlyable.OnStarted_AddListener(FStatesFlyable.TakingOff, TakingOff_Enter);
            FSMFlyable.SetUpdateAction(FStatesFlyable.TakingOff, TakingOff_Update);
            FSMFlyable.OnStopped_AddListener(FStatesFlyable.TakingOff, TakingOff_Exit);

            FSMFlyable.OnStarted_AddListener(FStatesFlyable.Landing, Landing_Enter);
            FSMFlyable.SetUpdateAction(FStatesFlyable.Landing, Landing_Update);

            FSMFlyable.OnStarted_AddListener(FStatesFlyable.NotFlying, NotFlying_Enter);
            FSMFlyable.OnStopped_AddListener(FStatesFlyable.NotFlying, NotFlying_Exit);

            FSMFlyable.OnStarted_AddListener(FStatesFlyable.Flying, Flying_Enter);

            FSMFlyable.OnStateChanged_AddListener(OnFlyableStateChanged_Listener);
            CharacterAnimParams.TrySetFStateFlyable(AnimatorComponent, (int)FSMFlyable.CurrentState);
        }

        #region TakingOff State
        void TakingOff_Enter()
        {
            BotWanderManagerComponent?.StopWandering();
            Flying3DObjectComponent.HoverWhenIdle = true;
            ToggleFlight(true);
        }

        void TakingOff_Update()
        {
            if (BotFSMLocomotionComponent.IsCloseToGround()) // .IsFarFromGround()) // 
            {
                Flying3DObjectComponent.ApplyVerticalThrust(true);
            }
            else
            {
                RigidBodyComponent.Sleep();
                FSMFlyable.ChangeState(FStatesFlyable.Flying);
            }
        }

        void TakingOff_Exit(bool success)
        {
            RigidBodyComponent.WakeUp();
        }
        #endregion TakingOff State

        #region Landing State

        void Landing_Enter()
        {
            BotWanderManagerComponent?.StopWandering();
            Flying3DObjectComponent.HoverWhenIdle = false;

        }

        void Landing_Update()
        {
            if (!_freefall)
            {
                Flying3DObjectComponent.ApplyVerticalThrust(false);
            }
            if (IsAlmostGrounded())
            {
                FSMFlyable.ChangeState(FStatesFlyable.NotFlying);
            }
        }

        #endregion Landing State

        #region NotFlying State

        void NotFlying_Enter()
        {
            ToggleFlight(false);
            CharacterAnimParams.TrySetIsGrounded(AnimatorComponent, true);
        }

        void NotFlying_Exit(bool success)
        {
            ToggleFlight(false);
            CharacterAnimParams.TrySetIsGrounded(AnimatorComponent, false);
        }

        #endregion NotFlying State

        #region Flying State

        void Flying_Enter()
        {
            ToggleFlight(true);
        }

        #endregion Flying State

        protected override void InitLocomotionTypes()
        {
            BotLogicNavMeshRef = new BotLogicNavMesh(BotFSMLocomotionComponent, NavMeshAgentComponent, JumpProximityChecker);
            BotLogicFlyingRef = new BotLogicFlight(BotFSMLocomotionComponent, Flying3DObjectComponent);
            InitStates();

            BTFiniteStateMachineManagerComponent.AddFSM(FSMFlyable);
        }

        protected override bool InitBotWander()
        {
            if (!base.InitBotWander())
            {
                return false;
            }
            BotWanderFlyingComponent = GetComponent<BotPartWanderFlying>();
            BotWanderNavMeshComponent = GetComponent<BotPartWanderNavMesh>();
            if (CurrentFlyableState != FStatesFlyable.NotFlying)
            {
                BotWanderManagerComponent.Initialize(BotWanderFlyingComponent);
            }
            else
            {
                BotWanderManagerComponent.Initialize(BotWanderNavMeshComponent);
            }
            return true;
        }

        private void OnFlyableStateChanged_Listener(BotFlyable.FStatesFlyable state)
        {
            CharacterAnimParams.TrySetFStateFlyable(AnimatorComponent, (int)state);
        }

        #region MonoBehaviour Functions

        protected override void Awake()
        {
            base.Awake();

        }

        protected override void Update()
        {
            base.Update();
            CharacterAnimParams.TrySetSpeed(AnimatorComponent, BotFSMLocomotionComponent.CurrentSpeed);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (CurrentFlyableState == FStatesFlyable.Landing)
            {
                NavMeshHit navHit;
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    if (NavMesh.SamplePosition(collision.contacts[i].point, out navHit, 1f, NavMesh.AllAreas))
                    {
                        FSMFlyable.ChangeState(FStatesFlyable.NotFlying);
                        break;
                    }
                }
            }
        }

        #endregion MonoBehaviour Functions

    }
}