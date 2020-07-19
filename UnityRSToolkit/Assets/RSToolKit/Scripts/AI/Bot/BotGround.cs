using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Locomotion;
using UnityEngine.AI;

namespace RSToolkit.AI
{
    public class BotGround : BotLocomotive
    {
        bool m_freefall = false;
        BotLogicNavMesh _botNavMesh;


        #region Components

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

        protected BotWanderNavMesh BotWanderNavMeshComponent {get; private set;}
        #endregion Components

        private void HandleFailling()
        {
            if (IsFarFromGround() && !m_freefall)
            {
                m_freefall = true;
                NavMeshAgentComponent.enabled = false;
            }
        }

        public override void ToggleComponentsForNetwork(bool owner)
        {
            base.ToggleComponentsForNetwork(owner);
            if (m_freefall && !owner)
            {
                m_freefall = false;
            }
            else if (owner)
            {
                HandleFailling();
            }
        }

        protected override void InitLocomotionTypes(){
            _botNavMesh = new BotLogicNavMesh(this, NavMeshAgentComponent);
            CurrentLocomotionType = _botNavMesh;
        }
        protected override bool InitBotWander(){
            if(!base.InitBotWander()){
                return false;
            }
            BotWanderNavMeshComponent = GetComponent<BotWanderNavMesh>();
            BotWanderManagerComponent.Initialize(BotWanderNavMeshComponent);
            return true;
        }

        #region MonoBehaviour Functions

        protected override void Awake()
        {
            base.Awake();
            HandleFailling();
        }

        protected override void Update()
        {
            base.Update();
            // There must be a better way to do this
            if (m_freefall && IsAlmostGrounded())
            {
                m_freefall = false;
                RigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                RigidBodyComponent.velocity = Vector3.zero;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                NavMeshAgentComponent.enabled = true;

            }
        }

        #endregion MonoBehaviour Functions
    }
}