using RSToolkit.Space3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(BotNavMesh))]
    [RequireComponent(typeof(BotFlying))]
    [RequireComponent(typeof(ProximityChecker))]
    public class BotFlyable : MonoBehaviour
    {
        public FlyableStates StartState = FlyableStates.Flying;
        public enum FlyableStates
        {
            Grounded,
            Landing,
            TakingOff,
            Flying
        }

        public bool AutoLandWhenCloseToGround = false;
        private BotNavMesh m_botNavMeshComponent;
        public BotNavMesh BotNavMeshComponent
        {
            get
            {
                if (m_botNavMeshComponent == null)
                {
                    m_botNavMeshComponent = GetComponent<BotNavMesh>();
                }
                return m_botNavMeshComponent;
            }

        }

        private BotFlying m_botFlyingComponent;
        public BotFlying BotFlyingComponent
        {
            get
            {
                if (m_botFlyingComponent == null)
                {
                    m_botFlyingComponent = GetComponent<BotFlying>();
                }

                return m_botFlyingComponent;
            }

        }

        private BotWanderNavMesh m_botWanderNavMeshComponent;
        public BotWanderNavMesh BotWanderNavMeshComponent
        {
            get
            {
                if(m_botWanderNavMeshComponent == null)
                {
                    m_botWanderNavMeshComponent = GetComponent<BotWanderNavMesh>();
                }
                return m_botWanderNavMeshComponent;
            }
        }

        private BotWanderFlying m_botWanderFlyingComponent;
        public BotWanderFlying BotWanderFlyingComponent
        {
            get
            {
                if (m_botWanderFlyingComponent == null)
                {
                    m_botWanderFlyingComponent = GetComponent<BotWanderFlying>();
                }
                return m_botWanderFlyingComponent;
            }
        }

        private ProximityChecker m_proximityCheckerComponent;
        public ProximityChecker ProximityCheckerComponent
        {
            get
            {
                if (m_proximityCheckerComponent == null)
                {
                    m_proximityCheckerComponent = GetComponent<ProximityChecker>();
                }

                return m_proximityCheckerComponent;
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


        protected FiniteStateMachine<FlyableStates> m_fsm;

        public FlyableStates CurrentState
        {
            get
            {
                return m_fsm.State;
            }
        }

        public void AddStateChangedListener(System.Action<FlyableStates> listener)
        {
            m_fsm.Changed += listener;
        }

        public void RemoveStateChangedListener(System.Action<FlyableStates> listener)
        {
            m_fsm.Changed -= listener;
        }

        private void ToggleFlight(bool on)
        {
            if (on)
            {
                RigidBodyComponent.constraints = RigidbodyConstraints.None;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                RigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                RigidBodyComponent.velocity = Vector3.zero;
            }
            

            BotNavMeshComponent.NavMeshAgentComponent.enabled = !on;
            BotNavMeshComponent.enabled = !on;
            BotFlyingComponent.Flying3DObjectComponent.enabled = on;
            BotFlyingComponent.enabled = on;
        }

        public bool TakeOff()
        {
            if (CurrentState == FlyableStates.Grounded)
            {
                m_fsm.ChangeState(FlyableStates.TakingOff);
                return true;
            }

            return false;
        }

        void TakingOff_Enter()
        {
            StopWandering();
            BotFlyingComponent.Flying3DObjectComponent.HoverWhenIdle = true;
            ToggleFlight(true);
        }

        void TakingOff_Update()
        {
            if(ProximityCheckerComponent.IsWithinRayDistance() != null)
            {
                BotFlyingComponent.Flying3DObjectComponent.ApplyVerticalThrust(true);
            }
            else
            {
                BotFlyingComponent.Flying3DObjectComponent.ApplyVerticalThrust(true);
                m_fsm.ChangeState(FlyableStates.Flying);
            }
        }

        public bool Land(bool checkForGround = true)
        {
            if (checkForGround && ProximityCheckerComponent.IsWithinRayDistance() == null)
            {
                return false;
            }
            m_fsm.ChangeState(FlyableStates.Landing);
            
            return true;
        }

        void Landing_Enter()
        {
            StopWandering();
            BotFlyingComponent.Flying3DObjectComponent.HoverWhenIdle = false;
            
        }

        void Landing_Update()
        {
            if (ProximityCheckerComponent.IsWithinRayDistance() != null && ProximityCheckerComponent.IsWithinRayDistance() < 0.2f)
            {  
                m_fsm.ChangeState(FlyableStates.Grounded);
                
            }
        }

        void Grounded_Enter()
        {
            ToggleFlight(false);
        }

        public bool Wander()
        {
            bool result = false;
            switch (CurrentState)
            {
                case FlyableStates.Grounded:
                    BotWanderNavMeshComponent?.Wander();
                    result = true;
                    break;
                case FlyableStates.Flying:
                    BotWanderFlyingComponent?.Wander();
                    result = true;
                    break;
            }

            return result;
        }

        public bool StopWandering()
        {


            if((CurrentState == FlyableStates.Grounded || CurrentState == FlyableStates.TakingOff) && BotWanderNavMeshComponent != null)
            {
                return BotWanderNavMeshComponent.StopWandering();
            }
            else if((CurrentState == FlyableStates.Flying || CurrentState == FlyableStates.Landing) && BotWanderFlyingComponent != null)
            {
                return BotWanderFlyingComponent.StopWandering();
            }

            return false;

        }

        private bool IsBotWandering(BotWander botWander)
        {
            return botWander != null && botWander.IsWandering();
        }
      
        public bool IsWandering()
        {
            return IsBotWandering(BotWanderNavMeshComponent) || IsBotWandering(BotWanderFlyingComponent);
        }

        public void MoveToFocusedTarget(bool fullspeed = true)
        {
            if(CurrentState == FlyableStates.Flying)
            {
                BotFlyingComponent.FlyToTarget(fullspeed);
            }
            else if(CurrentState == FlyableStates.Grounded)
            {
                if (fullspeed)
                {
                    BotNavMeshComponent.RunToFocusedTarget();
                }
                else
                {
                    BotNavMeshComponent.WalkToFocusedTarget();
                }
                
            }
        }

        void Awake()
        {
            m_fsm = FiniteStateMachine<FlyableStates>.Initialize(this, StartState);
            m_fsm.Changed += Fsm_Changed;
            // ProximityCheckerComponent.OnProximityEntered.AddListener(OnProximityEntered_Listener);
        }

        private void Fsm_Changed(FlyableStates state)
        {
            try
            {
                Debug.Log($"{transform.name} FlyableStates changed from {m_fsm.LastState.ToString()} to {state.ToString()}");
            }
            catch (System.Exception ex)
            {
                Debug.Log($"{transform.name} FlyableStates changed to {state.ToString()}");
            }

        }
    }
}