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
        bool m_freefall = false;
        public BotLogicNavMesh BotNavMeshRef { get; private set; }
        public BotLogicFlight BotFlyingRef { get; private set; }

        public enum FlyableStates
        {
            NotFlying,
            Landing,
            TakingOff,
            Flying
        }

        public FlyableStates CurrentFlyableState
        {
            get
            {
                return FSMFlyable.CurrentState;
            }
        }

        public float FlightMagnitudeModifier = 1.25f;

        public override float InteractionMagnitude
        {
            get
            {
                if (CurrentFlyableState != FlyableStates.NotFlying)
                {
                    return base.InteractionMagnitude * FlightMagnitudeModifier;
                }
                return base.InteractionMagnitude;
            }
        }

        public BTFiniteStateMachine<FlyableStates> FSMFlyable { get; private set; } = new BTFiniteStateMachine<FlyableStates>(FlyableStates.Flying);

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
        protected BotWanderNavMesh BotWanderNavMeshComponent {get; private set;}

        protected BotWanderFlying BotWanderFlyingComponent{get; private set;}

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

        public override void ToggleComponentsForNetwork(bool owner)
        {
            base.ToggleComponentsForNetwork(owner);
            if (!owner)
            {
                NavMeshAgentComponent.enabled = false;
                Flying3DObjectComponent.enabled = false;
            }
            else
            {
                BotFSMLocomotionComponent.GroundProximityCheckerComponent.enabled = true;
                ToggleFlight(CurrentFlyableState != FlyableStates.NotFlying);
            }
        }

        private void ToggleFlight(bool on)
        {
            if (on)
            {              
                RigidBodyComponent.constraints = RigidbodyConstraints.None;
                CurrentLocomotionType = BotFlyingRef;
                BotWanderManagerComponent?.SetCurrentBotWander(BotWanderFlyingComponent);
            }
            else
            {
                RigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                RigidBodyComponent.velocity = Vector3.zero;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);                
                CurrentLocomotionType = BotNavMeshRef;
                BotWanderManagerComponent?.SetCurrentBotWander(BotWanderNavMeshComponent);
            }
            
            NavMeshAgentComponent.enabled = !on;            
            Flying3DObjectComponent.enabled = on;
            
        }

        public bool TakeOff()
        {
            if (CurrentFlyableState == FlyableStates.NotFlying)
            {
                FSMFlyable.ChangeState(FlyableStates.TakingOff);
                return true;
            }

            return false;
        }

        public bool Land(bool onNavMesh = true, bool freefall = false)
        {

            if (CurrentFlyableState != FlyableStates.Flying || (onNavMesh && !NavMeshAgentComponent.IsAboveNavMeshSurface()))
            {
                return false;
            }
            m_freefall = freefall;
            FSMFlyable.ChangeState(FlyableStates.Landing);

            return true;
        }

        public bool CanMove()
        {
            return CurrentFlyableState == FlyableStates.Flying || CurrentFlyableState == FlyableStates.NotFlying;
        }

        public bool IsAboveNavMeshSurface()
        {            
            return BotNavMeshRef.IsAboveNavMeshSurface();
        }


        private void InitStates()
        {
            FSMFlyable.OnStarted_AddListener(FlyableStates.TakingOff, TakingOff_Enter);
            FSMFlyable.SetUpdateAction(FlyableStates.TakingOff, TakingOff_Update);
            FSMFlyable.OnStopped_AddListener(FlyableStates.TakingOff, TakingOff_Exit);

            FSMFlyable.OnStarted_AddListener(FlyableStates.Landing, Landing_Enter);
            FSMFlyable.SetUpdateAction(FlyableStates.Landing, Landing_Update);

            FSMFlyable.OnStarted_AddListener(FlyableStates.NotFlying, NotFlying_Enter);
            FSMFlyable.OnStopped_AddListener(FlyableStates.NotFlying, NotFlying_Exit);

            FSMFlyable.OnStarted_AddListener(FlyableStates.Flying, Flying_Enter);
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
            if(!BotFSMLocomotionComponent.IsFarFromGround()) // IsCloseToGround())
            {
                Flying3DObjectComponent.ApplyVerticalThrust(true);       
            }           
            else
            {
                RigidBodyComponent.Sleep();
                FSMFlyable.ChangeState(FlyableStates.Flying);
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
            if (!m_freefall)
            {
                Flying3DObjectComponent.ApplyVerticalThrust(false);
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

        protected override void InitLocomotionTypes(){
            BotNavMeshRef = new BotLogicNavMesh(BotFSMLocomotionComponent, NavMeshAgentComponent);
            BotFlyingRef = new BotLogicFlight(BotFSMLocomotionComponent, Flying3DObjectComponent);
            InitStates();

            BTFiniteStateMachineManagerComponent.AddFSM(FSMFlyable);
        }

        protected override bool InitBotWander(){
            if(!base.InitBotWander()){
                return false;
            }
            BotWanderFlyingComponent = GetComponent<BotWanderFlying>();
            BotWanderNavMeshComponent = GetComponent<BotWanderNavMesh>();
            if(CurrentFlyableState != FlyableStates.NotFlying){
                BotWanderManagerComponent.Initialize(BotWanderFlyingComponent);
            }else{
                BotWanderManagerComponent.Initialize(BotWanderNavMeshComponent);
            }
            return true;
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
            if (CurrentFlyableState == FlyableStates.Landing)
            {
                NavMeshHit navHit;
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    if (NavMesh.SamplePosition(collision.contacts[i].point, out navHit, 1f, NavMesh.AllAreas))
                    {
                        FSMFlyable.ChangeState(FlyableStates.NotFlying);
                        break;
                    }
                }
            }
        }

        #endregion MonoBehaviour Functions

    }
}