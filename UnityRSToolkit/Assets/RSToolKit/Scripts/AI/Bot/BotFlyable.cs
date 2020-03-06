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
        public bool AutoLandWhenCloseToGround = false;
        //public float TakeOffForce
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

        public void ToggleFlight(bool on)
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

        public void TakeOff()
        {
            ToggleFlight(true);
            BotFlyingComponent.Flying3DObjectComponent.ApplyVerticalThrust(true);
            BotFlyingComponent.Flying3DObjectComponent.ApplyVerticalThrust(true);
        }

        public bool Land(bool checkForGround = true)
        {
            if (checkForGround && ProximityCheckerComponent.IsWithinRayDistance() == null)
            {
                return false;
            }
            ToggleFlight(false);
            return true;
        }


        void Awake()
        {
            ProximityCheckerComponent.OnProximityEntered.AddListener(OnProximityEntered_Listener);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                TakeOff();
            }
        }

        private void OnProximityEntered_Listener()
        {
            if (AutoLandWhenCloseToGround)
            {
                Land(true);
            }
        }
    }
}