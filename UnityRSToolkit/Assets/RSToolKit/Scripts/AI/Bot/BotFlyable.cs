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
            if (ProximityCheckerComponent.IsWithinRayDistance() != null && ProximityCheckerComponent.IsWithinRayDistance() < 0.1f)
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
                    BotWanderNavMeshComponent.Wander();
                    result = true;
                    break;
            }

            return result;
        }

        public bool StopWandering()
        {
            bool result = false;
            switch (CurrentState)
            {
                case FlyableStates.Grounded:
                    BotWanderNavMeshComponent?.StopWandering();
                    result = true;
                    break;
                case FlyableStates.Flying:
                    BotWanderNavMeshComponent.StopWandering();
                    result = true;
                    break;
            }

            return result;
        }


        void Awake()
        {
            m_fsm = FiniteStateMachine<FlyableStates>.Initialize(this);
            // ProximityCheckerComponent.OnProximityEntered.AddListener(OnProximityEntered_Listener);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                TakeOff();
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                Land(true);
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                Land(true);
            }
        }
        /*
        private void OnProximityEntered_Listener()
        {
            if (AutoLandWhenCloseToGround)
            {
                Land(true);
            }
        }*/
    }
}